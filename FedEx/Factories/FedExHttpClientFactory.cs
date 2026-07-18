public class FedExHttpClientFactory(IHttpClientFactory httpClientFactory) : IShippingProviderHttpClientFactory
{
    public IShippingProviderHttpClient CreateHttpClient()
    {
        return new FedExHttpClient
        (
            httpClientFactory.CreateClient("FedExHttpClient")
        );
    }
}
