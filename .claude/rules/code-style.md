# Code style

## Hard limit: 100 characters per line

Applies to `*.cs`. Tabs count as 4. Never exceed it — break the line instead, using the
patterns below. Enforced by `.claude/hooks/check-line-length.ps1`.

Do not reformat generated code: `*.g.cs`, or anything under `obj/` or `bin/`.

## Long parameter lists

When a method declaration or invocation would exceed the limit, put the open paren on its
own line, one parameter per line at one indent level, and the close paren on its own line.

```csharp
public static void AddUpsHttpClient
(
    this IServiceCollection services,
    IConfiguration config,
    IHostEnvironment env
)
{
    ...
}
```

The `app.MapPost` endpoint in [Api/Program.cs](../../Api/Program.cs) (lines 58-65) is the
in-repo reference for this.

## Fluent chains

Break before each `.`, with continuations indented **one level (4 spaces)** from the
receiver.

```csharp
var results = repository.GetAll()
    .Where(x => x.IsActive)
    .OrderBy(x => x.Name)
    .AsNoTracking()
    .ToList();
```

This is the style used in [Api/Data/ShippingDb.cs](../../Api/Data/ShippingDb.cs) and in the
`BuildAddressRequest(...).SerializeRequest()` chain in
[Api/Program.cs](../../Api/Program.cs). Never align continuations to the column of the
opening call — one indent level, always, however deep the receiver expression is.

## Long object initializers

One property per line, brace on its own line.

```csharp
// before — 152 characters
new Address { Id = 1, Address1 = "555 Somewhere St.", City = "Altamonte Springs", State = "FL", ZipCode = "32789", CountryCode = "US" },

// after
new Address
{
    Id = 1,
    Address1 = "555 Somewhere St.",
    City = "Altamonte Springs",
    State = "FL",
    ZipCode = "32789",
    CountryCode = "US"
},
```

## Long generic type arguments

Break after the opening `<`, one argument per line.

```csharp
var factories = new Dictionary
<
    ShippingProviderType,
    IAddressValidationRequestBuilderFactory
>
{
    { ShippingProviderType.Ups, sp.GetRequiredService<UpsAddressValidationBuilderFactory>() },
    ...
};
```

## Long interpolated strings

Do not split mid-interpolation. Assign the message to a local first, or use a raw string
literal.

```csharp
// before
throw new KeyNotFoundException($"No address validation request builder found for shipping company type: {key}");

// after
var message = $"No address validation request builder found for shipping company type: {key}";
throw new KeyNotFoundException(message);
```

## Whitespace, indentation and spacing

Governed by [.editorconfig](../../.editorconfig) and verified by `dotnet format` — 4-space
indent, Allman braces, spaces around operators, no trailing whitespace, final newline. Run
it yourself with:

```
dotnet format whitespace shipping-api.sln --verify-no-changes
```

Drop `--verify-no-changes` to apply the fixes. This is safe to auto-apply: `dotnet format`
parses the code with Roslyn rather than pattern-matching text.
