using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Test.Util;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Test.Config
{
    [TestFixture]
    public class ConfigurationProviderTest
    {
        [Test]
        public void TestDefaultTestConfig()
        {
            IConfigurationProvider conf = SveaConfig.GetDefaultConfig();
            Assert.AreEqual("sverigetest", conf.GetUsername(PaymentType.INVOICE, CountryCode.SE));
            Assert.AreEqual("sverigetest", conf.GetPassword(PaymentType.PAYMENTPLAN, CountryCode.SE));
            Assert.AreEqual(32503, conf.GetClientNumber(PaymentType.PAYMENTPLAN, CountryCode.NO));
            Assert.AreEqual("1130", conf.GetMerchantId(PaymentType.HOSTED, CountryCode.NL));
            Assert.AreEqual("https://webservices.sveaekonomi.se/webpay_test/SveaWebPay.asmx?WSDL",
                            conf.GetEndPoint(PaymentType.INVOICE));
        }

        [Test]
        public void TestConfiguration()
        {
            var conf = new ConfigurationProviderTestData();
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(conf)
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                             .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(true, response.Accepted);
        }
    }
}