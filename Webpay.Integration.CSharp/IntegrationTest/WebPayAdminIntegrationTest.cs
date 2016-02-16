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
        private static CreateOrderEuResponse CreateInvoiceOrderWithTwoOrderRows()
        {
            CreateOrderBuilder createOrderBuilder = Webpay.Integration.CSharp.
                WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            return order;
        }

        /// WebPayAdmin.queryOrder() ---------------------------------------------------------------------------------------
        // .queryInvoiceOrder
        [Test]
        public void test_queryOrder_queryInvoiceOrder()
        {
            // create order
            Webpay.Integration.CSharp.Order.Create.CreateOrderBuilder createOrderBuilder = Webpay.Integration.CSharp.
                WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetOrderDate(TestingTool.DefaultTestDate)
                .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                .SetCurrency(TestingTool.DefaultTestCurrency)
                ;
            Webpay.Integration.CSharp.WebpayWS.CreateOrderEuResponse order =
                createOrderBuilder.UseInvoicePayment().DoRequest();
            Assert.IsTrue(order.Accepted);

            // query order
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp.
                WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
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
            // get campaigns
            var campaigns = WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DoRequest();

            // create order
            Webpay.Integration.CSharp.Order.Create.CreateOrderBuilder createOrderBuilder = Webpay.Integration.CSharp.
                WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp.
                WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            Webpay.Integration.CSharp.AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();

            Assert.IsTrue(answer.Accepted);
            Assert.That(order.CreateOrderResult.SveaOrderId, Is.EqualTo(answer.Orders.First().SveaOrderId));
        }

        // .queryCardOrder
        [Test]
        public void test_queryOrder_queryCardOrder()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPayment(PaymentMethod.SVEACARDPAY, customerRefNo));

            // query order
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp.
                WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            Webpay.Integration.CSharp.Hosted.Admin.Actions.QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();

            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.TransactionId, Is.EqualTo(payment.TransactionId));
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetName(), Is.EqualTo("Prod"));                     //SetName
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetAmountExVat(), Is.EqualTo(100M));
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetVatPercent(), Is.EqualTo(25));
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetAmountIncVat(), Is.EqualTo(125M));
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetDescription(), Is.EqualTo("Specification"));     //SetDescription
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetQuantity(), Is.EqualTo(2));                      //SetQuantity
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetArticleNumber(), Is.EqualTo("1"));               //SetArticleNumber
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetUnit(), Is.EqualTo("st"));                       //SetUnit
        }

        // .queryDirectBankOrder
        [Test]
        public void test_queryOrder_queryDirectBankOrder()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPayment(PaymentMethod.NORDEASE, customerRefNo));

            // query order
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp.
            WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE)
            ;
            //Webpay.Integration.CSharp.Hosted.Admin.HostedAdminResponse answer = queryOrderBuilder.QueryDirectBankOrder().DoRequest();
            Webpay.Integration.CSharp.Hosted.Admin.Actions.QueryResponse answer = queryOrderBuilder.QueryDirectBankOrder().DoRequest();

            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.TransactionId, Is.EqualTo(payment.TransactionId));
        }


        // / WebPayAdmin.deliverOrderRows()
        // ---------------------------------------------------------------------------------
        // .deliverInvoiceOrderRows
        [Test]
        public void test_deliverOrderRows_deliverInvoiceOrderRows()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();

            // deliver first order row and assert the response
            Webpay.Integration.CSharp.Order.Handle.DeliverOrderRowsBuilder builder = Webpay.Integration.CSharp.
                WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetInvoiceDistributionType(DistributionType.POST) // TODO harmonise InvoiceDistributionType w/AdminWS?!
                .SetRowToDeliver(1)
                ;
            Webpay.Integration.CSharp.AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrderRows().DoRequest();
            Assert.IsTrue(delivery.Accepted);
        }

        //// .deliverCardOrderRows
        [Test]
        public void test_deliverOrderRows_deliverCardOrderRows_deliver_all_rows()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPaymentWithTwoRows(PaymentMethod.SVEACARDPAY, customerRefNo));

            // query order
            Webpay.Integration.CSharp.Order.Handle.QueryOrderBuilder queryOrderBuilder = Webpay.Integration.CSharp.
                WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;

            Webpay.Integration.CSharp.Hosted.Admin.Actions.QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);

            // deliver all order rows
            Webpay.Integration.CSharp.Order.Handle.DeliverOrderRowsBuilder builder = Webpay.Integration.CSharp.
                WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetTransactionId(answer.TransactionId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetRowToDeliver(1)
                .SetRowToDeliver(2)
                .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows)
                ;

            Webpay.Integration.CSharp.Hosted.Admin.Actions.ConfirmResponse delivery = builder.DeliverCardOrderRows().DoRequest();
            Assert.IsTrue(delivery.Accepted);
            // TODO check that amount is correct for entire order
        }

        //public void test_deliverOrderRows_deliverCardOrderRows_deliver_first_of_two_rows()
        // check that amount is correct for entire order
    }
}
