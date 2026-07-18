public class AddressValidationBuilderFactory(Func<ShippingProviderType, IAddressValidationRequestBuilderFactory> factoryResolver) : 
    BaseFactory<IAddressValidationRequestBuilderFactory>(factoryResolver)
{
    public IAddressValidationRequestBuilder CreateBuilder(int shippingCompanyId)
    {
        var factory = GetFactory(shippingCompanyId);
        return factory.CreateBuilder();
    }
}