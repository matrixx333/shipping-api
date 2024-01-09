
/// <summary>
/// Represents a builder for creating address validation requests.
/// </summary>
public interface IAddressValidationRequestBuilder : ISerializableRequest
{
    /// <summary>
    /// Builds an address validation request based on the provided address.
    /// </summary>
    /// <param name="address">The address to be validated.</param>
    IAddressValidationRequestBuilder BuildAddressRequest(Address address);
}