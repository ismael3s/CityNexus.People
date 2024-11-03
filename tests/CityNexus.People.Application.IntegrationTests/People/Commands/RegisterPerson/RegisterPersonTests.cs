using System.Text.Json;
using CityNexus.People.Application.IntegrationTests.Common;
using CityNexus.People.Application.People.Commands.RegisterPerson;
using CityNexus.People.Domain.Entities;
using CityNexus.People.Domain.People;
using CityNexus.People.Domain.People.Events;
using CityNexus.People.Infra.Database.EF;
using CityNexus.People.Infra.Database.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CityNexus.People.Application.IntegrationTests.People.Commands.RegisterPerson;

[Collection("IntegrationSetup")]
public sealed class RegisterPersonTests(IntegrationTestSetup setup) : IAsyncLifetime
{
    public async Task DisposeAsync()
    {
        await setup.ResetAsync();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    private async Task<(RegisterPersonCommand, ApplicationDbContext)> MakeSut()
    {
        var applicationDbContext = new ApplicationDbContext(
            new DbContextOptionsBuilder()
                .UseSnakeCaseNamingConvention()
                .UseNpgsql(setup.ConnectionString)
                .Options
        );
        await applicationDbContext.Database.EnsureCreatedAsync();
        var unitOfWork = new UnitOfWork(applicationDbContext);
        var personRepository = new PersonRepository(applicationDbContext);
        var sut = new RegisterPersonCommand(personRepository, unitOfWork);
        return (sut, applicationDbContext);
    }

    [Fact]
    [Trait(
        "CreatePersonCommand - Integration",
        "Shouldn't be able to create a person when the provided data is invalid"
    )]
    public async Task Should_Throw_Validation_Failed_Exception_When_Invalid_Data()
    {
        var (sut, _) = await MakeSut();

        var input = new RegisterPersonCommand.Input("Ismael Souza", "ismael@gmail.com", null!);

        var action = () => sut.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    [Trait(
        "CreatePersonCommand - Integration",
        "Shouldn't be able to create a person when the the CPF already exists in the database"
    )]
    public async Task ShouldThrow_WhenDocumentIsAlreadyInUse()
    {
        var (sut, applicationDbContext) = await MakeSut();
        var person = Person.Create("Ismael Souza", "ismael@gmail.com", "57075723090");
        applicationDbContext.People.Add(person);
        await applicationDbContext.SaveChangesAsync();

        var input = new RegisterPersonCommand.Input(
            "Ismael Souza",
            "ismael@gmail.com",
            "57075723090"
        );

        var action = () => sut.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>().WithMessage("Cpf is already in use");
    }

    [Fact]
    [Trait(
        "CreatePersonCommand - Integration",
        "Shouldn't be able to create a person when the the Email already exists in the database"
    )]
    public async Task ShouldThrow_WhenEmailIsAlreadyInUse()
    {
        var (sut, applicationDbContext) = await MakeSut();
        var person = Person.Create("Ismael Souza", "ismael@gmail.com", "57075723090");
        applicationDbContext.People.Add(person);
        await applicationDbContext.SaveChangesAsync();
        var input = new RegisterPersonCommand.Input(
            "Ismael Souza",
            "ismael@gmail.com",
            "35381884087"
        );

        var action = () => sut.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>().WithMessage("Email is already in use");
    }

    [Fact]
    [Trait(
        "CreatePersonCommand - Integration",
        "Should be able to create a person when the provided data is valid"
    )]
    public async Task ShouldBeAbleToRegisterAPersonIntoTheNexus()
    {
        var (sut, applicationDbContext) = await MakeSut();
        var input = new RegisterPersonCommand.Input(
            "Ismael Souza",
            "ismael@gmail.com",
            "57075723090"
        );

        await sut.Handle(input, CancellationToken.None);

        var person = await applicationDbContext.People.FirstAsync();
        person.Should().NotBeNull();
        person.Id.Should().NotBeEmpty();
        person.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(1000));
        person.Name.Value.Should().Be("Ismael Souza");
        person.Email.Value.Should().Be("ismael@gmail.com");
        person.Document.Value.Should().Be("57075723090");

        var outboxEvents = await applicationDbContext.Outbox.ToListAsync();
        outboxEvents.Should().HaveCount(1);
        var outboxEvent = outboxEvents[0];
        outboxEvent.Id.Should().NotBeEmpty();
        outboxEvent.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(1000));
        outboxEvent.EventName.Should().Be(nameof(RegisteredPersonDomainEvent));
        outboxEvent
            .Payload.Should()
            .Be(
                JsonSerializer.Serialize(
                    new RegisteredPersonDomainEvent(person.Id),
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
    }
}
