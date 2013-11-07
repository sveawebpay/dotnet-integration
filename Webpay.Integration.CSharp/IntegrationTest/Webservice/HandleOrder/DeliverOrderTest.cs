using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;
using InvoiceDistributionType = Webpay.Integration.CSharp.Util.Constant.InvoiceDistributionType;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.HandleOrder
{
    [TestFixture]
    public class DeliverOrderTest
    {
        [Test]
        public void TestDeliverPaymentPlanOrderDoRequest()
        {
            DeliverOrderEuResponse response =
                WebpayConnection.DeliverOrder()
                                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                .SetOrderId(54086L)
                                .SetInvoiceDistributionType(InvoiceDistributionType.POST)
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .DeliverInvoiceOrder()
                                .DoRequest();

            Assert.AreEqual("An order with the provided id does not exist.", response.ErrorMessage);
        }
    }
}