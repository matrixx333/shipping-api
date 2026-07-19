
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

static class ApplicationServiceExtensions
{
    public static void AddUpsHttpClient
    (
        this IServiceCollection services,
        IConfiguration config,
        IHostEnvironment env
    )
    {
        if (env.IsDevelopment())
        {
            services.Configure<ShippingProviderHttpClientSettings>
            (
                config.GetSection("UpsHttpClient")
            );
        }

        var baseAddress = config["UpsHttpClient:BaseAddress"];
        var apiKey = config["UpsHttpClient:ApiKey"];

        services.AddHttpClient<UpsHttpClient>(client =>
        {
            var headers = client.DefaultRequestHeaders;

            client.BaseAddress = baseAddress is not null ? new Uri(baseAddress) : null;
            headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            headers.Add("X-Locale", "en_US");
            headers.TryAddWithoutValidation("Content-Type", "application/json");
        });
    }

    public static void AddFedExHttpClient
    (
        this IServiceCollection services,
        IConfiguration config,
        IHostEnvironment env
    )
    {
        if (env.IsDevelopment())
        {
            services.Configure<ShippingProviderHttpClientSettings>
            (
                config.GetSection("FedExHttpClient")
            );
        }

        var baseAddress = config["FedExHttpClient:BaseAddress"];
        var apiKey = config["FedExHttpClient:ApiKey"];

        services.AddHttpClient<FedExHttpClient>(client =>
        {
            var headers = client.DefaultRequestHeaders;

            client.BaseAddress = baseAddress != null ? new Uri(baseAddress) : null;
            headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            headers.Add("X-Locale", "en_US");
            headers.TryAddWithoutValidation("Content-Type", "application/json");
        });
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
        services.AddTransient
        <
            Func<ShippingProviderType, IAddressValidationRequestBuilderFactory>
        >(sp =>
        {
            var factories = new Dictionary
            <
                ShippingProviderType,
                IAddressValidationRequestBuilderFactory
            >
            {
                {
                    ShippingProviderType.Ups,
                    sp.GetRequiredService<UpsAddressValidationBuilderFactory>()
                },
                {
                    ShippingProviderType.FedEx,
                    sp.GetRequiredService<FedExAddressValidationBuilderFactory>()
                }
            };

            return key =>
            {
                if (factories.TryGetValue(key, out var factory))
                {
                    return factory;
                }

                var message = "No address validation request builder found for "
                    + $"shipping company type: {key}";
                throw new KeyNotFoundException(message);
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

                var message = $"No HTTP client factory found for shipping company type: {key}";
                throw new KeyNotFoundException(message);
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
