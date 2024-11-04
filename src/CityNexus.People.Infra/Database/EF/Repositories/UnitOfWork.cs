using CityNexus.People.Application.Abstractions;

namespace CityNexus.People.Infra.Database.EF.Repositories;

public sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
