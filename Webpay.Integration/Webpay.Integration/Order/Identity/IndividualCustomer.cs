using WebpayWS;

namespace Webpay.Integration.Order.Identity;

public class IndividualCustomer : Customer<IndividualCustomer>
{
    public IndividualCustomer()
    {
        IndividualIdentity = new IndividualIdentity();
        CustomerType = CustomerType.Individual;
    }

    public string GetFirstName()
    {
        return IndividualIdentity.FirstName;
    }

    public string GetLastName()
    {
        return IndividualIdentity.LastName;
    }

    /// <summary>
    /// SetName
    /// Required for private Customers in NL and DE
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <returns>IndividualCustomer</returns>
    public IndividualCustomer SetName(string firstName, string lastName)
    {
        IndividualIdentity.FirstName = firstName;
        IndividualIdentity.LastName = lastName;
        SetFullName(firstName, lastName);
        return this;
    }

    public override IndividualCustomer SetCoAddress(string coAddress)
    {
        CoAddress = coAddress;
        return this;
    }

    public override IndividualCustomer SetEmail(string email)
    {
        Email = email;
        return this;
    }

    public override IndividualCustomer SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
        return this;
    }

    public override IndividualCustomer SetLocality(string locality)
    {
        Locality = locality;
        return this;
    }

    public override IndividualCustomer SetStreetAddress(string streetAddress, string houseNumber)
    {
        Street = streetAddress;
        HouseNumber = houseNumber;
        return this;
    }

    public override IndividualCustomer SetZipCode(string zipCode)
    {
        ZipCode = zipCode;
        return this;
    }

    public override IndividualCustomer SetIpAddress(string ipAddress)
    {
        IpAddress = ipAddress;
        return this;
    }

    /// <summary>
    /// SetNationalIdNumber
    /// Required for private customers in SE, NO, DK, FI
    /// format SE, DK:  yyyymmddxxxx
    /// format FI:  ddmmyyxxxx
    /// format NO:  ddmmyyxxxxx
    /// </summary>
    /// <param name="nationalIdNumber"></param>
    /// <returns>IndividualCustomer</returns>
    public IndividualCustomer SetNationalIdNumber(string nationalIdNumber)
    {
        NationalIdNumber = nationalIdNumber;
        return this;
    }

    public string GetInitials()
    {
        return IndividualIdentity.Initials;
    }

    /// <summary>
    /// Required for private customers in NL 
    /// </summary>
    /// <param name="initials"></param>
    /// <returns>IndividualCustomer</returns>
    public IndividualCustomer SetInitials(string initials)
    {
        IndividualIdentity.Initials = initials;
        return this;
    }

    public string GetBirthDate()
    {
        return IndividualIdentity.BirthDate;
    }

    /// <summary>
    /// Required for private customers in NL and DE
    /// Format: yyyymmdd
    /// </summary>
    /// <param name="date"></param>
    /// <returns>IndividualCustomer</returns>
    public IndividualCustomer SetBirthDate(string date)
    {
        IndividualIdentity.BirthDate = date;
        return this;
    }
}