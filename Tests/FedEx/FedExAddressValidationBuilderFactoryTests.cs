using FluentAssertions;
using NUnit.Framework;

namespace Tests.FedEx;

[TestFixture]
[Category("UnitTest")]
public class FedExAddressValidationBuilderFactoryTests
{
    [Test]
    public void CreateBuilder_ReturnsFedExAddressValidationRequestBuilder()
    {
        // Arrange
        var sut = new FedExAddressValidationBuilderFactory();

        // Act
        var result = sut.CreateBuilder();

        // Assert
        result.Should().BeOfType<FedExAddressValidationRequestBuilder>();
    }

    [Test]
    public void CreateBuilder_ReturnsANewInstanceOnEachCall()
    {
        // Arrange
        var sut = new FedExAddressValidationBuilderFactory();

        // Act
        var first = sut.CreateBuilder();
        var second = sut.CreateBuilder();

        // Assert
        first.Should().NotBeSameAs(second);
    }
}
