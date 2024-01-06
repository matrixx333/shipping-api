
static class ApplicationServiceExtensions
{
    public static void AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<UpsHttpClient>();
        services.AddHttpClient<FedExHttpClient>();
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
