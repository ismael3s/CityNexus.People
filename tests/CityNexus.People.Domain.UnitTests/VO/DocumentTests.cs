using CityNexus.People.Domain.VO;
using FluentAssertions;

namespace CityNexus.People.Domain.UnitTests.VO;

public sealed class DocumentTests
{
    [Theory]
    [Trait("Document - Unit", "When Invalid document data is provided, should throws Exceptions")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("123.456.789-20")]
    [InlineData("023.412.789-20")]
    public void Document_Create_WhenInvalidDataIsPassed_ShouldThrowException(string value)
    {
        var action = () => Document.Create(value);  
        
        action.Should().Throw<Exception>().WithMessage("The CPF is invalid");
    }
}