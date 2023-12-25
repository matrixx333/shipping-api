interface IShippingHttpClient
{
    /// <summary>
    /// Validates the address stored in the database against the selected shipping company API. 
    /// </summary>
    /// <param name="addressId">The ID of the address that is stored in the database.</param>
    /// <returns></returns>
    Task<string> ValidateAddress(int addressId);
}
