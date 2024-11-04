using CityNexus.People.Application.People.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CityNexus.People.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddPeopleModule();
        return services;
    }
}
