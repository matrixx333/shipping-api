using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;

namespace Tests.FedEx;

[TestFixture]
[Category("UnitTest")]
public class FedExAddressValidationRequestBuilderTests
{
    private FedExAddressValidationRequestBuilder _builder = null!;

    [SetUp]
    public void SetUp() => _builder = new FedExAddressValidationRequestBuilder();

    [Test]
    public void BuildAddressRequest_MapsAllAddressFields_IntoFedExPayload()
    {
        // Arrange
        var address = AddressBuilder.New()
            .WithAddress1("777 Magnolia Blvd.")
            .WithAddress2("Unit 4")
            .InCity("Thornton")
            .InState("CO")
            .WithZipCode("80241")
            .InCountry("US")
            .Build();

        // Act
        var json = _builder.BuildAddressRequest(address).SerializeRequest();

        // Assert
        var toValidate = JsonDocument.Parse(json).RootElement
            .GetProperty("AddressesToValidate")[0];
        toValidate.GetProperty("StreetLines")[0].GetString().Should().Be("777 Magnolia Blvd.");
        toValidate.GetProperty("StreetLines")[1].GetString().Should().Be("Unit 4");
        toValidate.GetProperty("City").GetString().Should().Be("Thornton");
        toValidate.GetProperty("StateOrProvinceCode").GetString().Should().Be("CO");
        toValidate.GetProperty("PostalCode").GetString().Should().Be("80241");
        toValidate.GetProperty("CountryCode").GetString().Should().Be("US");
    }

    [Test]
    public void BuildAddressRequest_WhenAddress2IsNull_UsesEmptyStringForSecondLine()
    {
        // Arrange
        var address = AddressBuilder.New().WithAddress1("100 Main St.").WithAddress2(null).Build();

        // Act
        var json = _builder.BuildAddressRequest(address).SerializeRequest();

        // Assert
        var streetLines = JsonDocument.Parse(json).RootElement
            .GetProperty("AddressesToValidate")[0]
            .GetProperty("StreetLines");
        streetLines[0].GetString().Should().Be("100 Main St.");
        streetLines[1].GetString().Should().Be(string.Empty);
    }

    [Test]
    public void BuildAddressRequest_WhenAddress1IsNull_UsesEmptyStringForFirstLine()
    {
        // Arrange
        var address = AddressBuilder.New().WithAddress1(null).WithAddress2("Apt 2").Build();

        // Act
        var json = _builder.BuildAddressRequest(address).SerializeRequest();

        // Assert
        var streetLines = JsonDocument.Parse(json).RootElement
            .GetProperty("AddressesToValidate")[0]
            .GetProperty("StreetLines");
        streetLines[0].GetString().Should().Be(string.Empty);
        streetLines[1].GetString().Should().Be("Apt 2");
    }

    [Test]
    public void BuildAddressRequest_ReturnsSameBuilderInstance_ForFluentChaining()
    {
        // Arrange
        var address = AddressBuilder.New().Build();

        // Act
        var result = _builder.BuildAddressRequest(address);

        // Assert
        result.Should().BeSameAs(_builder);
    }

    [Test]
    public void SerializeRequest_BeforeBuildingAddress_ReturnsEmptyAddressesArray()
    {
        // Act
        var json = _builder.SerializeRequest();

        // Assert
        JsonDocument.Parse(json).RootElement
            .GetProperty("AddressesToValidate").GetArrayLength().Should().Be(0);
    }
}
