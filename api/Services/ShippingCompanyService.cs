using Microsoft.EntityFrameworkCore;


class ShippingCompanyService(ShippingDb db)
{
    public async Task<List<ShippingCompany>> GetShippingCompaniesAsync()
    {
        return await db.ShippingCompanies.ToListAsync();
    }

    public async Task<ShippingCompany> GetShippingCompanyAsync(int id)
    {
        var shippingCompany = await db.ShippingCompanies.FindAsync(id);
        return shippingCompany ?? throw new Exception("Shipping Company not found.");
    }
}