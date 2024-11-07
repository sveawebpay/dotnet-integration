using WebpayWS;

namespace Webpay.Integration.Order.Identity;

public class CompanyCustomer : Customer<CompanyCustomer>
{
    private string _addressSelector;

    public CompanyCustomer()
    {
        CompanyIdentity = new CompanyIdentity();
        CustomerType = CustomerType.Company;
    }

    public string GetCompanyName()
    {
        return FullName;
    }

    /// <summary>
    /// Required for Eu countries like NL and DE
    /// </summary>
    /// <param name="name"></param>
    /// <returns>CompanyCustomer</returns>
    public CompanyCustomer SetCompanyName(string name)
    {
        FullName = name;
        return this;
    }

    public override CompanyCustomer SetCoAddress(string coAddress)
    {
        CoAddress = coAddress;
        return this;
    }

    public override CompanyCustomer SetEmail(string email)
    {
        Email = email;
        return this;
    }

    public override CompanyCustomer SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
        return this;
    }

    public override CompanyCustomer SetLocality(string locality)
    {
        Locality = locality;
        return this;
    }

    public override CompanyCustomer SetStreetAddress(string streetAddress, string houseNumber)
    {
        Street = streetAddress;
        HouseNumber = houseNumber;
        return this;
    }

    public override CompanyCustomer SetZipCode(string zipCode)
    {
        ZipCode = zipCode;
        return this;
    }

    public override CompanyCustomer SetIpAddress(string ipAddress)
    {
        IpAddress = ipAddress;
        return this;
    }

    /// <summary>
    /// SetNationalIdNumber
    /// Required for company customers in SE, NO, DK, FI
    /// For SE: Organisationsnummer
    /// For NO: Organisasjonsnummer
    /// For DK: CVR
    /// For FI: Yritystunnus
    /// </summary>
    /// <param name="companyIdNumber"></param>
    /// <returns>CompanyCustomer</returns>
    public CompanyCustomer SetNationalIdNumber(string companyIdNumber)
    {
        NationalIdNumber = companyIdNumber;
        return this;
    }

    public string GetVatNumber()
    {
        return CompanyIdentity.CompanyVatNumber;
    }

    /// <summary>
    /// Required for NL and DE
    /// </summary>
    /// <param name="vatNumber"></param>
    /// <returns>CompanyCustomer</returns>
    public CompanyCustomer SetVatNumber(string vatNumber)
    {
        CompanyIdentity.CompanyVatNumber = vatNumber;
        return this;
    }

    /// <summary>
    /// SetAddressSelector
    /// </summary>
    /// <param name="addressSelector"></param>
    /// <returns>CompanyCustomer</returns>
    public CompanyCustomer SetAddressSelector(string addressSelector)
    {
        _addressSelector = addressSelector;
        return this;
    }

    public string GetAddressSelector()
    {
        return _addressSelector;
    }
}