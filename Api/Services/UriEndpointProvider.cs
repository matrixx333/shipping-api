using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class UriEndpointProvider(IConfiguration config, IHostEnvironment env)
{
    public string GetAddressValidationEndpoint(int shippingCompanyId)
    {
        if (!Enum.IsDefined(typeof(ShippingProviderType), shippingCompanyId))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingProviderType = (ShippingProviderType)shippingCompanyId;

        if (env.IsDevelopment())
        {
            config.GetSection("UpsHttpClient");
            config.GetSection("FedExHttpClient");
        }

        var upsAddressValidationEndpoint = config["UpsHttpClient:AddressValidationEndpoint"]!;
        var fedExAddressValidationEndpoint = config["UpsHttpClient:AddressValidationEndpoint"]!;

        return shippingProviderType switch
        {
            ShippingProviderType.Ups => upsAddressValidationEndpoint,
            ShippingProviderType.FedEx => fedExAddressValidationEndpoint,
            _ => throw new KeyNotFoundException("Shipping provider not found")
        };
    }
}
