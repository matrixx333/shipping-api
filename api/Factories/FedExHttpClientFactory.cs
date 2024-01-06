class FedExHttpClientFactory(IHttpClientFactory httpClientFactory, FedExAddressValidationRequestBuilder fedExAddressValidationRequestBuilder) : IShippingHttpClientFactory
{
    public IShippingHttpClient CreateHttpClient(ShippingCompany shippingCompany)
    {
        return new FedExHttpClient
        (
            httpClientFactory.CreateClient(),
            fedExAddressValidationRequestBuilder,
            shippingCompany
        );
    }
}
