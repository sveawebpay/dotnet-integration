using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Test.Config;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.CreateOrder
{
    [TestFixture]
    public class CreateOrderTest
    {
        [Test]
        public void TestConfiguration()
        {
            var conf = new ConfigurationProviderTestData();
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(conf)
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber)
                                                                                     .SetIpAddress("123.123.123"))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestFormatShippingFeeRowsZero()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddFee(Item.ShippingFee()
                                                                         .SetShippingId("0")
                                                                         .SetName("Tess")
                                                                         .SetDescription("Tester")
                                                                         .SetAmountExVat(0)
                                                                         .SetVatPercent(0)
                                                                         .SetUnit("st"))
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestCompanyIdResponse()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(CustomerType.Company, response.CreateOrderResult.CustomerIdentity.CustomerType);
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.IndividualIdentity);
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.CompanyIdentity);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestDeCompanyIdentity()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateOrderRowDe())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetNationalIdNumber("12345")
                                                                                     .SetVatNumber("DE123456789")
                                                                                     .SetStreetAddress(
                                                                                         "Adalbertsteinweg", "1")
                                                                                     .SetZipCode("52070")
                                                                                     .SetLocality("AACHEN"))
                                                             .SetCountryCode(CountryCode.DE)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(Currency.EUR)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.AreEqual(CustomerType.Company, response.CreateOrderResult.CustomerIdentity.CustomerType);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestNlCompanyIdentity()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateOrderRowNl())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetCompanyName("Svea bakkerij 123")
                                                                                     .SetVatNumber("NL123456789A12")
                                                                                     .SetStreetAddress("broodstraat",
                                                                                                       "1")
                                                                                     .SetZipCode("1111 CD")
                                                                                     .SetLocality("BARENDRECHT"))
                                                             .SetCountryCode(CountryCode.NL)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(Currency.EUR)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.AreEqual(CustomerType.Company, response.CreateOrderResult.CustomerIdentity.CustomerType);
            Assert.IsTrue(response.Accepted);
        }
    }
}
