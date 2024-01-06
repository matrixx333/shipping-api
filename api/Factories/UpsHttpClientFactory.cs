class UpsHttpClientFactory(IHttpClientFactory httpClientFactory, UpsAddressValidationRequestBuilder upsAddressValidationRequestBuilder) : IShippingHttpClientFactory
{
    public IShippingHttpClient CreateHttpClient(ShippingCompany shippingCompany)
    {
        return new UpsHttpClient
        (
            httpClientFactory.CreateClient(),
            upsAddressValidationRequestBuilder,
            shippingCompany
        );
    }
}
