using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
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
            Assert.AreEqual(36000, conf.GetClientNumber(PaymentType.PAYMENTPLAN, CountryCode.NO));
            Assert.AreEqual("1130", conf.GetMerchantId(PaymentType.HOSTED, CountryCode.NL));
            Assert.AreEqual("https://webservices.sveaekonomi.se/webpay_test/SveaWebPay.asmx?WSDL",
                            conf.GetEndPoint(PaymentType.INVOICE));
        }

        [Test]
        public void TestConfiguration()
        {
            var conf = new ConfigurationProviderTestData();
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(conf)
                                                 .AddOrderRow(Item.OrderRow()
                                                                  .SetArticleNumber("1")
                                                                  .SetQuantity(2)
                                                                  .SetAmountExVat(new decimal(100.00))
                                                                  .SetDescription("Specification")
                                                                  .SetName("Prod")
                                                                  .SetUnit("st")
                                                                  .SetVatPercent(25)
                                                                  .SetDiscountPercent(0))
                                                 .AddOrderRow(Item.OrderRow()
                                                                  .SetArticleNumber("1")
                                                                  .SetQuantity(2)
                                                                  .SetAmountExVat(new decimal(100.00))
                                                                  .SetDescription("Specification")
                                                                  .SetName("Prod")
                                                                  .SetVatPercent(25)
                                                                  .SetDiscountPercent(0))
                                                 .AddCustomerDetails(Item.IndividualCustomer()
                                                                         .SetNationalIdNumber("194605092222")
                                                                         .SetIpAddress("123.123.123"))
                                                 .SetCountryCode(CountryCode.SE)
                                                 .SetOrderDate("2012-12-12")
                                                 .SetClientOrderNumber("33")
                                                 .SetCurrency(Currency.SEK)
                                                 .UseInvoicePayment()
                                                 .DoRequest();

            Assert.AreEqual(true, response.Accepted);
        }
    }
}