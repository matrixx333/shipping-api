public class UriEndpointProvider(IConfiguration config)
{
    public string GetAddressValidationEndpoint(int shippingCompanyId)
    {
        if (!Enum.IsDefined(typeof(ShippingProviderType), shippingCompanyId))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingProviderType = (ShippingProviderType)shippingCompanyId;

        return shippingProviderType switch
        {
            ShippingProviderType.Ups => config["UpsHttpClient:AddressValidationEndpoint"]!,
            ShippingProviderType.FedEx => config["FedExHttpClient:AddressValidationEndpoint"]!,
            _ => throw new KeyNotFoundException("Shipping provider not found")
        };
    }
}
