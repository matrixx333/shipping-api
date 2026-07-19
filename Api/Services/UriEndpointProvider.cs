public class UriEndpointProvider(IConfiguration config)
{
    public string GetAddressValidationEndpoint(int shippingCompanyId)
    {
        if (!Enum.IsDefined(typeof(ShippingProviderType), shippingCompanyId))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingProviderType = (ShippingProviderType)shippingCompanyId;

        var configKey = shippingProviderType switch
        {
            ShippingProviderType.Ups => "UpsHttpClient:AddressValidationEndpoint",
            ShippingProviderType.FedEx => "FedExHttpClient:AddressValidationEndpoint",
            _ => throw new KeyNotFoundException("Shipping provider not found")
        };

        var endpoint = config[configKey];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            var message = $"No address validation endpoint is configured at '{configKey}' "
                + $"for shipping provider {shippingProviderType}.";
            throw new InvalidOperationException(message);
        }

        return endpoint;
    }
}
