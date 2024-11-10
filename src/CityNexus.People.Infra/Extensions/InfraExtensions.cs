using CityNexus.People.Application.Abstractions;
using CityNexus.People.Application.People.Commands.RegisterPerson;
using CityNexus.People.Application.People.Repositories;
using CityNexus.People.Domain.People.Events;
using CityNexus.People.Infra.Configuration;
using CityNexus.People.Infra.Database.Dapper;
using CityNexus.People.Infra.Database.EF;
using CityNexus.People.Infra.Database.EF.Repositories;
using CityNexus.People.Infra.OutboxMessage;
using Dapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using RabbitMQ.Client;

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
        ConfigureOptions(services, configuration);
        AddTelemetry(services, configuration);
        AddDatabase(services, connectionString);
        AddRepositoriesImplementation(services);
        AddBackgroundJobs(services, configuration);
        AddMassTransit(services, configuration);
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
        services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMq"));
        return services;
    }

    private static void AddRepositoriesImplementation(IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>().AddOpenTelemetry();
    }

    private static void AddDatabase(IServiceCollection services, string connectionString)
    {
        SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(
            connectionString
        ));
    }

    private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddQuartz()
            .AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true)
            .ConfigureOptions<ProcessOutboxJobSetup>();
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitmqCfg = configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();
        services.AddMassTransit(x =>
        {
            x.AddConfigureEndpointsCallback(
                (context, name, cfg) =>
                {
                    cfg.UseMessageRetry(r =>
                        r.Exponential(
                            5,
                            TimeSpan.FromSeconds(10),
                            TimeSpan.FromMinutes(2),
                            TimeSpan.FromSeconds(10)
                        )
                    );
                }
            );
            x.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.Host(
                        rabbitmqCfg!.Host,
                        rabbitmqCfg.Port,
                        rabbitmqCfg.VHost,
                        hostCfg =>
                        {
                            hostCfg.Username(rabbitmqCfg.Username);
                            hostCfg.Password(rabbitmqCfg.Password);
                        }
                    );
                    cfg.Message<RegisteredPersonDomainEvent>(cfg =>
                    {
                        cfg.SetEntityName("person.registered");
                    });
                    MessageCorrelation.UseCorrelationId<RegisteredPersonDomainEvent>(@event =>
                        @event.Id
                    );
                    cfg.Publish<RegisteredPersonDomainEvent>(cfg =>
                    {
                        cfg.ExchangeType = ExchangeType.Fanout;
                        cfg.Durable = true;

                        cfg.BindQueue(
                            "person.registered",
                            "person.registered.notifications",
                            que =>
                            {
                                que.SetQueueArgument(
                                    "x-dead-letter-exchange",
                                    "dlq.person.registered"
                                );
                                que.SetQueueArgument(
                                    "x-dead-letter-routing-key",
                                    "dlq.person.registered.notifications"
                                );
                            }
                        );
                        cfg.BindQueue("person.registered", "person.registered.contracts");
                    });

                    cfg.ConfigureEndpoints(context);
                }
            );
        });
    }
}
