using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests.Api;

/// <summary>
/// Exercises <see cref="AddressValidationBuilderFactory"/> and, through it, the
/// shared <see cref="BaseFactory{TFactory}"/> dispatch/validation logic.
/// </summary>
[TestFixture]
[Category("UnitTest")]
public class AddressValidationBuilderFactoryTests
{
    [Test]
    public void CreateBuilder_WithValidUpsId_ReturnsBuilderFromTheResolvedFactory()
    {
        // Arrange
        var expectedBuilder = Mock.Of<IAddressValidationRequestBuilder>();
        var providerFactory = new Mock<IAddressValidationRequestBuilderFactory>();
        providerFactory.Setup(f => f.CreateBuilder()).Returns(expectedBuilder);

        ShippingProviderType? resolvedKey = null;
        var sut = new AddressValidationBuilderFactory(key =>
        {
            resolvedKey = key;
            return providerFactory.Object;
        });

        // Act
        var result = sut.CreateBuilder((int)ShippingProviderType.Ups);

        // Assert
        result.Should().BeSameAs(expectedBuilder);
        resolvedKey.Should().Be(ShippingProviderType.Ups);
    }

    [Test]
    public void CreateBuilder_WithValidFedExId_ResolvesTheFedExProviderType()
    {
        // Arrange
        ShippingProviderType? resolvedKey = null;
        var sut = new AddressValidationBuilderFactory(key =>
        {
            resolvedKey = key;
            return Mock.Of<IAddressValidationRequestBuilderFactory>();
        });

        // Act
        sut.CreateBuilder((int)ShippingProviderType.FedEx);

        // Assert
        resolvedKey.Should().Be(ShippingProviderType.FedEx);
    }

    [Test]
    public void CreateBuilder_WithUndefinedShippingCompanyId_ThrowsArgumentException()
    {
        // Arrange
        var sut = new AddressValidationBuilderFactory(
            _ => Mock.Of<IAddressValidationRequestBuilderFactory>());

        // Act
        Action act = () => sut.CreateBuilder(0);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid shipping company ID");
    }
}
