using System.Net.Http.Headers;
using System.Text;

class UpsHttpClient : IShippingHttpClient
{
    private readonly HttpClient _httpClient;
    private UpsAddressValidationRequestBuilder _builder;

    /// <summary>
    /// https://developer.ups.com/api/reference?loc=en_US#operation/AddressValidation
    /// </summary>
    public UpsHttpClient(
        HttpClient httpClient, 
        UpsAddressValidationRequestBuilder builder,
        string accountKey)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://wwwcie.ups.com/api/addressvalidation/v1/1?regionalrequestindicator=string&maximumcandidatelistsize=1");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accountKey);
        _httpClient.DefaultRequestHeaders.Add("X-Locale", "en_US");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        _builder = builder;
    }

    public async Task<string> ValidateAddress(int addressId)
    {
        await _builder.BuildAddressRequest(addressId);
        var request = _builder.SerializeRequest();
        var content = new StringContent(request, Encoding.UTF8, "application/json");
        // since we do not have an actual UPS account, return the request payload
        return request;        // return await _httpClient.PostAsync(_httpClient.BaseAddress, content);
    }
}
