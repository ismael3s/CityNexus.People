using CityNexus.People.Domain.Entities;

namespace CityNexus.People.Application.People.Repositories;

public interface IPersonRepository
{
    public Task AddAsync(Person people, CancellationToken cancellationToken = default);
}
