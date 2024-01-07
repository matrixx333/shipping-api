/// <summary>
/// Represents an HTTP client for interacting with a shipping company API.
/// </summary>
public interface IShippingHttpClient
{
    /// <summary>
    /// Validates the address stored in the database against the selected shipping company API. 
    /// </summary>
    /// <param name="addressId">The ID of the address that is stored in the database.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validation result as a string.</returns>
    Task<string> ValidateAddress(Address address);
}
