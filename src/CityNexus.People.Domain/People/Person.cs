using CityNexus.People.Domain.Abstractions;
using CityNexus.People.Domain.People.Events;
using CityNexus.People.Domain.VO;

namespace CityNexus.People.Domain.People;

public sealed class Person : Entity
{
    public Name Name { get; private set; } = default!;
    public Document Document { get; private set; } = default!;
    public Email Email { get; private set; } = default!;

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Person() { }

    public static Person Create(string fullName, string anEmail, string cpf)
    {
        var document = Document.Create(cpf);
        var email = Email.Create(anEmail);
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = Name.From(fullName),
            Document = document,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        person.RaiseDomainEvent(new RegisteredPersonDomainEvent(person.Id));
        return person;
    }
}