
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

static class ApplicationServiceExtensions
{
    public static void AddUpsHttpClient(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.Configure<ShippingProviderHttpClientSettings>(config.GetSection("UpsHttpClient"));
        }

        var baseAddress = config["UpsHttpClient:BaseAddress"];
        var apiKey = config["UpsHttpClient:ApiKey"];

        services.AddHttpClient<UpsHttpClient>(client =>
        {
            client.BaseAddress = baseAddress != null ? new Uri(baseAddress) : null;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Add("X-Locale", "en_US");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        });
    }

    public static void AddFedExHttpClient(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.Configure<ShippingProviderHttpClientSettings>(config.GetSection("UpsHttpClient"));
        }

        var baseAddress = config["FedExHttpClient:BaseAddress"];
        var apiKey = config["FedExHttpClient:ApiKey"];

        services.AddHttpClient<FedExHttpClient>(client =>
        {
            client.BaseAddress = baseAddress != null ? new Uri(baseAddress) : null;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Add("X-Locale", "en_US");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        });
    }

    public static void AddBuilders(this IServiceCollection services)
    {
        services.AddScoped<UpsAddressValidationRequestBuilder>();
        services.AddScoped<FedExAddressValidationRequestBuilder>();      
    }

    public static void AddFactories(this IServiceCollection services)
    {
        services.AddScoped<ShippingProviderHttpClientFactory>();
        
        services.AddScoped(sp =>
        {
            return new UpsHttpClientFactory(sp.GetRequiredService<IHttpClientFactory>());
        });

        services.AddScoped(sp =>
        {
            return new FedExHttpClientFactory(sp.GetRequiredService<IHttpClientFactory>());
        });

        services.AddScoped<AddressValidationBuilderFactory>();

        services.AddScoped(sp =>
        {
            return new UpsAddressValidationBuilderFactory();
        });

        services.AddScoped(sp =>
        {
            return new FedExAddressValidationBuilderFactory();
        });
    }

    public static void AddFactoryResolvers(this IServiceCollection services)
    {
        services.AddTransient<Func<ShippingProviderType, IAddressValidationRequestBuilderFactory>>(sp =>
        {
            var factories = new Dictionary<ShippingProviderType, IAddressValidationRequestBuilderFactory>
            {
                { ShippingProviderType.Ups, sp.GetRequiredService<UpsAddressValidationBuilderFactory>() },
                { ShippingProviderType.FedEx, sp.GetRequiredService<FedExAddressValidationBuilderFactory>() }
            };

            return key =>
            {
                if (factories.TryGetValue(key, out var factory))
                {
                    return factory;
                }

                throw new KeyNotFoundException($"No address validation request builder found for shipping company type: {key}");
            };
        });

        services.AddTransient<Func<ShippingProviderType, IShippingProviderHttpClientFactory>>(sp =>
        {
            var factories = new Dictionary<ShippingProviderType, IShippingProviderHttpClientFactory>
            {
                { ShippingProviderType.Ups, sp.GetRequiredService<UpsHttpClientFactory>() },
                { ShippingProviderType.FedEx, sp.GetRequiredService<FedExHttpClientFactory>() }
            };

            return key =>
            {
                if (factories.TryGetValue(key, out var factory))
                {
                    return factory;
                }

                throw new KeyNotFoundException($"No HTTP client factory found for shipping company type: {key}");
            };
        });
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ShippingCompanyService>();
        services.AddScoped<AddressService>();
        services.AddScoped<UriEndpointProvider>();
    }
}
