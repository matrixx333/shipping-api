public class UpsAddressValidationRequestBuilder : IAddressValidationRequestBuilder, ISerializableRequest
{
    private UpsAddressValidationPayload _payload;

    public UpsAddressValidationRequestBuilder()
    {
        _payload = new UpsAddressValidationPayload
        {
            XAVRequest = []
        };
    }

    public void BuildAddressRequest(Address address)
    {
        var address1 = address.Address1 ?? string.Empty;
        var address2 = address.Address2 ?? string.Empty;
        var addressKeyFormat = new AddressKeyFormat()
        {
            AddressLine = [address1, address2],
            Region = address.State,
            PoliticalDivision2 = address.City,
            PoliticalDivision1 = address.State,
            PostcodePrimaryLow = address.ZipCode,
            CountryCode = address.CountryCode
        };        
        var xavRequest = new XAVRequest
        {
            AddressKeyFormat = addressKeyFormat
        };
        _payload.XAVRequest = [xavRequest];
    }

    public string SerializeRequest()
    {
        return SerializationHelper.SerializeRequest(_payload);
    }

    private class UpsAddressValidationPayload
    {
        public required XAVRequest[] XAVRequest { get; set; }
    }

    private class XAVRequest
    {
        public required AddressKeyFormat AddressKeyFormat { get; set; }
    }

    private class AddressKeyFormat
    {
        public string? ConsigneeName { get; set; }
        public string? BuildingName { get; set; }
        public string[]? AddressLine { get; set; }
        public string? Region { get; set; }
        public string? PoliticalDivision2 { get; set; }
        public string? PoliticalDivision1 { get; set; }
        public string? PostcodePrimaryLow { get; set; }
        public string? PostcodeExtendedLow { get; set; }
        public string? Urbanization { get; set; }
        public required string CountryCode { get; set; }
    }
}