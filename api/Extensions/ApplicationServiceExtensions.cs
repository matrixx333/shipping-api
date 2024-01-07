
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

static class ApplicationServiceExtensions
{
    public static void AddUpsHttpClient(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<UpsHttpClientSettings>(config.GetSection("UpsHttpClient"));
        var clientSettings = config.GetSection("UpsHttpClient").Get<UpsHttpClientSettings>();

        services.AddHttpClient<UpsHttpClient>(client =>
        {
            client.BaseAddress = new Uri($"{clientSettings!.Url}/addressvalidation/v1/1?regionalrequestindicator=string&maximumcandidatelistsize=1");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientSettings.ApiKey);
            client.DefaultRequestHeaders.Add("X-Locale", "en_US");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        });        
    }

    public static void AddFedExHttpClient(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<FedExHttpClientSettings>(config.GetSection("FedExHttpClient"));

        services.AddHttpClient<FedExHttpClient>(client =>
        {
            var clientSettings = config.GetSection("FedExHttpClient").Get<FedExHttpClientSettings>();
            client.BaseAddress = new Uri($"{clientSettings!.Url}/address/v1/addresses/resolve");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientSettings.ApiKey);
            client.DefaultRequestHeaders.Add("X-Locale", "en_US");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        });
    }

    public static void AddFactories(this IServiceCollection services)
    {
        services.AddScoped(sp =>
        {
            return new UpsHttpClientFactory(
                sp.GetRequiredService<IHttpClientFactory>(),
                sp.GetRequiredService<UpsAddressValidationRequestBuilder>()
            );
        });

        services.AddScoped(sp =>
        {
            return new FedExHttpClientFactory(
                sp.GetRequiredService<IHttpClientFactory>(),
                sp.GetRequiredService<FedExAddressValidationRequestBuilder>()
            );
        });

        services.AddTransient<Func<ShippingCompanyType, IShippingHttpClientFactory>>(sp =>
        {
            var factories = new Dictionary<ShippingCompanyType, IShippingHttpClientFactory>
            {
                { ShippingCompanyType.Ups, sp.GetRequiredService<UpsHttpClientFactory>() },
                { ShippingCompanyType.FedEx, sp.GetRequiredService<FedExHttpClientFactory>() }
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
        services.AddScoped<UpsAddressValidationRequestBuilder>();
        services.AddScoped<FedExAddressValidationRequestBuilder>();
        services.AddScoped<ShippingHttpClientFactory>();
    }
}
