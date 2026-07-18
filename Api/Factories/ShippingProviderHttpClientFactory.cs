public class ShippingProviderHttpClientFactory(Func<ShippingProviderType, IShippingProviderHttpClientFactory> factoryResolver) : 
    BaseFactory<IShippingProviderHttpClientFactory>(factoryResolver)
{
    public IShippingProviderHttpClient CreateHttpClient(int shippingCompanyId)
    {
        var factory = GetFactory(shippingCompanyId);
        return factory.CreateHttpClient();
    }
}