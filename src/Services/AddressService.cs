using Microsoft.EntityFrameworkCore;

class AddressService(ShippingDb db)
{
    private readonly ShippingDb _db = db;

    public async Task<List<Address>> GetAddressesAsync()
    {
        return await _db.Addresses.ToListAsync();
    }

    public async Task<Address> GetAddressAsync(int id)
    {
        var address = await _db.Addresses.FindAsync(id);
        return address ?? throw new Exception("Address not found.");
    }
}
