class ShippingHttpClientFactory(IServiceProvider service, ShippingCompanyService shippingCompanyService)
{
    public async Task<IShippingHttpClient> CreateClient(int shippingCompanyId)
    {
        IShippingHttpClient shippingHttpClient;
        var httpClientFactory = service.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient();
        var shippingCompany = await shippingCompanyService.GetShippingCompanyAsync(shippingCompanyId);

        switch (shippingCompany.Name)
        {
            case "UPS":
                var upsAddressValidationRequestBuilder = service.GetRequiredService<UpsAddressValidationRequestBuilder>();
                shippingHttpClient = new UpsHttpClient
                (
                    httpClient, 
                    upsAddressValidationRequestBuilder, 
                    shippingCompany.AccountKey, 
                    shippingCompany.ApiUrl
                );
                break;
            case "Fed Ex":
                var fedExAddressValidationRequestBuilder = service.GetRequiredService<FedExAddressValidationRequestBuilder>();
                shippingHttpClient = new FedExHttpClient
                (
                    httpClient, 
                    fedExAddressValidationRequestBuilder, 
                    shippingCompany.AccountKey, 
                    shippingCompany.ApiUrl
                );
                break;
            default:
                throw new Exception("Shipping company not found.");
        }

        return shippingHttpClient;       
    }
}
