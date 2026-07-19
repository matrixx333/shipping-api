using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Tests.Api;

[TestFixture]
[Category("UnitTest")]
public class ApplicationServiceExtensionsTests
{
    [Test]
    public void AddServices_RegistersTheApplicationServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddServices();

        // Assert
        services.Should().Contain(d => d.ServiceType == typeof(AddressService));
        services.Should().Contain(d => d.ServiceType == typeof(ShippingCompanyService));
        services.Should().Contain(d => d.ServiceType == typeof(UriEndpointProvider));
    }

    [Test]
    public void AddFactories_And_AddFactoryResolvers_RegisterResolvableFactories()
    {
        // Arrange
        using var provider = BuildFullProvider();
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;

        // Act / Assert
        sp.GetRequiredService<UpsHttpClientFactory>().Should().NotBeNull();
        sp.GetRequiredService<FedExHttpClientFactory>().Should().NotBeNull();
        sp.GetRequiredService<UpsAddressValidationBuilderFactory>().Should().NotBeNull();
        sp.GetRequiredService<FedExAddressValidationBuilderFactory>().Should().NotBeNull();
        sp.GetRequiredService<ShippingProviderHttpClientFactory>().Should().NotBeNull();
        sp.GetRequiredService<AddressValidationBuilderFactory>().Should().NotBeNull();
    }

    [Test]
    public void AddressValidationBuilderResolver_MapsKnownProviders_AndThrowsForUnknown()
    {
        // Arrange
        using var provider = BuildFullProvider();
        using var scope = provider.CreateScope();
        var resolver = scope.ServiceProvider
            .GetRequiredService
            <
                Func<ShippingProviderType, IAddressValidationRequestBuilderFactory>
            >();

        // Act / Assert
        resolver(ShippingProviderType.Ups).Should().BeOfType<UpsAddressValidationBuilderFactory>();
        resolver(ShippingProviderType.FedEx).Should()
            .BeOfType<FedExAddressValidationBuilderFactory>();

        Action unknown = () => resolver((ShippingProviderType)999);
        unknown.Should().Throw<KeyNotFoundException>();
    }

    [Test]
    public void HttpClientFactoryResolver_MapsKnownProviders_AndThrowsForUnknown()
    {
        // Arrange
        using var provider = BuildFullProvider();
        using var scope = provider.CreateScope();
        var resolver = scope.ServiceProvider
            .GetRequiredService<Func<ShippingProviderType, IShippingProviderHttpClientFactory>>();

        // Act / Assert
        resolver(ShippingProviderType.Ups).Should().BeOfType<UpsHttpClientFactory>();
        resolver(ShippingProviderType.FedEx).Should().BeOfType<FedExHttpClientFactory>();

        Action unknown = () => resolver((ShippingProviderType)999);
        unknown.Should().Throw<KeyNotFoundException>();
    }

    [Test]
    public void AddUpsHttpClient_WithBaseAddress_ConfiguresTheNamedClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["UpsHttpClient:BaseAddress"] = "https://ups.example/",
            ["UpsHttpClient:ApiKey"] = "ups-key"
        });

        // Act
        services.AddUpsHttpClient(config);

        // Assert
        var client = CreateNamedClient(services, "UpsHttpClient");
        client.BaseAddress.Should().Be(new Uri("https://ups.example/"));
        client.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
        client.DefaultRequestHeaders.Authorization.Parameter.Should().Be("ups-key");
    }

    [Test]
    public void AddUpsHttpClient_WithoutBaseAddress_LeavesBaseAddressNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildConfig(new Dictionary<string, string?>());

        // Act
        services.AddUpsHttpClient(config);

        // Assert
        var client = CreateNamedClient(services, "UpsHttpClient");
        client.BaseAddress.Should().BeNull();
    }

    [Test]
    public void AddFedExHttpClient_WithBaseAddress_ConfiguresTheNamedClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["FedExHttpClient:BaseAddress"] = "https://fedex.example/",
            ["FedExHttpClient:ApiKey"] = "fedex-key"
        });

        // Act
        services.AddFedExHttpClient(config);

        // Assert
        var client = CreateNamedClient(services, "FedExHttpClient");
        client.BaseAddress.Should().Be(new Uri("https://fedex.example/"));
        client.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
        client.DefaultRequestHeaders.Authorization.Parameter.Should().Be("fedex-key");
    }

    [Test]
    public void AddFedExHttpClient_WithoutBaseAddress_LeavesBaseAddressNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildConfig(new Dictionary<string, string?>());

        // Act
        services.AddFedExHttpClient(config);

        // Assert
        var client = CreateNamedClient(services, "FedExHttpClient");
        client.BaseAddress.Should().BeNull();
    }

    [Test]
    public void ProviderSettings_AreKeyedByProvider_AndDoNotShadowEachOther()
    {
        // Arrange — registering both providers must not let one provider's section
        // overwrite the other's, which is what unnamed options would do.
        var services = new ServiceCollection();
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["UpsHttpClient:BaseAddress"] = "https://ups.example/",
            ["UpsHttpClient:ApiKey"] = "ups-key",
            ["UpsHttpClient:AddressValidationEndpoint"] = "/ups/av",
            ["FedExHttpClient:BaseAddress"] = "https://fedex.example/",
            ["FedExHttpClient:ApiKey"] = "fedex-key",
            ["FedExHttpClient:AddressValidationEndpoint"] = "/fedex/av"
        });

        // Act
        services.AddUpsHttpClient(config);
        services.AddFedExHttpClient(config);

        using var provider = services.BuildServiceProvider();
        var monitor = provider
            .GetRequiredService<IOptionsMonitor<ShippingProviderHttpClientSettings>>();

        var ups = monitor.Get(ShippingProviderType.Ups.ToString());
        var fedEx = monitor.Get(ShippingProviderType.FedEx.ToString());

        // Assert
        ups.BaseAddress.Should().Be("https://ups.example/");
        ups.ApiKey.Should().Be("ups-key");
        ups.AddressValidationEndpoint.Should().Be("/ups/av");

        fedEx.BaseAddress.Should().Be("https://fedex.example/");
        fedEx.ApiKey.Should().Be("fedex-key");
        fedEx.AddressValidationEndpoint.Should().Be("/fedex/av");
    }

    private static ServiceProvider BuildFullProvider()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddServices();
        services.AddFactories();
        services.AddFactoryResolvers();
        return services.BuildServiceProvider();
    }

    private static HttpClient CreateNamedClient(IServiceCollection services, string name)
    {
        using var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
    }

    private static IConfiguration BuildConfig(Dictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();
}
