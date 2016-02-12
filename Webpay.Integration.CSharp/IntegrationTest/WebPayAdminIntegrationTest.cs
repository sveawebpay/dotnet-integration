using System;
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
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin;
namespace Webpay.Integration.CSharp.IntegrationTest
{
    [TestFixture]
    public class WebpayAdminIntegrationTest
    {

        /// WebPayAdmin.queryOrder() ---------------------------------------------------------------------------------------
        // .queryInvoiceOrder
        [Test]
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

            Assert.IsTrue(answer.Accepted);
            Assert.That(order.CreateOrderResult.SveaOrderId, Is.EqualTo(answer.Orders.First().SveaOrderId));
        }

        // .queryPaymentPlanOrder
        [Test]
        public void test_queryOrder_queryPaymentPlanOrder()
        {
            var campaigns = WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DoRequest();

            Webpay.Integration.CSharp.Order.Create.CreateOrderBuilder createOrderBuilder = Webpay.Integration.CSharp
                .WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetOrderDate(TestingTool.DefaultTestDate)
                .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                .SetCurrency(TestingTool.DefaultTestCurrency)
                ;
            Webpay.Integration.CSharp.WebpayWS.CreateOrderEuResponse order =
                createOrderBuilder.UsePaymentPlanPayment(campaigns.CampaignCodes[0].CampaignCode).DoRequest();
            Assert.IsTrue(order.Accepted);

            // query order
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp
                .WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            Webpay.Integration.CSharp.AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();

            //Assert.IsTrue(answer.Accepted);
            Assert.That(order.CreateOrderResult.SveaOrderId, Is.EqualTo(answer.Orders.First().SveaOrderId));
        }
        
        // .queryCardOrder
        //@Test
        //public void test_queryOrder_queryCardOrder()
        [Test]
        public void test_queryOrder_queryCardOrder()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPayment(PaymentMethod.SVEACARDPAY, customerRefNo));

            // query order
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp.WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
            ;
            Webpay.Integration.CSharp.Hosted.Admin.HostedAdminResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();

            Assert.That(answer.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
            var transactionid = Convert.ToInt64(answer.MessageXmlDocument.SelectSingleNode("/response/transaction").Attributes["id"].Value);
            Assert.That(transactionid, Is.EqualTo(payment.TransactionId));
        }
        // directbank
        [Test]
        public void test_queryOrder_queryDirectBankOrder()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPayment(PaymentMethod.NORDEASE, customerRefNo));

            // query order
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp.WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
            ;
            Webpay.Integration.CSharp.Hosted.Admin.HostedAdminResponse answer = queryOrderBuilder.QueryDirectBankOrder().DoRequest();

            Assert.That(answer.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
            var transactionid = Convert.ToInt64(answer.MessageXmlDocument.SelectSingleNode("/response/transaction").Attributes["id"].Value);
            Assert.That(transactionid, Is.EqualTo(payment.TransactionId));
        }
    }
}
