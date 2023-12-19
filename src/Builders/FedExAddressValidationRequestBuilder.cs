using System.Runtime.CompilerServices;
using System.Text.Json;

class FedExAddressValidationRequestBuilder : IAddressValidationRequestBuilder
{
    private readonly AddressService _addressService;
    private FedExAddressValidationPayload _payload;
    public FedExAddressValidationRequestBuilder(AddressService addressService)
    {
        _addressService = addressService;
        _payload = new FedExAddressValidationPayload
        {
            AddressesToValidate = []
        };
    }
   
    public async Task BuildAddresses(int addressId)
    {
        var address = await _addressService.GetAddressAsync(addressId);
        var address1 = address.Address1 ?? string.Empty;
        var address2 = address.Address2 ?? string.Empty;
        var addressToValidate = new Address()
        {
            StreetLines = [address1, address2],
            StateOrProvinceCode = address.State,
            City = address.City,
            PostalCode = address.ZipCode,
            CountryCode = address.CountryCode
        };        
        _payload.AddressesToValidate = [addressToValidate];
    }

    public string SerializeRequest()
    {
        if (_payload == null) return string.Empty;
        return JsonSerializer.Serialize(_payload);
    }

    private class FedExAddressValidationPayload
    {
        public required Address[] AddressesToValidate { get; set; }
    }

    private class Address
    {
        public required string[] StreetLines { get; set; }
        public required string City { get; set; }
        public required string StateOrProvinceCode { get; set; }
        public required string PostalCode { get; set; }
        public string? CountryCode { get; set; }
        Boolean Residential { get; set; }
    }
}