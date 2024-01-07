using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<ShippingDb>(options =>
    options.UseInMemoryDatabase("Addresses"));

builder.Services.AddServices();
builder.Services.AddFactories();
builder.Services.AddUpsHttpClient(configuration);
builder.Services.AddFedExHttpClient(configuration);
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ShippingDb>();
    dbContext.Database.EnsureCreated();
}

app.MapPost("/validate-address", async (AddressValidationRequest addressValidationRequest, ShippingHttpClientFactory factory, ShippingCompanyService shippingCompanyService, AddressService addressService) =>
{
    //HttpResponseMessage response;
    string response;
    using (var scope = app.Services.CreateScope())
    {
        var shippingCompany = await shippingCompanyService.GetShippingCompanyAsync(addressValidationRequest.ShippingCompanyId);
        var httpClient = factory.CreateHttpClient(shippingCompany);
        var address = await addressService.GetAddressAsync(addressValidationRequest.AddressId);
        response = await httpClient.ValidateAddress(address);    
    }
    return Results.Ok(response);
})
.WithTags("ValidateAddresses");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();