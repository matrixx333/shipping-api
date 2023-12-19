 public interface IAddressValidationRequestBuilder
{
    Task BuildAddresses(int addressId);
    string SerializeRequest();
}