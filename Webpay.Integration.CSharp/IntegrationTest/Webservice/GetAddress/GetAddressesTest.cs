using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
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
            GetAddressesEuResponse response = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .SetIndividual("460509-2222")
                                                              .SetZipCode("99999")
                                                              .SetOrderTypeInvoice()
                                                              .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
            Assert.AreEqual("Persson, Tess T", response.GetAddressesResult.Addresses[0].Address.FullName);
            Assert.AreEqual("Testgatan 1", response.GetAddressesResult.Addresses[0].Address.Street);
            Assert.AreEqual("Stan", response.GetAddressesResult.Addresses[0].Address.Locality);
        }

        [Test]
        public void TestResultGetAddresses()
        {
            GetAddressesEuResponse request = WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig())
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderTypeInvoice()
                                                             .SetIndividual(TestingTool.DefaultTestIndividualNationalIdNumber)
                                                             .SetZipCode("99999")
                                                             .DoRequest();

            Assert.AreEqual(0, request.ResultCode);
            Assert.IsTrue(request.Accepted);
            Assert.AreEqual("Tess T", request.GetAddressesResult.Addresses[0].FirstName);
            Assert.AreEqual("Persson", request.GetAddressesResult.Addresses[0].LastName);
            Assert.AreEqual("Testgatan 1", request.GetAddressesResult.Addresses[0].Address.Street);
            Assert.AreEqual("99999", request.GetAddressesResult.Addresses[0].Address.ZipCode);
            Assert.AreEqual("Stan", request.GetAddressesResult.Addresses[0].Address.Locality);
        }
    }
}