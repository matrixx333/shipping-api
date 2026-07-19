using Microsoft.Extensions.Configuration;

namespace Tests.Harnesses;

/// <summary>
/// Composition-root harness for <see cref="UriEndpointProvider"/>. Owns an in-memory
/// <see cref="IConfiguration"/> and exposes fluent Given… arrangement.
/// </summary>
public class UriEndpointProviderHarness
{
    private readonly Dictionary<string, string?> _configValues = new();

    public UriEndpointProviderHarness GivenUpsAddressValidationEndpoint(string endpoint)
    {
        _configValues["UpsHttpClient:AddressValidationEndpoint"] = endpoint;
        return this;
    }

    public UriEndpointProviderHarness GivenFedExAddressValidationEndpoint(string endpoint)
    {
        _configValues["FedExHttpClient:AddressValidationEndpoint"] = endpoint;
        return this;
    }

    public UriEndpointProvider Build()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(_configValues)
            .Build();

        return new UriEndpointProvider(config);
    }
}
