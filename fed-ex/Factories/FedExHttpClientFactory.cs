public class FedExHttpClientFactory(IHttpClientFactory httpClientFactory, IAddressValidationRequestBuilder fedExAddressValidationRequestBuilder) : IShippingHttpClientFactory
{
    public IShippingHttpClient CreateHttpClient()
    {
        return new FedExHttpClient
        (
            httpClientFactory.CreateClient("FedExHttpClient"),
            fedExAddressValidationRequestBuilder
        );
    }
}
