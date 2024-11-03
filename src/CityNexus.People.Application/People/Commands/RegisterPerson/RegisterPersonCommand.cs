using CityNexus.People.Application.People.Gateways;
using CityNexus.People.Application.People.Repositories;
using CityNexus.People.Domain.Entities;

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
        await personRepository.AddAsync(person, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
