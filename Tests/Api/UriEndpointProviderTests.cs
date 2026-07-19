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
    public void GetAddressValidationEndpoint_ForUps_ReturnsConfiguredEndpoint()
    {
        // Arrange
        var sut = _harness
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
        // Arrange — both endpoints are configured so the assertion proves FedEx
        // resolves to its own key rather than falling through to the UPS one.
        var sut = _harness
            .GivenUpsAddressValidationEndpoint("https://ups.example/av")
            .GivenFedExAddressValidationEndpoint("https://fedex.example/av")
            .Build();

        // Act
        var result = sut.GetAddressValidationEndpoint((int)ShippingProviderType.FedEx);

        // Assert
        result.Should().Be("https://fedex.example/av");
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
