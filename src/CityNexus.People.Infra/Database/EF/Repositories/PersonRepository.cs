using CityNexus.People.Application.People.Repositories;
using CityNexus.People.Domain.Entities;
using CityNexus.People.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace CityNexus.People.Infra.Database.EF.Repositories;

public sealed class PersonRepository(ApplicationDbContext dbContext) : IPersonRepository
{
    public async Task AddAsync(Person people, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(people, cancellationToken);
    }

    public Task<Person?> FindByCpf(Document document, CancellationToken cancellationToken = default)
    {
        return dbContext.People.FirstOrDefaultAsync(p => p.Document == document, cancellationToken);
    }

    public Task<Person?> FindByEmail(Email email, CancellationToken cancellationToken = default)
    {
        return dbContext.People.FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }
}
