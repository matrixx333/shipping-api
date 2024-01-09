/// <summary>
/// Represents a factory for creating instances of <see cref="IShippingProviderHttpClient"/>.
/// </summary>
public interface IShippingProviderHttpClientFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IShippingProviderHttpClient"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IShippingProviderHttpClient"/>.</returns>
    public IShippingProviderHttpClient CreateHttpClient();
}
