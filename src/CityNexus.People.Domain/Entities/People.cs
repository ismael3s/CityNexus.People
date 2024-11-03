using CityNexus.People.Domain.VO;

namespace CityNexus.People.Domain.Entities;

public sealed class People
{
    public Guid Id { get; private set; }

    public Name Name { get; private set; } = default!;
    public Document Document { get; private set; } = default!;
    public Email Email { get; private set; } = default!;

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private People() { }

    public static People Create(string fullName, string anEmail, string cpf)
    {
        var document = Document.Create(cpf);
        var email = Email.Create(anEmail);
        var people = new People
        {
            Id = Guid.NewGuid(),
            Name = Name.From(fullName),
            Document = document,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        return people;
    }
}
