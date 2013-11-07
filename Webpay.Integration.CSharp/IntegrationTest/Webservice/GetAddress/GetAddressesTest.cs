using NUnit.Framework;
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
            GetAddressesEuResponse response = WebpayConnection.GetAddresses()
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
            GetAddressesEuResponse response = WebpayConnection.GetAddresses()
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .SetIndividual("194605092222")
                                                              .SetZipCode("99999")
                                                              .SetOrderTypeInvoice()
                                                              .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.AreEqual(true, response.Accepted);
            Assert.AreEqual("Tess T", response.GetAddressesResult.Addresses[0].FirstName);
            Assert.AreEqual("Persson", response.GetAddressesResult.Addresses[0].LastName);
            Assert.AreEqual("Testgatan 1", response.GetAddressesResult.Addresses[0].Address.Street);
            Assert.AreEqual("99999", response.GetAddressesResult.Addresses[0].Address.ZipCode);
            Assert.AreEqual("Stan", response.GetAddressesResult.Addresses[0].Address.Locality);
        }
    }
}