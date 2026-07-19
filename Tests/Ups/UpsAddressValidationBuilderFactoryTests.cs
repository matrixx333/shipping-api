using FluentAssertions;
using NUnit.Framework;

namespace Tests.Ups;

[TestFixture]
[Category("UnitTest")]
public class UpsAddressValidationBuilderFactoryTests
{
    [Test]
    public void CreateBuilder_ReturnsUpsAddressValidationRequestBuilder()
    {
        // Arrange
        var sut = new UpsAddressValidationBuilderFactory();

        // Act
        var result = sut.CreateBuilder();

        // Assert
        result.Should().BeOfType<UpsAddressValidationRequestBuilder>();
    }

    [Test]
    public void CreateBuilder_ReturnsANewInstanceOnEachCall()
    {
        // Arrange
        var sut = new UpsAddressValidationBuilderFactory();

        // Act
        var first = sut.CreateBuilder();
        var second = sut.CreateBuilder();

        // Assert
        first.Should().NotBeSameAs(second);
    }
}
