using Microsoft.EntityFrameworkCore;

class AddressService(ShippingDb db)
{
    public async Task<List<Address>> GetAddressesAsync()
    {
        return await db.Addresses.ToListAsync();
    }

    public async Task<Address> GetAddressAsync(int id)
    {
        var address = await db.Addresses.FindAsync(id);
        return address ?? throw new Exception("Address not found.");
    }
}
