class ShippingHttpClientFactory(Func<ShippingCompanyType, IShippingHttpClientFactory> factoryResolver)
{   
    public IShippingHttpClient CreateHttpClient(int shippingCompanyId)
    {
        if (!Enum.IsDefined(typeof(ShippingCompanyType), shippingCompanyId))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingCompanyType = (ShippingCompanyType)shippingCompanyId;
        var factory = factoryResolver(shippingCompanyType);

        return factory.CreateHttpClient();
    }
}