
/// <summary>
/// Represents an interface for an HTTP client used to communicate with a shipping provider.
/// </summary>
public interface IShippingProviderHttpClient
{
    /// <summary>
    /// Sends a request to the shipping provider with the specified URL suffix and request payload.
    /// </summary>
    /// <param name="urlSuffix">The URL suffix to append to the base URL of the shipping provider.</param>
    /// <param name="requestPayload">The payload of the request.</param>
    /// <returns>The response from the shipping provider.</returns>
    string SendRequest(string urlSuffix, string requestPayload);
}
