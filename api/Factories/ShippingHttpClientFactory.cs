
class ShippingHttpClientFactory(IServiceProvider service, IHttpClientFactory httpClientFactory) : IShippingHttpClientFactory
{
    public IShippingHttpClient CreateHttpClient(ShippingCompany shippingCompany)
    {
        IShippingHttpClientFactory factory;

        if (!Enum.IsDefined(typeof(ShippingCompanyType), shippingCompany.Id))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingCompanyType = (ShippingCompanyType)shippingCompany.Id;

        switch (shippingCompanyType)
        {
            case ShippingCompanyType.Ups:
                var upsAddressValidationRequestBuilder = service.GetRequiredService<UpsAddressValidationRequestBuilder>();
                factory = new UpsHttpClientFactory(httpClientFactory, upsAddressValidationRequestBuilder);
                break;
            case ShippingCompanyType.FedEx:
                var fedExAddressValidationRequestBuilder = service.GetRequiredService<FedExAddressValidationRequestBuilder>();
                factory = new FedExHttpClientFactory(httpClientFactory, fedExAddressValidationRequestBuilder);
                break;
            default:
                throw new ArgumentException("Invalid shipping company");
        }

        return factory.CreateHttpClient(shippingCompany);
    }
}
