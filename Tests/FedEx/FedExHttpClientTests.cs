using FluentAssertions;
using NUnit.Framework;

namespace Tests.FedEx;

[TestFixture]
[Category("UnitTest")]
public class FedExHttpClientTests
{
    [Test]
    public void SendRequest_ReturnsTheRequestPayloadUnchanged()
    {
        // Arrange
        var sut = new FedExHttpClient(new HttpClient());
        const string payload = "{\"AddressesToValidate\":[]}";

        // Act
        var result = sut.SendRequest("/address/v1/addresses/resolve", payload);

        // Assert
        result.Should().Be(payload);
    }
}
