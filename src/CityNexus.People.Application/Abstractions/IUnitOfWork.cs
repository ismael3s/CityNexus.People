namespace CityNexus.People.Application.Abstractions;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default);
}
