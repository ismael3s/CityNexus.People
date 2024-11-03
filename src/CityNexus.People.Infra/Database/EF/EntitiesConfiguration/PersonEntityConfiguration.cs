using CityNexus.People.Domain.Entities;
using CityNexus.People.Domain.People;
using CityNexus.People.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityNexus.People.Infra.Database.EF.EntitiesConfiguration;

public class PersonEntityConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasIndex(p => p.Document).IsUnique();
        builder.HasIndex(p => p.Email).IsUnique();
        builder.Property(p => p.Name).HasConversion(p => p.Value, p => new Name(p));
        builder.Property(p => p.Email).HasConversion(p => p.Value, p => new Email(p));
        builder.Property(p => p.Document).HasConversion(p => p.Value, p => new Document(p));
        builder.HasKey(p => p.Id);
    }
}
