using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Test.Util;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Test.Webservice.Handleorder
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
    }
}