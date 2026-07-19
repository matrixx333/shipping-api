using Microsoft.EntityFrameworkCore;

namespace Tests.Builders;

/// <summary>
/// Builds an isolated in-memory <see cref="ShippingDb"/> for tests. Each build uses
/// a unique database name and calls EnsureCreated so the context's HasData seed
/// (2 shipping companies, 2 addresses) is applied. Extra rows can be layered on top.
/// </summary>
internal class ShippingDbBuilder
{
    private readonly List<Address> _extraAddresses = new();
    private readonly List<ShippingCompany> _extraCompanies = new();

    public static ShippingDbBuilder New() => new();

    public ShippingDbBuilder WithAddress(Address address)
    {
        _extraAddresses.Add(address);
        return this;
    }

    public ShippingDbBuilder WithShippingCompany(ShippingCompany company)
    {
        _extraCompanies.Add(company);
        return this;
    }

    public ShippingDb Build()
    {
        var options = new DbContextOptionsBuilder<ShippingDb>()
            .UseInMemoryDatabase($"ShippingDb-{Guid.NewGuid()}")
            .Options;

        var db = new ShippingDb(options);
        db.Database.EnsureCreated();

        if (_extraAddresses.Count > 0)
        {
            db.Addresses.AddRange(_extraAddresses);
        }

        if (_extraCompanies.Count > 0)
        {
            db.ShippingCompanies.AddRange(_extraCompanies);
        }

        if (_extraAddresses.Count > 0 || _extraCompanies.Count > 0)
        {
            db.SaveChanges();
        }

        return db;
    }
}
