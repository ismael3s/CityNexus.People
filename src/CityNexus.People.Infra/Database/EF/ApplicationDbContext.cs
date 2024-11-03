using CityNexus.People.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityNexus.People.Infra.Database.EF;

public sealed class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Person> People { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var options = optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
