using CityNexus.People.Domain.Entities;
using CityNexus.People.Domain.People;
using CityNexus.People.Domain.VO;

namespace CityNexus.People.Application.People.Repositories;

public interface IPersonRepository
{
    public Task AddAsync(Person people, CancellationToken cancellationToken = default);
    public Task<Person?> FindByCpf(
        Document document,
        CancellationToken cancellationToken = default
    );
    public Task<Person?> FindByEmail(Email email, CancellationToken cancellationToken = default);
}
