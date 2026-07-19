using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;

namespace Tests.Api;

[TestFixture]
[Category("UnitTest")]
public class AddressServiceTests
{
    [Test]
    public async Task GetAddressesAsync_ReturnsAllSeededAddresses()
    {
        // Arrange
        var db = ShippingDbBuilder.New().Build();
        var sut = new AddressService(db);

        // Act
        var result = await sut.GetAddressesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Select(a => a.Id).Should().BeEquivalentTo(new[] { 1, 2 });
    }

    [Test]
    public async Task GetAddressAsync_WhenAddressExists_ReturnsTheAddress()
    {
        // Arrange
        var custom = AddressBuilder.New().WithId(42).InCity("Denver").Build();
        var db = ShippingDbBuilder.New().WithAddress(custom).Build();
        var sut = new AddressService(db);

        // Act
        var result = await sut.GetAddressAsync(42);

        // Assert
        result.Id.Should().Be(42);
        result.City.Should().Be("Denver");
    }

    [Test]
    public async Task GetAddressAsync_WhenAddressDoesNotExist_ThrowsException()
    {
        // Arrange
        var db = ShippingDbBuilder.New().Build();
        var sut = new AddressService(db);

        // Act
        Func<Task> act = () => sut.GetAddressAsync(999);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Address not found.");
    }
}
