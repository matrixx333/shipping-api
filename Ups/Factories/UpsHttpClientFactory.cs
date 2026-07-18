public class UpsHttpClientFactory(IHttpClientFactory httpClientFactory) : IShippingProviderHttpClientFactory
{
    public IShippingProviderHttpClient CreateHttpClient()
    {
        return new UpsHttpClient
        (
            httpClientFactory.CreateClient("UpsHttpClient")
        );
    }
}
