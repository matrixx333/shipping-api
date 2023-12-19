using Microsoft.EntityFrameworkCore;

class ShippingDb : DbContext
{
    public ShippingDb(DbContextOptions<ShippingDb> options) : base(options)
    {
        
    }

    public DbSet<ShippingCompany> ShippingCompanies => Set<ShippingCompany>();
    public DbSet<Address> Addresses => Set<Address>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ShippingCompany>()
            .HasData(
            new ShippingCompany { Id = 1, Name = "UPS", AccountNumber="12345", AccountKey="fda5432fda"},
            new ShippingCompany { Id = 2, Name = "Fed Ex", AccountNumber="67890", AccountKey="req987reqw"}
            );
        modelBuilder.Entity<Address>()
            .HasData(
            new Address { Id = 1, Address1 = "555 Somewhere St.", City = "Altamonte Springs", State = "FL", ZipCode = "32789", CountryCode = "US" },
            new Address { Id = 2, Address1 = "777 Magnolia Blvd.", City = "Thornton", State = "CO", ZipCode = "80241", CountryCode = "US" }
            );
    }
}
