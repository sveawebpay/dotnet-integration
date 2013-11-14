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

            Assert.AreEqual(GetCustomerAddressesRejectionCode.Accepted, response.RejectionCode);
            Assert.AreEqual("Persson, Tess T", response.Addresses[0].LegalName);
            Assert.AreEqual("Testgatan 1", response.Addresses[0].AddressLine2);
            Assert.AreEqual("Stan", response.Addresses[0].Postarea);
        }

        [Test]
        public void TestResultGetAddresses()
        {
            GetCustomerAddressesResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderTypeInvoice()
                                                             .SetIndividual(TestingTool.DefaultTestIndividualNationalIdNumber)
                                                             .DoRequest();

            Assert.AreEqual(GetCustomerAddressesRejectionCode.Accepted, response.RejectionCode);
            Assert.AreEqual("Tess T", response.Addresses[0].FirstName);
            Assert.AreEqual("Persson", response.Addresses[0].LastName);
            Assert.AreEqual("Testgatan 1", response.Addresses[0].AddressLine2);
            Assert.AreEqual(99999, response.Addresses[0].Postcode);
            Assert.AreEqual("Stan", response.Addresses[0].Postarea);
        }

        [Test]
        public void TestResultGetIndividualAddressesNO()
        {
            GetCustomerAddressesResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                             .SetCountryCode(CountryCode.NO)
                                                             .SetOrderTypeInvoice()
                                                             .SetIndividual("17054512066")
                                                             .DoRequest();

            Assert.AreEqual(GetCustomerAddressesRejectionCode.Error, response.RejectionCode);
            Assert.That(response.ErrorMessage, Is.EqualTo("CountryCode: Supported countries are SE, DK"));
        }

        [Test]
        public void TestResultGetCompanyAddressesNO()
        {
            GetCustomerAddressesResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                             .SetCountryCode(CountryCode.NO)
                                                             .SetOrderTypeInvoice()
                                                             .SetCompany("923313850")
                                                             .DoRequest();

            Assert.AreEqual(GetCustomerAddressesRejectionCode.Accepted, response.RejectionCode);
            Assert.AreEqual("Test firma AS", response.Addresses[0].LegalName);
            Assert.AreEqual("Testveien 1", response.Addresses[0].AddressLine2);
            Assert.AreEqual("Oslo", response.Addresses[0].Postarea);
        }
    }
}