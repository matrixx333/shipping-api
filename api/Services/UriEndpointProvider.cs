using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class UriEndpointProvider(IConfiguration config)
{
    public string GetAddressValidationEndpoint(int shippingCompanyId)
    {
        if (!Enum.IsDefined(typeof(ShippingProviderType), shippingCompanyId))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingProviderType = (ShippingProviderType)shippingCompanyId;
        var fedExClientSettings = config.GetSection("FedExHttpClient").Get<ShippingProviderHttpClientSettings>();            
        var upsClientSettings = config.GetSection("UpsHttpClient").Get<ShippingProviderHttpClientSettings>();            

        return shippingProviderType switch
        {
            ShippingProviderType.Ups => $"{upsClientSettings!.AddressValidationEndpoint}",
            ShippingProviderType.FedEx => $"{fedExClientSettings!.AddressValidationEndpoint}",
            _ => throw new KeyNotFoundException("Shipping provider not found")
        };
    }
}
