using System.Net.Http.Headers;
using System.Text;

public class UpsHttpClient : IShippingProviderHttpClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// https://developer.ups.com/api/reference?loc=en_US#operation/AddressValidation
    /// </summary>
    public UpsHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string SendRequest(string url, string requestPayload)
    {
        var content = new StringContent(requestPayload, Encoding.UTF8, "application/json");
        //var response = _httpClient.PostAsync(url, content);
        return requestPayload;
    }
}
