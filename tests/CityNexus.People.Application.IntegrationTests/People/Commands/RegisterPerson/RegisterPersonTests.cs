using CityNexus.People.Application.IntegrationTests.Common;
using CityNexus.People.Application.People.Commands.RegisterPerson;
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

    [Fact]
    [Trait(
        "CreatePersonCommand - Integration",
        "Shouldn't be able to create a person when the provided data is invalid"
    )]
    public async Task Should_Throw_Validation_Failed_Exception_When_Invalid_Data()
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
        var input = new RegisterPersonCommand.Input("Ismael Souza", "ismael@gmail.com", null!);

        var action = () => sut.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    [Trait(
        "CreatePersonCommand - Integration",
        "Should be able to create a person when the provided data is valid"
    )]
    public async Task ShouldBeAbleToRegisterAPersonIntoTheNexus()
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
    }
}
