using CityNexus.People.Application.Abstractions;
using CityNexus.People.Application.People.Repositories;
using CityNexus.People.Infra.Database.Dapper;
using CityNexus.People.Infra.Database.EF;
using CityNexus.People.Infra.Database.EF.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(
            connectionString
        ));
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        return services;
    }
}
