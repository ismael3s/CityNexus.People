using CityNexus.People.Application.People.Repositories;
using CityNexus.People.Domain.Entities;

namespace CityNexus.People.Infra.Database.EF.Repositories;

public sealed class PersonRepository(ApplicationDbContext dbContext) : IPersonRepository
{
    public async Task AddAsync(Person people, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(people, cancellationToken);
    }
}
