namespace CityNexus.People.Application.People.Gateways;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default);
}
