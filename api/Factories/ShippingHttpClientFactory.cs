class ShippingHttpClientFactory(Func<ShippingCompanyType, IShippingHttpClientFactory> factoryResolver)
{   
    public IShippingHttpClient CreateHttpClient(ShippingCompany shippingCompany)
    {
        if (!Enum.IsDefined(typeof(ShippingCompanyType), shippingCompany.Id))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingCompanyType = (ShippingCompanyType)shippingCompany.Id;
        var factory = factoryResolver(shippingCompanyType);

        return factory.CreateHttpClient(shippingCompany);
    }
}