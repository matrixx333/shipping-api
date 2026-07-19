using FluentAssertions;
using NUnit.Framework;

namespace Tests.Ups;

[TestFixture]
[Category("UnitTest")]
public class UpsHttpClientTests
{
    [Test]
    public void SendRequest_ReturnsTheRequestPayloadUnchanged()
    {
        // Arrange
        var sut = new UpsHttpClient(new HttpClient());
        const string payload = "{\"XAVRequest\":[]}";

        // Act
        var result = sut.SendRequest("/addressvalidation", payload);

        // Assert
        result.Should().Be(payload);
    }
}
