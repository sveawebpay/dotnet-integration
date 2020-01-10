using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.GetAddress
{
    [TestFixture]
    public class GetAddressesTest
    {
        [Test]
        public void TestGetAddresses()
        {
            GetCustomerAddressesResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .SetIndividual("460509-2222")
                                                              .SetZipCode("99999")
                                                              .SetOrderTypeInvoice()
                                                              .DoRequest();

            Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Accepted));
            Assert.That(response.Addresses[0].LegalName, Is.EqualTo("Persson Tess T"));
            Assert.That(response.Addresses[0].AddressLine2, Is.EqualTo("Testgatan 1"));
            Assert.That(response.Addresses[0].Postarea, Is.EqualTo("Stan"));
        }

        [Test]
        public void TestGetAddressesWithoutOrderType()
        {
            GetCustomerAddressesResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .SetIndividual("460509-2222")
                                                              .SetZipCode("99999")
                                                              .DoRequest();

            Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Accepted));
            Assert.That(response.Addresses[0].LegalName, Is.EqualTo("Persson Tess T"));
            Assert.That(response.Addresses[0].AddressLine2, Is.EqualTo("Testgatan 1"));
            Assert.That(response.Addresses[0].Postarea, Is.EqualTo("Stan"));
        }

        [Test]
        public void TestResultGetAddresses()
        {
            GetCustomerAddressesResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderTypeInvoice()
                                                             .SetIndividual(TestingTool.DefaultTestIndividualNationalIdNumber)
                                                             .DoRequest();

            Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Accepted));
            Assert.That(response.Addresses[0].FirstName, Is.EqualTo("Tess"));
            Assert.That(response.Addresses[0].LastName, Is.EqualTo("Persson"));
            Assert.That(response.Addresses[0].AddressLine2, Is.EqualTo("Testgatan 1"));
            Assert.That(response.Addresses[0].Postcode, Is.EqualTo(99999));
            Assert.That(response.Addresses[0].Postarea, Is.EqualTo("Stan"));
        }

        [Test]
        public void TestResultGetIndividualAddressesNO()
        {
            GetCustomerAddressesResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                             .SetCountryCode(CountryCode.NO)
                                                             .SetOrderTypeInvoice()
                                                             .SetIndividual("17054512066")
                                                             .DoRequest();

            Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Error));
            Assert.That(response.ErrorMessage, Is.EqualTo("Invalid CountryCode: Supported CountryCodes are: SE, DK."));
        }

        [Test]
        public void TestResultGetCompanyAddressesNO()
        {
            GetCustomerAddressesResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                             .SetCountryCode(CountryCode.NO)
                                                             .SetOrderTypeInvoice()
                                                             .SetCompany("923313850")
                                                             .DoRequest();

            Assert.That(response.RejectionCode, Is.EqualTo(GetCustomerAddressesRejectionCode.Accepted));
            Assert.That(response.Addresses[0].LegalName, Is.EqualTo("Test firma AS"));
            Assert.That(response.Addresses[0].AddressLine2, Is.EqualTo("Testveien 1"));
            Assert.That(response.Addresses[0].Postarea, Is.EqualTo("Oslo"));
        }
    }
}