public class UpsHttpClientFactory(IHttpClientFactory httpClientFactory, IAddressValidationRequestBuilder upsAddressValidationRequestBuilder) : IShippingHttpClientFactory
{
    public IShippingHttpClient CreateHttpClient()
    {
        return new UpsHttpClient
        (
            httpClientFactory.CreateClient("UpsHttpClient"),
            (UpsAddressValidationRequestBuilder)upsAddressValidationRequestBuilder
        );
    }
}
