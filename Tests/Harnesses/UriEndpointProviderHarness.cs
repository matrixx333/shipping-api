using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Tests.Harnesses;

/// <summary>
/// Composition-root harness for <see cref="UriEndpointProvider"/>. Owns the mocked
/// <see cref="IHostEnvironment"/> and an in-memory <see cref="IConfiguration"/>, and
/// exposes fluent Given… arrangement. Defaults to the Development environment.
/// </summary>
public class UriEndpointProviderHarness
{
    private readonly Dictionary<string, string?> _configValues = new();

    public Mock<IHostEnvironment> Environment { get; } = new();

    public UriEndpointProviderHarness()
    {
        Environment.SetupAllProperties();
        Environment.Object.EnvironmentName = Environments.Development;
    }

    public UriEndpointProviderHarness GivenEnvironment(string environmentName)
    {
        Environment.Object.EnvironmentName = environmentName;
        return this;
    }

    public UriEndpointProviderHarness GivenDevelopment() =>
        GivenEnvironment(Environments.Development);

    public UriEndpointProviderHarness GivenProduction() =>
        GivenEnvironment(Environments.Production);

    public UriEndpointProviderHarness GivenUpsAddressValidationEndpoint(string endpoint)
    {
        _configValues["UpsHttpClient:AddressValidationEndpoint"] = endpoint;
        return this;
    }

    public UriEndpointProvider Build()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(_configValues)
            .Build();

        return new UriEndpointProvider(config, Environment.Object);
    }
}
