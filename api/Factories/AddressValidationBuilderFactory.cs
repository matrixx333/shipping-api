
public class AddressValidationBuilderFactory(Func<ShippingProviderType, IAddressValidationRequestBuilderFactory> factoryResolver)
{
    public IAddressValidationRequestBuilder CreateBuilderFactory(int shippingCompanyId)
    {
        if (!Enum.IsDefined(typeof(ShippingProviderType), shippingCompanyId))
        {
            throw new ArgumentException("Invalid shipping company ID");
        }

        var shippingCompanyType = (ShippingProviderType)shippingCompanyId;
        var factory = factoryResolver(shippingCompanyType);

        return factory.CreateBuilder();
    }
}