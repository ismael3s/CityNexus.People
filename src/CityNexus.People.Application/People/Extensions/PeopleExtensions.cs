using CityNexus.People.Application.People.Commands.RegisterPerson;
using CityNexus.People.Application.People.Queries.FindPeople;
using Microsoft.Extensions.DependencyInjection;

namespace CityNexus.People.Application.People.Extensions;

internal static class PeopleExtensions
{
    public static IServiceCollection AddPeopleModule(this IServiceCollection services)
    {
        services.AddScoped<RegisterPersonCommand>().AddScoped<FindPeopleQueryHandler>();
        return services;
    }
}
