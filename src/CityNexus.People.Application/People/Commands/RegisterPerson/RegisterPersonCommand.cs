using CityNexus.People.Application.Abstractions;
using CityNexus.People.Application.People.Repositories;
using CityNexus.People.Domain.People;

namespace CityNexus.People.Application.People.Commands.RegisterPerson;

public sealed class RegisterPersonCommand(
    IPersonRepository personRepository,
    IUnitOfWork unitOfWork
)
{
    public sealed record Input(string Name, string Email, string Document);

    public async Task Handle(Input input, CancellationToken cancellationToken = default!)
    {
        var person = Person.Create(input.Name, input.Email, input.Document);
        var personByDocument = await personRepository.FindByCpf(person.Document, cancellationToken);
        if (personByDocument is not null)
            throw new Exception("CPF is already in use");
        var personByEmail = await personRepository.FindByEmail(person.Email, cancellationToken);
        if (personByEmail is not null)
            throw new Exception("Email is already in use");
        await personRepository.AddAsync(person, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
