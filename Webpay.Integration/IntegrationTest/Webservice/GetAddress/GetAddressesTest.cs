using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using WebpayWS;

namespace Webpay.Integration.IntegrationTest.Webservice.GetAddress;

[TestFixture]
public class GetAddressesTest
{
    [Test]
    public async Task TestGetAddresses()
    {
        var response = await WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetIndividual("460509-2222")
            .SetZipCode("99999")
            .SetOrderTypeInvoice()
            .DoRequestAsync();

        Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Accepted));
        Assert.That(response.Addresses[0].LegalName, Is.EqualTo("Persson, Tess T"));
        Assert.That(response.Addresses[0].AddressLine2, Is.EqualTo("Testgatan 1"));
        Assert.That(response.Addresses[0].Postarea, Is.EqualTo("Stan"));
    }

    [Test]
    public async Task TestGetAddressesWithoutOrderType()
    {
        var response = await WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetIndividual("460509-2222")
            .SetZipCode("99999")
            .DoRequestAsync();

        Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Accepted));
        Assert.That(response.Addresses[0].LegalName, Is.EqualTo("Persson, Tess T"));
        Assert.That(response.Addresses[0].AddressLine2, Is.EqualTo("Testgatan 1"));
        Assert.That(response.Addresses[0].Postarea, Is.EqualTo("Stan"));
    }

    [Test]
    public async Task TestResultGetAddresses()
    {
        var response = await WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderTypeInvoice()
            .SetIndividual(TestingTool.DefaultTestIndividualNationalIdNumber)
            .DoRequestAsync();

        Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Accepted));
        Assert.That(response.Addresses[0].FirstName, Is.EqualTo("Tess"));
        Assert.That(response.Addresses[0].LastName, Is.EqualTo("Persson"));
        Assert.That(response.Addresses[0].AddressLine2, Is.EqualTo("Testgatan 1"));
        Assert.That(response.Addresses[0].Postcode, Is.EqualTo(99999));
        Assert.That(response.Addresses[0].Postarea, Is.EqualTo("Stan"));
    }

    [Test]
    public async Task TestResultGetIndividualAddressesNO()
    {
        var response = await WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.NO)
            .SetOrderTypeInvoice()
            .SetIndividual("17054512066")
            .DoRequestAsync();

        Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Error));
        Assert.That(response.ErrorMessage, Is.EqualTo("Invalid CountryCode: Supported CountryCodes are: SE, DK."));
    }

    [Test]
    public async Task TestResultGetCompanyAddressesNO()
    {
        var response = await WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.NO)
            .SetOrderTypeInvoice()
            .SetCompany("923313850")
            .DoRequestAsync();

        Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Accepted));
        Assert.That(response.Addresses[0].LegalName, Is.EqualTo("Test firma AS"));
        Assert.That(response.Addresses[0].AddressLine2, Is.EqualTo("Testveien 1"));
        Assert.That(response.Addresses[0].Postarea, Is.EqualTo("Oslo"));
    }
}