using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using CityNexus.People.Domain.Abstractions;
using CityNexus.People.Domain.Entities;
using CityNexus.People.Domain.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CityNexus.People.Infra.Database.EF;

public sealed class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Person> People { get; set; }

    public DbSet<Outbox> Outbox { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var options = optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddDomainEventsAsOutboxMessages();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new Outbox(
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings()
                    {
                        ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new CamelCaseNamingStrategy(),
                        },
                    }
                )
            ))
            .ToList();

        AddRange(outboxMessages);
    }
}
