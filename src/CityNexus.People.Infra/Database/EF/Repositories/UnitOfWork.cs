using CityNexus.People.Application.People.Gateways;

namespace CityNexus.People.Infra.Database.EF.Repositories;

public sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
