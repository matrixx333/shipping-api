using FluentAssertions;
using NUnit.Framework;
using Tests.Harnesses;

namespace Tests.Api;

[TestFixture]
[Category("UnitTest")]
public class UriEndpointProviderTests
{
    private UriEndpointProviderHarness _harness = null!;

    [SetUp]
    public void SetUp() => _harness = new UriEndpointProviderHarness();

    [Test]
    public void GetAddressValidationEndpoint_ForUpsInDevelopment_ReturnsConfiguredEndpoint()
    {
        // Arrange
        var sut = _harness
            .GivenDevelopment()
            .GivenUpsAddressValidationEndpoint("https://ups.example/av")
            .Build();

        // Act
        var result = sut.GetAddressValidationEndpoint((int)ShippingProviderType.Ups);

        // Assert
        result.Should().Be("https://ups.example/av");
    }

    [Test]
    public void GetAddressValidationEndpoint_ForFedEx_ReturnsConfiguredEndpoint()
    {
        // Arrange
        // NOTE: the production code reads the UpsHttpClient endpoint key for both
        // providers, so FedEx resolves to the same configured value.
        var sut = _harness
            .GivenDevelopment()
            .GivenUpsAddressValidationEndpoint("https://ups.example/av")
            .Build();

        // Act
        var result = sut.GetAddressValidationEndpoint((int)ShippingProviderType.FedEx);

        // Assert
        result.Should().Be("https://ups.example/av");
    }

    [Test]
    public void GetAddressValidationEndpoint_InProduction_ReturnsConfiguredEndpoint()
    {
        // Arrange — exercises the non-Development branch.
        var sut = _harness
            .GivenProduction()
            .GivenUpsAddressValidationEndpoint("https://ups.prod/av")
            .Build();

        // Act
        var result = sut.GetAddressValidationEndpoint((int)ShippingProviderType.Ups);

        // Assert
        result.Should().Be("https://ups.prod/av");
    }

    [Test]
    public void GetAddressValidationEndpoint_WithUndefinedId_ThrowsArgumentException()
    {
        // Arrange
        var sut = _harness.Build();

        // Act
        Action act = () => sut.GetAddressValidationEndpoint(0);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid shipping company ID");
    }
}
