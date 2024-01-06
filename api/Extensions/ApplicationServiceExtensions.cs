
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

        services.AddTransient<Func<ShippingCompanyType, IShippingHttpClientFactory>>(sp => key =>
        {
            switch (key)
            {
                case ShippingCompanyType.Ups:
                    return sp.GetRequiredService<UpsHttpClientFactory>();
                case ShippingCompanyType.FedEx:
                    return sp.GetRequiredService<FedExHttpClientFactory>();
                default:
                    throw new KeyNotFoundException();
            }
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
