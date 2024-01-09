class ShippingProviderHttpClientFactory(Func<ShippingProviderType, IShippingProviderHttpClientFactory> factoryResolver)
{   
    public IShippingProviderHttpClient CreateHttpClientFactory(int shippingCompanyId)
    {
        if (!Enum.IsDefined(typeof(ShippingProviderType), shippingCompanyId))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingCompanyType = (ShippingProviderType)shippingCompanyId;
        var factory = factoryResolver(shippingCompanyType);

        return factory.CreateHttpClient();
    }
}