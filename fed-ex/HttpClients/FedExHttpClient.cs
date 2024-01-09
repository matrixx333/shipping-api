using System.Text;

public class FedExHttpClient : IShippingProviderHttpClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html#operation/Validate%20Address
    /// </summary>
    public FedExHttpClient(HttpClient httpClient)
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
