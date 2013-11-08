using NUnit.Framework;
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
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);

            CloseOrderEuResponse closeResponse = WebpayConnection.CloseOrder()
                                                                 .SetOrderId(response.CreateOrderResult.SveaOrderId)
                                                                 .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                 .CloseInvoiceOrder()
                                                                 .DoRequest();

            Assert.AreEqual(0, closeResponse.ResultCode);
            Assert.IsTrue(closeResponse.Accepted);
        }

        [Test]
        public void TestFailOnMissingCountryCodeOfCloseOrder()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);

            CloseOrder closeOrder = WebpayConnection.CloseOrder()
                                                    .SetOrderId(orderId)
                                                    .CloseInvoiceOrder();

            const string expectedMsg = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";

            Assert.AreEqual(expectedMsg, closeOrder.ValidateRequest());
        }
    }
}