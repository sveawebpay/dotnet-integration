using System.Linq;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.AdminWS;

namespace Webpay.Integration.CSharp.IntegrationTest
{
    [TestFixture]
    public class WebpayAdminIntegrationTest
    {

        /// WebPayAdmin.queryOrder() ---------------------------------------------------------------------------------------
        // .queryInvoiceOrder
        [Test]
        //[Ignore("target acceptance test for queryInvoiceOrder")]
        public void test_queryOrder_queryInvoiceOrder()
        {
            // create order
            Webpay.Integration.CSharp.Order.Create.CreateOrderBuilder createOrderBuilder = Webpay.Integration.CSharp.WebpayConnection.CreateOrder(
                SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetOrderDate(TestingTool.DefaultTestDate)
                .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                .SetCurrency(TestingTool.DefaultTestCurrency)
                ;
            Webpay.Integration.CSharp.WebpayWS.CreateOrderEuResponse order = createOrderBuilder.UseInvoicePayment().DoRequest();
            Assert.IsTrue(order.Accepted);

            // query order
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp.WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
            ;
            Webpay.Integration.CSharp.AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryInvoiceOrder().DoRequest();

            //Assert.IsTrue(answer.Accepted);
            Assert.That(order.CreateOrderResult.SveaOrderId, Is.EqualTo(answer.Orders.First().SveaOrderId));
        }

        // .queryPaymentPlanOrder
        //@Test
        //public void test_queryOrder_queryPaymentPlanOrder()

        // .queryCardOrder (uses webdriver)
        //@Test
        //public void test_queryOrder_queryCardOrder()

        // directbank
        //@Test
        //public void test_queryOrder_queryDirectBankOrder()
    }
}
