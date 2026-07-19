using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;

namespace Tests.Ups;

[TestFixture]
[Category("UnitTest")]
public class UpsAddressValidationRequestBuilderTests
{
    private UpsAddressValidationRequestBuilder _builder = null!;

    [SetUp]
    public void SetUp() => _builder = new UpsAddressValidationRequestBuilder();

    [Test]
    public void BuildAddressRequest_MapsAllAddressFields_IntoUpsPayload()
    {
        // Arrange
        var address = AddressBuilder.New()
            .WithAddress1("555 Somewhere St.")
            .WithAddress2("Suite 100")
            .InCity("Altamonte Springs")
            .InState("FL")
            .WithZipCode("32789")
            .InCountry("US")
            .Build();

        // Act
        var json = _builder.BuildAddressRequest(address).SerializeRequest();

        // Assert
        var keyFormat = JsonDocument.Parse(json).RootElement
            .GetProperty("XAVRequest")[0]
            .GetProperty("AddressKeyFormat");
        keyFormat.GetProperty("AddressLine")[0].GetString().Should().Be("555 Somewhere St.");
        keyFormat.GetProperty("AddressLine")[1].GetString().Should().Be("Suite 100");
        keyFormat.GetProperty("Region").GetString().Should().Be("FL");
        keyFormat.GetProperty("PoliticalDivision2").GetString().Should().Be("Altamonte Springs");
        keyFormat.GetProperty("PoliticalDivision1").GetString().Should().Be("FL");
        keyFormat.GetProperty("PostcodePrimaryLow").GetString().Should().Be("32789");
        keyFormat.GetProperty("CountryCode").GetString().Should().Be("US");
    }

    [Test]
    public void BuildAddressRequest_WhenAddress2IsNull_UsesEmptyStringForSecondLine()
    {
        // Arrange
        var address = AddressBuilder.New().WithAddress1("100 Main St.").WithAddress2(null).Build();

        // Act
        var json = _builder.BuildAddressRequest(address).SerializeRequest();

        // Assert
        var addressLine = JsonDocument.Parse(json).RootElement
            .GetProperty("XAVRequest")[0]
            .GetProperty("AddressKeyFormat")
            .GetProperty("AddressLine");
        addressLine[0].GetString().Should().Be("100 Main St.");
        addressLine[1].GetString().Should().Be(string.Empty);
    }

    [Test]
    public void BuildAddressRequest_WhenAddress1IsNull_UsesEmptyStringForFirstLine()
    {
        // Arrange
        var address = AddressBuilder.New().WithAddress1(null).WithAddress2("Apt 2").Build();

        // Act
        var json = _builder.BuildAddressRequest(address).SerializeRequest();

        // Assert
        var addressLine = JsonDocument.Parse(json).RootElement
            .GetProperty("XAVRequest")[0]
            .GetProperty("AddressKeyFormat")
            .GetProperty("AddressLine");
        addressLine[0].GetString().Should().Be(string.Empty);
        addressLine[1].GetString().Should().Be("Apt 2");
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
    public void SerializeRequest_BeforeBuildingAddress_ReturnsEmptyRequestArray()
    {
        // Act
        var json = _builder.SerializeRequest();

        // Assert
        JsonDocument.Parse(json).RootElement
            .GetProperty("XAVRequest").GetArrayLength().Should().Be(0);
    }
}
