using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Handleorder;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.HandleOrder
{
    [TestFixture]
    public class CloseOrderTest
    {
        [Test]
        public void TestCloseOrder()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.That(response.ResultCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);

            CloseOrderEuResponse closeResponse = WebpayConnection.CloseOrder(SveaConfig.GetDefaultConfig())
                                                                 .SetOrderId(response.CreateOrderResult.SveaOrderId)
                                                                 .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                 .CloseInvoiceOrder()
                                                                 .DoRequest();

            Assert.That(closeResponse.ResultCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
        }

        [Test]
        public void TestFailOnMissingCountryCodeOfCloseOrder()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            long orderId = response.CreateOrderResult.SveaOrderId;

            Assert.That(response.ResultCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);

            CloseOrder closeOrder = WebpayConnection.CloseOrder(SveaConfig.GetDefaultConfig())
                                                    .SetOrderId(orderId)
                                                    .CloseInvoiceOrder();

            const string expectedMsg = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";

            Assert.That(closeOrder.ValidateRequest(), Is.EqualTo(expectedMsg));
        }
    }
}