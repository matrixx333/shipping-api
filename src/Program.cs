using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ShippingDb>(options =>
    options.UseInMemoryDatabase("Addresses"));

builder.Services.AddServices();
builder.Services.AddHttpClients();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Shipping API",
        Description = "API for validating shipping addresses from different shipping providers.",
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ShippingDb>();
    dbContext.Database.EnsureCreated();
}

app.MapPost("/validate-address", async (AddressValidationRequest addressValidationRequest, ShippingDb db) =>
{
    string response;

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var shippingCompanyService = services.GetRequiredService<ShippingCompanyService>();
        
        var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient();
        var shippingCompany = await shippingCompanyService.GetShippingCompanyAsync(addressValidationRequest.ShippingCompanyId);

        switch (shippingCompany.Name)
        {
            case "UPS":
                var upsAddressValidationRequestBuilder = services.GetRequiredService<UpsAddressValidationRequestBuilder>();
                var upsHttpClient = new UpsHttpClient(httpClient, upsAddressValidationRequestBuilder, shippingCompany.AccountKey);
                response = await upsHttpClient.ValidateAddress(addressValidationRequest.AddressId);
                break;
            case "Fed Ex":
                var fedExAddressValidationRequestBuilder = services.GetRequiredService<FedExAddressValidationRequestBuilder>();
                var fedExHttpClient = new FedExHttpClient(httpClient, fedExAddressValidationRequestBuilder, shippingCompany.AccountKey);
                response = await fedExHttpClient.ValidateAddress(addressValidationRequest.AddressId);
                break;
            default:
                throw new Exception("Shipping Company not found.");
        }        
    }

    return Results.Ok(response);
})
.WithTags("ValidateAddresses");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();