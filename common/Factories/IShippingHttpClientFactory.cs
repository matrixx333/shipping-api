/// <summary>
/// Represents a factory for creating instances of <see cref="IShippingHttpClient"/>.
/// </summary>
public interface IShippingHttpClientFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IShippingHttpClient"/>.
    /// </summary>
    /// <returns>The created <see cref="IShippingHttpClient"/> instance.</returns>
    IShippingHttpClient CreateHttpClient();
}
