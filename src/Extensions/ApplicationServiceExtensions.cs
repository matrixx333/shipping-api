
static class ApplicationServiceExtensions
{
    public static void AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<UpsHttpClient>();
        services.AddHttpClient<FedExHttpClient>();
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
