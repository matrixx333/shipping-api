using Microsoft.EntityFrameworkCore;


class ShippingCompanyService(ShippingDb db)
{
    private readonly ShippingDb _db = db;

    public async Task<List<ShippingCompany>> GetShippingCompaniesAsync()
    {
        return await _db.ShippingCompanies.ToListAsync();
    }

    public async Task<ShippingCompany> GetShippingCompanyAsync(int id)
    {
        var shippingCompany = await _db.ShippingCompanies.FindAsync(id);
        return shippingCompany ?? throw new Exception("Shipping Company not found.");
    }
}