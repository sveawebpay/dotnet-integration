using System;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.IntegrationTest
{
    [TestFixture]
    public class WebpayConnectionIntegrationTest
    {
        // WebpayConnection.CreateOrder()
        [Test] public void Test_CreateOrder_CreateInvoiceOrder_WithAllExVatRows()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.That(order.Accepted);
        }
        [Test] public void Test_CreateOrder_CreatePaymentPlanOrder_WithAllExVatRows()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
            Assert.That(order.Accepted);
        }
        // WebpayConnection.DeliverOrder()
        [Test] public void Test_DeliverOrder_DeliverInvoiceOrder_WithAllIdenticalRows()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            // deliver first order row and assert the response
            DeliverOrderBuilder builder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetNumberOfCreditDays(30)
                .SetCaptureDate(DateTime.Now)
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                ;
            var delivery = builder.DeliverInvoiceOrder().DoRequest();
            Assert.That(delivery.Accepted);
            Assert.That(delivery.DeliverOrderResult.Amount, Is.EqualTo(500.00M)); // 100ex@25%*2st *2rows
        }
        [Test] public void Test_DeliverOrder_DeliverInvoiceOrder_WithOneIdenticalRows()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            // deliver first order row and assert the response
            DeliverOrderBuilder builder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetNumberOfCreditDays(30)
                .SetCaptureDate(DateTime.Now)
                //.AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                ;
            var delivery = builder.DeliverInvoiceOrder().DoRequest();
            Assert.That(delivery.Accepted);
            Assert.That(delivery.DeliverOrderResult.Amount, Is.EqualTo(250.00M)); // 100ex@25%*2st *1row
        }
        [Test] public void Test_DeliverOrder_DeliverPaymentPlanOrder_WithAllIdenticalRows()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();

            // deliver first order row and assert the response
            DeliverOrderBuilder builder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                ;
            var delivery = builder.DeliverPaymentPlanOrder().DoRequest();
            Assert.That(delivery.Accepted);
            Assert.That(delivery.DeliverOrderResult.Amount, Is.EqualTo(5000.00M)); // 1000ex@25%*2st *2rows
        }
        [Test] public void Test_DeliverOrder_DeliverPaymentPlanOrder_IgnoresOrderRows()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();

            // deliver first order row and assert the response
            DeliverOrderBuilder builder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                //.AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                ;
            var delivery = builder.DeliverPaymentPlanOrder().DoRequest();
            Assert.That(delivery.Accepted);
            Assert.That(delivery.DeliverOrderResult.Amount, Is.EqualTo(5000.00M)); // 1000ex@25%*2st *2row
        }
    }
}