/// <summary>
/// Represents a builder for creating address validation request objects.
/// </summary>
public interface IAddressValidationRequestBuilderFactory
{
    /// <summary>
    /// Builds the JSON request object that the shipping company API expects.
    /// </summary>
    /// <param name="address">The address object to be included in the request.</param>
    IAddressValidationRequestBuilder CreateBuilder();
}