# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Core (net8.0) minimal API that validates shipping addresses through multiple shipping providers (UPS, FedEx). An internal web app POSTs a `shippingCompanyId` + `addressId`; this API looks up the address from a database, builds a provider-specific JSON payload, and forwards it to that provider's API.

Each shipping provider has its own request schema, so the codebase is built around a provider-keyed factory pattern that lets new providers be added without touching existing ones. **Note:** this project does not actually call live provider APIs — `UpsHttpClient`/`FedExHttpClient.SendRequest` just echo the serialized payload back (the real `PostAsync` call is commented out), since there are no live UPS/FedEx developer accounts.

## Build & run

```
dotnet build                      # build the whole solution (shipping-api.sln)
dotnet run --project Api          # run the API (Swagger UI opens at /swagger)
dotnet watch --project Api run    # run with hot reload
dotnet test                       # run the unit tests (Tests project)
```

## Testing

The `Tests` project holds the unit suite (NUnit + Moq + FluentAssertions). Test data comes from fluent builders in `Tests/Builders/` (`AddressBuilder`, `ShippingCompanyBuilder`, and the internal `ShippingDbBuilder`, which seeds an isolated EF Core InMemory `ShippingDb` per test); SUTs with multiple dependencies are wired through a composition-root harness in `Tests/Harnesses/`. The internal `Api` types (`AddressService`, `ShippingCompanyService`, `ShippingDb`, `ApplicationServiceExtensions`) are reachable because `Api.csproj` has `<InternalsVisibleTo Include="Tests" />`.

Collect coverage with coverlet (build single-threaded so instrumentation isn't corrupted); it writes a cobertura report under `Tests/TestResults/`:

```
dotnet test Tests/Tests.csproj -m:1 -p:CollectCoverage=true -p:CoverletOutputFormat=cobertura
```

The logic-bearing classes are at 100% line/branch coverage. Two things are intentionally excluded: the minimal-API endpoint in `Program.cs` (integration-test territory — needs the wired host and a JWT) and plain POCOs with no logic. The `default` arm of the switch in [UriEndpointProvider](Api/Services/UriEndpointProvider.cs) is unreachable dead code — the up-front `Enum.IsDefined` guard means the cast can only ever be `Ups` or `FedEx` — so that one branch cannot be covered without a production change.

Local config: `Api/appsettings.json` and `Api/appsettings.Development.json` are gitignored (contain provider API keys) and must be created locally. Expected shape:

```json
{
  "Auth0": { "Domain": "...", "Audience": "..." },
  "UpsHttpClient": { "BaseAddress": "...", "ApiKey": "...", "AddressValidationEndpoint": "..." },
  "FedExHttpClient": { "BaseAddress": "...", "ApiKey": "...", "AddressValidationEndpoint": "..." }
}
```

The API requires a valid Auth0 JWT bearer token on `POST /validate-address` (`.RequireAuthorization()` in [Program.cs](Api/Program.cs)).

## Solution structure

Six projects, dependency direction flows one way (`Api` → provider projects → `Common`/`Helpers` → `Models`):

- **Models** — plain POCOs shared everywhere (`Address`, `ShippingCompany`, `AddressValidationRequest`, `ShippingProviderType` enum). No dependencies.
- **Helpers** — `SerializationHelper` (thin `System.Text.Json` wrapper). No dependencies.
- **Common** — the provider-agnostic contracts every provider implements: `IAddressValidationRequestBuilder`, `IAddressValidationRequestBuilderFactory`, `IShippingProviderHttpClient`, `IShippingProviderHttpClientFactory`, `ISerializableRequest`.
- **Ups** / **FedEx** — one project per shipping provider, each implementing the `Common` interfaces with that provider's own request schema (`Builders/`, `Factories/`, `HttpClients/`).
- **Api** — the ASP.NET Core host: minimal API endpoint, EF Core InMemory `DbContext`, DI wiring, and the factory-of-factories that dispatches by provider.

## Adding a new shipping provider

This is the most common extension point, so follow the existing UPS/FedEx pattern exactly:

1. Add a value to `ShippingProviderType` in [Models/ShippingProviderType.cs](Models/ShippingProviderType.cs).
2. Create a new project (e.g. `Dhl/`) referencing `Common`, `Models`, `Helpers`, mirroring the `Ups`/`FedEx` project layout: `Builders/<Provider>AddressValidationRequestBuilder.cs`, `Factories/<Provider>AddressValidationBuilderFactory.cs`, `Factories/<Provider>HttpClientFactory.cs`, `HttpClients/<Provider>HttpClient.cs` — each implementing the matching `Common` interface.
3. Register the new project in `shipping-api.sln` and add a `ProjectReference` from `Api/Api.csproj`.
4. Wire it up in [Api/Extensions/ApplicationServiceExtensions.cs](Api/Extensions/ApplicationServiceExtensions.cs): add an `Add<Provider>HttpClient` extension method, register the builder factory and HTTP client factory in `AddFactories` (each concrete builder is constructed fresh via `new` inside its `I...BuilderFactory.CreateBuilder()` — builders are never registered directly in DI, since a builder must be a new instance per construction, not a shared/scoped service), and add the new `ShippingProviderType` → factory mapping in both dictionaries inside `AddFactoryResolvers`.
5. Add the provider's config section (`BaseAddress`, `ApiKey`, `AddressValidationEndpoint`) to `appsettings.json` and extend `UriEndpointProvider` ([Api/Services/UriEndpointProvider.cs](Api/Services/UriEndpointProvider.cs)) with the new endpoint lookup.

## Key architectural pattern: factory-of-factories dispatch

Provider selection happens by `ShippingProviderType` (an `int` from the request, cast via `Enum.IsDefined`), not by DI-time polymorphism. The flow:

1. `ApplicationServiceExtensions.AddFactoryResolvers` registers two `Func<ShippingProviderType, IXxxFactory>` delegates in DI — each closes over a `Dictionary<ShippingProviderType, IXxxFactory>` built from the individually-registered per-provider factories.
2. `AddressValidationBuilderFactory` and `ShippingProviderHttpClientFactory` (in `Api/Factories/`) both derive from `BaseFactory<TFactory>` ([Api/Factories/BaseFactory.cs](Api/Factories/BaseFactory.cs)), which validates the incoming `shippingCompanyId` and resolves the right per-provider factory via the injected `Func<...>` delegate.
3. The per-provider factory (e.g. `FedExAddressValidationBuilderFactory`) then constructs the concrete builder/HTTP client for that provider.

When touching provider dispatch logic, both `AddressValidationBuilderFactory` (picks the request builder) and `ShippingProviderHttpClientFactory` (picks the HTTP client) need to stay in sync — they're parallel structures keyed the same way.

## Builder pattern for provider payloads

Each provider's `*AddressValidationRequestBuilder` implements `IAddressValidationRequestBuilder` (fluent `BuildAddressRequest(Address) → this`, then `SerializeRequest()`). The provider-specific JSON shape (e.g. FedEx's `AddressesToValidate` vs UPS's nested `XAVRequest.AddressKeyFormat`) is modeled with **private nested DTO classes** inside the builder — these are intentionally separate from the shared `Models.Address` POCO since each provider's wire format differs.

## Data layer

`ShippingDb` (EF Core, InMemory provider only — see `Program.cs`) holds `Address` and `ShippingCompany` records, seeded with hardcoded `HasData` rows in `OnModelCreating` ([Api/Data/ShippingDb.cs](Api/Data/ShippingDb.cs)). There is no real database/migrations — this stands in for "the customer's database" described in the README.
