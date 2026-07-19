using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests.Api;

/// <summary>
/// Exercises <see cref="ShippingProviderHttpClientFactory"/> and, through it, the
/// shared <see cref="BaseFactory{TFactory}"/> dispatch/validation logic for the
/// second closed generic instantiation.
/// </summary>
[TestFixture]
[Category("UnitTest")]
public class ShippingProviderHttpClientFactoryTests
{
    [Test]
    public void CreateHttpClient_WithValidUpsId_ReturnsClientFromTheResolvedFactory()
    {
        // Arrange
        var expectedClient = Mock.Of<IShippingProviderHttpClient>();
        var providerFactory = new Mock<IShippingProviderHttpClientFactory>();
        providerFactory.Setup(f => f.CreateHttpClient()).Returns(expectedClient);

        ShippingProviderType? resolvedKey = null;
        var sut = new ShippingProviderHttpClientFactory(key =>
        {
            resolvedKey = key;
            return providerFactory.Object;
        });

        // Act
        var result = sut.CreateHttpClient((int)ShippingProviderType.Ups);

        // Assert
        result.Should().BeSameAs(expectedClient);
        resolvedKey.Should().Be(ShippingProviderType.Ups);
    }

    [Test]
    public void CreateHttpClient_WithValidFedExId_ResolvesTheFedExProviderType()
    {
        // Arrange
        ShippingProviderType? resolvedKey = null;
        var sut = new ShippingProviderHttpClientFactory(key =>
        {
            resolvedKey = key;
            return Mock.Of<IShippingProviderHttpClientFactory>();
        });

        // Act
        sut.CreateHttpClient((int)ShippingProviderType.FedEx);

        // Assert
        resolvedKey.Should().Be(ShippingProviderType.FedEx);
    }

    [Test]
    public void CreateHttpClient_WithUndefinedShippingCompanyId_ThrowsArgumentException()
    {
        // Arrange
        var sut = new ShippingProviderHttpClientFactory(
            _ => Mock.Of<IShippingProviderHttpClientFactory>());

        // Act
        Action act = () => sut.CreateHttpClient(99);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid shipping company ID");
    }
}
