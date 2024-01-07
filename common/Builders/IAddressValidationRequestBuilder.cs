/// <summary>
/// Represents a builder for creating address validation request objects.
/// </summary>
public interface IAddressValidationRequestBuilder
{
    /// <summary>
    /// Builds the JSON request object that the shipping company API expects.
    /// </summary>
    /// <param name="address">The address object to be included in the request.</param>
    void BuildAddressRequest(Address address);

    /// <summary>
    /// Serializes the request object into a string to be passed to the shipping company API.
    /// </summary>
    /// <returns>A string representation of the serialized request object.</returns>
    string SerializeRequest();
}