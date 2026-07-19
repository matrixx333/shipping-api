using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests.FedEx;

[TestFixture]
[Category("UnitTest")]
public class FedExHttpClientFactoryTests
{
    private Mock<IHttpClientFactory> _httpClientFactory = null!;
    private FedExHttpClientFactory _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactory = new Mock<IHttpClientFactory>();
        _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient());
        _sut = new FedExHttpClientFactory(_httpClientFactory.Object);
    }

    [Test]
    public void CreateHttpClient_ReturnsFedExHttpClient()
    {
        // Act
        var result = _sut.CreateHttpClient();

        // Assert
        result.Should().BeOfType<FedExHttpClient>();
    }

    [Test]
    public void CreateHttpClient_ResolvesTheNamedFedExHttpClient()
    {
        // Act
        _sut.CreateHttpClient();

        // Assert
        _httpClientFactory.Verify(f => f.CreateClient("FedExHttpClient"), Times.Once);
    }
}
