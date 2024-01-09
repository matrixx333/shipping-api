public class UpsAddressValidationBuilderFactory() : IAddressValidationRequestBuilderFactory
{
    public IAddressValidationRequestBuilder CreateBuilder()
    {
        return new UpsAddressValidationRequestBuilder();
    }
}