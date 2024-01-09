public class FedExAddressValidationBuilderFactory() : IAddressValidationRequestBuilderFactory
{
    public IAddressValidationRequestBuilder CreateBuilder()
    {
        return new FedExAddressValidationRequestBuilder();
    }
}