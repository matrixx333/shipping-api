public abstract class BaseFactory<TFactory>(Func<ShippingProviderType, TFactory> factoryResolver)
{
    protected TFactory GetFactory(int shippingCompanyId)
    {
        if (!Enum.IsDefined(typeof(ShippingProviderType), shippingCompanyId))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingCompanyType = (ShippingProviderType)shippingCompanyId;
        return factoryResolver(shippingCompanyType);
    }
}