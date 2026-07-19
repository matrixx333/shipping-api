namespace Tests.Builders;

/// <summary>
/// Fluent builder for <see cref="ShippingCompany"/> test data. Defaults describe
/// a valid UPS shipping company with id 1.
/// </summary>
public class ShippingCompanyBuilder
{
    private int _id = 1;
    private string _name = "UPS";
    private string _accountNumber = "12345";

    public static ShippingCompanyBuilder New() => new();

    public ShippingCompanyBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public ShippingCompanyBuilder Named(string name)
    {
        _name = name;
        return this;
    }

    public ShippingCompanyBuilder WithAccountNumber(string accountNumber)
    {
        _accountNumber = accountNumber;
        return this;
    }

    public ShippingCompany Build()
    {
        return new ShippingCompany
        {
            Id = _id,
            Name = _name,
            AccountNumber = _accountNumber
        };
    }
}
