namespace Tests.Builders;

/// <summary>
/// Fluent builder for <see cref="Address"/> test data. Defaults describe a
/// complete, valid US address (both address lines populated) with id 1.
/// </summary>
public class AddressBuilder
{
    private int _id = 1;
    private string _address1 = "555 Somewhere St.";
    private string? _address2 = "Suite 100";
    private string _city = "Altamonte Springs";
    private string _state = "FL";
    private string _zipCode = "32789";
    private string _countryCode = "US";

    public static AddressBuilder New() => new();

    public AddressBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public AddressBuilder WithAddress1(string? address1)
    {
        _address1 = address1!;
        return this;
    }

    public AddressBuilder WithAddress2(string? address2)
    {
        _address2 = address2;
        return this;
    }

    public AddressBuilder InCity(string city)
    {
        _city = city;
        return this;
    }

    public AddressBuilder InState(string state)
    {
        _state = state;
        return this;
    }

    public AddressBuilder WithZipCode(string zipCode)
    {
        _zipCode = zipCode;
        return this;
    }

    public AddressBuilder InCountry(string countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public Address Build()
    {
        return new Address
        {
            Id = _id,
            Address1 = _address1,
            Address2 = _address2,
            City = _city,
            State = _state,
            ZipCode = _zipCode,
            CountryCode = _countryCode
        };
    }
}
