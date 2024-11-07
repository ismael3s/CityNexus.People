using CityNexus.People.Application.Abstractions;
using CityNexus.People.Application.People.Repositories;
using CityNexus.People.Infra.Configuration;
using CityNexus.People.Infra.Database.Dapper;
using CityNexus.People.Infra.Database.EF;
using CityNexus.People.Infra.Database.EF.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CityNexus.People.Infra.Extensions;

public static class InfraExtensions
{
    public static IServiceCollection AddInfra(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration));
        SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        ConfigureOptions(services, configuration);
        AddTelemetry(services, configuration);
        services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(
            connectionString
        ));
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>().AddOpenTelemetry();
        return services;
    }

    private static IServiceCollection AddTelemetry(
        IServiceCollection services,
        IConfiguration configuration
    )
    {
        var serviceName = configuration.GetValue<string>(
            "OTEL:SERVICE:NAME",
            "CityNexus.People.Api"
        );
        var serviceVersion = configuration.GetValue<string>("OTEL:SERVICE:VERSION", "0.0.1");
        services
            .AddOpenTelemetry()
            .ConfigureResource(r =>
                r.AddService(serviceName: serviceName, serviceVersion: serviceVersion)
            )
            .WithMetrics(metrics =>
                metrics.AddMeter(serviceName).AddConsoleExporter().AddOtlpExporter()
            )
            .WithTracing(tracing =>
                tracing
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter()
            );
        return services;
    }

    private static IServiceCollection ConfigureOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<TelemetryConfigurationOption>(configuration.GetSection("Telemetry"));
        return services;
    }
}
