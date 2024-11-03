namespace CityNexus.People.Domain.UnitTests.Entities;

using SUT = CityNexus.People.Domain.Entities.People;

public sealed class PeopleTests
{
    [Theory]
    [Trait("People - Unit", "Shouldn't be able to create a people when invalid is provided")]
    [InlineData(null, null, null)]
    [InlineData("Ismael Santana", null, null)]
    [InlineData("Ismael Santana", "ismael.santana@dev.com", null)]
    [InlineData("Ismael Santana", "ismael.santana@dev.com", "123.456.789-12")]
    public void People_Create_Should_Throw_Exception_When_InvalidDataIsProvided(
        string name,
        string email,
        string document
    )
    {
        var action = () => SUT.Create(name, email, document);

        action.Should().Throw<Exception>();
    }
}
