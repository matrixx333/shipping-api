using System.Net.Http.Headers;
using System.Text;

public class FedExHttpClient : IShippingHttpClient
{
    private readonly HttpClient _httpClient;
    private IAddressValidationRequestBuilder _builder;

    /// <summary>
    /// https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html#operation/Validate%20Address
    /// </summary>
    public FedExHttpClient(
        HttpClient httpClient, 
        IAddressValidationRequestBuilder builder)
    {
        _httpClient = httpClient;
        _builder = builder;
    }

    public async Task<string> ValidateAddress(Address address)
    {
        var request = BuildAddressRequest(address);
        var content = new StringContent(request, Encoding.UTF8, "application/json");
        // since we do not have an actual Fed Ex account, return the request payload             
        //return await _httpClient.PostAsync(_httpClient.BaseAddress, content);
        return request;   
    }

    private string BuildAddressRequest(Address address)
    {
        _builder.BuildAddressRequest(address);
        return _builder.SerializeRequest();
    }
}
