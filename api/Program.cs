using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<ShippingDb>(options =>
    options.UseInMemoryDatabase("Addresses"));

builder.Services.AddServices();
builder.Services.AddBuilders();
builder.Services.AddFactories();
builder.Services.AddFactoryResolvers();
builder.Services.AddUpsHttpClient(configuration, builder.Environment);
builder.Services.AddFedExHttpClient(configuration, builder.Environment);
builder.Services.AddEndpointsApiExplorer();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Shipping API",
        Description = "API for validating shipping addresses from different shipping providers.",
    });
});

var isDevelopment = builder.Environment.IsDevelopment();

if (!isDevelopment)
{
    var appConfigEndpoint = Environment.GetEnvironmentVariable("AzureAppConfigurationEndpoint");
    // Add Azure App Configuration
    if (appConfigEndpoint is not null)
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential());
        });
    }
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ShippingDb>();
    dbContext.Database.EnsureCreated();
}

app.MapPost("/validate-address", async
(
    AddressValidationRequest addressValidationRequest,
    AddressService addressService,
    UriEndpointProvider uriEndpointProvider,
    ShippingProviderHttpClientFactory httpClientFactory,
    AddressValidationBuilderFactory addressValidationBuilderFactory
) =>
{
    //HttpResponseMessage response;
    string response;
    using var scope = app.Services.CreateScope();
    
    var addressValidationRequestBuilder = addressValidationBuilderFactory.CreateBuilder(addressValidationRequest.ShippingCompanyId);
    var address = await addressService.GetAddressAsync(addressValidationRequest.AddressId);

    var requestPayload = addressValidationRequestBuilder
                            .BuildAddressRequest(address)
                            .SerializeRequest();

    var addressValidationEndpoint = uriEndpointProvider.GetAddressValidationEndpoint(addressValidationRequest.ShippingCompanyId);
    var httpClient = httpClientFactory.CreateHttpClient(addressValidationRequest.ShippingCompanyId);
    response = httpClient.SendRequest(addressValidationEndpoint, requestPayload);

    return Results.Ok(response);
})
.WithTags("ValidateAddresses");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();