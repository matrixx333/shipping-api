using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = $"https://{configuration["Auth0:Domain"]}/";
    options.TokenValidationParameters =
        new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidAudience = configuration["Auth0:Audience"],
            ValidIssuer = $"{builder.Configuration["Auth0:Domain"]}"
        };
});
builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ShippingDb>();
    dbContext.Database.EnsureCreated();
}

app.MapPost("/validate-address", async
(
    AddressValidationRequest request,
    AddressService addressService,
    UriEndpointProvider uriEndpointProvider,
    ShippingProviderHttpClientFactory httpClientFactory,
    AddressValidationBuilderFactory addressValidationBuilderFactory
) =>
{
    //HttpResponseMessage response;
    string response;
    using var scope = app.Services.CreateScope();

    var addressValidationRequestBuilder = addressValidationBuilderFactory.CreateBuilder(request.ShippingCompanyId);
    var address = await addressService.GetAddressAsync(request.AddressId);

    var requestPayload = addressValidationRequestBuilder
                            .BuildAddressRequest(address)
                            .SerializeRequest();

    var addressValidationEndpoint = uriEndpointProvider.GetAddressValidationEndpoint(request.ShippingCompanyId);
    var httpClient = httpClientFactory.CreateHttpClient(request.ShippingCompanyId);
    response = httpClient.SendRequest(addressValidationEndpoint, requestPayload);

    return Results.Ok(response);
})
.WithTags("ValidateAddresses")
.RequireAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();