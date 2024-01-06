using System.Net.Http.Headers;
using System.Text;

public class FedExHttpClient : IShippingHttpClient
{
    private readonly HttpClient _httpClient;
    private FedExAddressValidationRequestBuilder _builder;

    /// <summary>
    /// https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html#operation/Validate%20Address
    /// </summary>
    public FedExHttpClient(
        HttpClient httpClient, 
        FedExAddressValidationRequestBuilder builder,
        ShippingCompany shippingCompany)
    {
        _httpClient = httpClient;
        _builder = builder;

        _httpClient.BaseAddress = new Uri($"{shippingCompany.ApiUrl}/address/v1/addresses/resolve");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", shippingCompany.AccountKey);
        _httpClient.DefaultRequestHeaders.Add("X-Locale", "en_US");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
    }

    public async Task<string> ValidateAddress(Address address)
    {
        _builder.BuildAddressRequest(address);
        var request = _builder.SerializeRequest();
        var content = new StringContent(request, Encoding.UTF8, "application/json");
        // since we do not have an actual Fed Ex account, return the request payload
        return request;        // return await _httpClient.PostAsync(_httpClient.BaseAddress, content);
    }
}
