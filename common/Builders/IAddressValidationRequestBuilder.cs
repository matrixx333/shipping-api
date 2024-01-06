public interface IAddressValidationRequestBuilder
{
    /// <summary>
    /// Builds the JSON request object that the shipping company API expects
    /// </summary>
    /// <param name="addressId">The ID of the address in the database</param>
    /// <returns></returns>
    void BuildAddressRequest(Address address);
    /// <summary>
    /// Serializes the request object into a string to be passed to the shipping company API 
    /// </summary>
    /// <returns></returns>
    string SerializeRequest();
}