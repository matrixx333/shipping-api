using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;

namespace Tests.Api;

[TestFixture]
[Category("UnitTest")]
public class ShippingCompanyServiceTests
{
    [Test]
    public async Task GetShippingCompaniesAsync_ReturnsAllSeededShippingCompanies()
    {
        // Arrange
        var db = ShippingDbBuilder.New().Build();
        var sut = new ShippingCompanyService(db);

        // Act
        var result = await sut.GetShippingCompaniesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Select(c => c.Name).Should().BeEquivalentTo(new[] { "UPS", "Fed Ex" });
    }

    [Test]
    public async Task GetShippingCompanyAsync_WhenCompanyExists_ReturnsTheCompany()
    {
        // Arrange
        var custom = ShippingCompanyBuilder.New().WithId(7).Named("DHL").Build();
        var db = ShippingDbBuilder.New().WithShippingCompany(custom).Build();
        var sut = new ShippingCompanyService(db);

        // Act
        var result = await sut.GetShippingCompanyAsync(7);

        // Assert
        result.Id.Should().Be(7);
        result.Name.Should().Be("DHL");
    }

    [Test]
    public async Task GetShippingCompanyAsync_WhenCompanyDoesNotExist_ThrowsException()
    {
        // Arrange
        var db = ShippingDbBuilder.New().Build();
        var sut = new ShippingCompanyService(db);

        // Act
        Func<Task> act = () => sut.GetShippingCompanyAsync(999);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Shipping Company not found.");
    }
}
