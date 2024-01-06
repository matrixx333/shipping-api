interface IShippingHttpClientFactory
{
    IShippingHttpClient CreateHttpClient(ShippingCompany shippingCompany);
}
