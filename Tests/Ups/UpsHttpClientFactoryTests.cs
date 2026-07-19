using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests.Ups;

[TestFixture]
[Category("UnitTest")]
public class UpsHttpClientFactoryTests
{
    private Mock<IHttpClientFactory> _httpClientFactory = null!;
    private UpsHttpClientFactory _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactory = new Mock<IHttpClientFactory>();
        _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient());
        _sut = new UpsHttpClientFactory(_httpClientFactory.Object);
    }

    [Test]
    public void CreateHttpClient_ReturnsUpsHttpClient()
    {
        // Act
        var result = _sut.CreateHttpClient();

        // Assert
        result.Should().BeOfType<UpsHttpClient>();
    }

    [Test]
    public void CreateHttpClient_ResolvesTheNamedUpsHttpClient()
    {
        // Act
        _sut.CreateHttpClient();

        // Assert
        _httpClientFactory.Verify(f => f.CreateClient("UpsHttpClient"), Times.Once);
    }
}
