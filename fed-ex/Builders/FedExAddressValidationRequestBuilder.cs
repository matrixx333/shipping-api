public class FedExAddressValidationRequestBuilder : IAddressValidationRequestBuilder, ISerializableRequest
{
    private FedExAddressValidationPayload _payload;
    public FedExAddressValidationRequestBuilder()
    {
        _payload = new FedExAddressValidationPayload
        {
            AddressesToValidate = []
        };
    }
   
    public void BuildAddressRequest(global::Address address)
    {
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
        return SerializationHelper.SerializeRequest(_payload);
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