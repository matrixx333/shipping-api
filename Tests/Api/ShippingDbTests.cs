using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;

namespace Tests.Api;

[TestFixture]
[Category("UnitTest")]
public class ShippingDbTests
{
    [Test]
    public void OnModelCreating_SeedsTheExpectedShippingCompanies()
    {
        // Arrange
        var db = ShippingDbBuilder.New().Build();

        // Act
        var companies = db.ShippingCompanies.OrderBy(c => c.Id).ToList();

        // Assert
        companies.Select(c => c.Name).Should().Equal("UPS", "Fed Ex");
        companies.Select(c => c.AccountNumber).Should().Equal("12345", "67890");
    }

    [Test]
    public void OnModelCreating_SeedsTheExpectedAddresses()
    {
        // Arrange
        var db = ShippingDbBuilder.New().Build();

        // Act
        var addresses = db.Addresses.OrderBy(a => a.Id).ToList();

        // Assert
        addresses.Select(a => a.City).Should().Equal("Altamonte Springs", "Thornton");
        addresses.Select(a => a.ZipCode).Should().Equal("32789", "80241");
    }
}
