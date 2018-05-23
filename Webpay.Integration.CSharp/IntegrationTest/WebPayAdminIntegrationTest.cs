using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Testing;


namespace Webpay.Integration.CSharp.IntegrationTest
{
    [TestFixture]
    public class WebpayAdminIntegrationTest
    {
        private static PaymentResponse CreateCardOrderWithTwoOrderRows()
        {
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPaymentWithTwoRowsSpecifiedExVatAndVatPercent(PaymentMethod.SVEACARDPAY, customerRefNo));
            return payment;
        }
        private static PaymentResponse CreateCardOrderWithTwoOrderRowsSpecifiedIncVatAndVatPercent()
        {
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPaymentWithTwoRowsSpecifiedIncVatAndVatPercent(PaymentMethod.SVEACARDPAY, customerRefNo));
            return payment;
        }
        private static PaymentResponse CreateCardOrderWithTwoOrderRowsSpecifiedIncVatAndExVat()
        {
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPaymentWithTwoRowsSpecifiedIncVatAndExVat(PaymentMethod.SVEACARDPAY, customerRefNo));
            return payment;
        }

        // WebpayAdmin.QueryOrder() ---------------------------------------------------------------------------------------
        [Test] public void Test_QueryOrder_QueryInvoiceOrder()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.IsTrue(order.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryInvoiceOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(order.CreateOrderResult.SveaOrderId, Is.EqualTo(answer.Orders.First().SveaOrderId));
        }
        [Test] public void Test_QueryOrder_QueryPaymentPlanOrder()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(order.CreateOrderResult.SveaOrderId, Is.EqualTo(answer.Orders.First().SveaOrderId));
        }
        [Test] public void Test_QueryOrder_QueryCardOrder()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var payment = CreateCardOrderWithTwoOrderRows();

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.TransactionId, Is.EqualTo(payment.TransactionId));
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetName(), Is.EqualTo("Prod")); //SetName
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetAmountExVat(), Is.EqualTo(100M));
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetVatPercent(), Is.EqualTo(25));
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetAmountIncVat(), Is.EqualTo(125M));
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetDescription(), Is.EqualTo("Specification"));
            //SetDescription
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetQuantity(), Is.EqualTo(2)); //SetQuantity
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetArticleNumber(), Is.EqualTo("1"));
            //SetArticleNumber
            Assert.That(answer.Transaction.NumberedOrderRows.First().GetUnit(), Is.EqualTo("st")); //SetUnit
        }
        [Test] public void Test_QueryOrder_QueryDirectBankOrder()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var payment = CreateCardOrderWithTwoOrderRows();

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            //Webpay.Integration.CSharp.Hosted.Admin.HostedAdminResponse answer = queryOrderBuilder.QueryDirectBankOrder().DoRequest();
            QueryResponse answer = queryOrderBuilder.QueryDirectBankOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.TransactionId, Is.EqualTo(payment.TransactionId));
        }
        // WebpayAdmin.UpdateOrder()
        [Test] public void Test_UpdateOrder_UpdateInvoiceOrder()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // update order
            var clientOrderNumberText = "Updated clientOrderNumber";
            var notesText = "Updated notes";

            var updateBuilder = new UpdateOrderBuilder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)    
                .SetCountryCode(CountryCode.SE)
                .SetClientOrderNumber(clientOrderNumberText)
                .SetNotes(notesText)
            ;
            AdminWS.UpdateOrderResponse updateResponse = updateBuilder.UpdateInvoiceOrder().DoRequest();
            Assert.True(updateResponse.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryInvoiceOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Orders.FirstOrDefault().ClientOrderId, Is.EqualTo(clientOrderNumberText));
            Assert.That(answer.Orders.FirstOrDefault().Notes, Is.EqualTo(notesText));
        }
        [Test] public void Test_UpdateOrder_UpdateInvoiceOrder_TooLongClientOrderNumberReturnsError()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // update order
            var maxClientOrderNumberLength = 29; // should be 32, bug report filed 160226;
            var maxNotesLength = 200;

            var clientOrderNumberText = "Updated clientOrderNumber";
            var notesText = "Updated notes";
            var newClientOrderNumber = clientOrderNumberText.PadRight(maxClientOrderNumberLength +1, '.');
            var newNotes = notesText.PadRight(maxNotesLength +1, '.');

            var updateBuilder = new UpdateOrderBuilder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .SetClientOrderNumber(newClientOrderNumber)
                .SetNotes(newNotes)
            ;
            AdminWS.UpdateOrderResponse updateResponse = updateBuilder.UpdateInvoiceOrder().DoRequest();
            Assert.False(updateResponse.Accepted);
            Assert.That(updateResponse.ResultCode, Is.EqualTo(20035));
            Assert.That(updateResponse.ErrorMessage, Is.EqualTo("The field Notes can't contain more than 200 characters."));
        }
        [Test] public void Test_UpdateOrder_UpdatePaymentPlanOrder()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
            Assert.True(order.Accepted);
 
            // update order
            var clientOrderNumberText = "Updated clientOrderNumber";
            var notesText = "Updated notes";

            var updateBuilder = new UpdateOrderBuilder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .SetClientOrderNumber(clientOrderNumberText)
                .SetNotes(notesText) // will be ignored for paymentplan order
            ;
            AdminWS.UpdateOrderResponse updateResponse = updateBuilder.UpdatePaymentPlanOrder().DoRequest();
            Assert.True(updateResponse.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Orders.FirstOrDefault().ClientOrderId, Is.EqualTo(clientOrderNumberText));
            Assert.That(answer.Orders.FirstOrDefault().Notes, Is.Null); // i.e. no change compared with before the update order request
        }
        // WebpayAdmin.DeliverOrders()
        [Test] public void Test_DeliverOrders_DeliverInvoiceOrderRows_SetOrderIdAndSingleOrder()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            DeliverOrdersBuilder builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                //.SetOrderIds()                
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrders().DoRequest();
            Assert.IsTrue(delivery.Accepted);
            /*
            Assert.NotNull(delivery.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber);

            var referenceNumber = from dr in delivery.OrdersDelivered
                where dr.SveaOrderId.Equals(order.CreateOrderResult.SveaOrderId)
                select dr.DeliveryReferenceNumber;
            Assert.NotNull(referenceNumber);
            */
            Assert.NotNull( delivery.OrdersDelivered.Single(od => od.SveaOrderId == order.CreateOrderResult.SveaOrderId).DeliveryReferenceNumber );
        }
        [Test] public void Test_DeliverOrders_DeliverInvoiceOrderRows_WithSetOrderIdsAndMultipleOrders()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            var orderTwo = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            var orderIdsToDeliver = new List<long>
            {
                order.CreateOrderResult.SveaOrderId,
                orderTwo.CreateOrderResult.SveaOrderId
            };

            DeliverOrdersBuilder builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
                .SetOrderIds(orderIdsToDeliver)                
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrders().DoRequest();
            Assert.IsTrue(delivery.Accepted);
            Assert.That(delivery.OrdersDelivered.Select( od => orderIdsToDeliver.Contains(od.SveaOrderId) ).Count(), Is.EqualTo(2));
        }
        [Test] public void Test_DeliverOrders_DeliverInvoiceOrderRows_WithMixedSetOrderIdxAndMultipleOrders()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            var orderTwo = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            var orderThree = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            var orderIdsToDeliver = new List<long>
            {
                order.CreateOrderResult.SveaOrderId,
                orderTwo.CreateOrderResult.SveaOrderId,
            };

            DeliverOrdersBuilder builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
                .SetOrderIds(orderIdsToDeliver)
                .SetOrderId(orderThree.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrders().DoRequest();
            Assert.IsTrue(delivery.Accepted);

            var referenceNumbers = from od in delivery.OrdersDelivered
                                   where orderIdsToDeliver.Contains(od.SveaOrderId) || od.SveaOrderId == orderThree.CreateOrderResult.SveaOrderId
                                   select od.DeliveryReferenceNumber;
            Assert.That(referenceNumbers.Count(), Is.EqualTo(3));

            //Assert.That(delivery.OrdersDelivered.Select(od => orderIdsToDeliver.Contains(od.SveaOrderId) || od.SveaOrderId == orderThree.CreateOrderResult.SveaOrderId).Count, Is.EqualTo(3));
        }
        [Test] public void Test_DeliverOrders_DeliverInvoiceOrderRows_WithMixedOrderTypesFails()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            var orderTwo = TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
            var orderIdsToDeliver = new List<long>
            {
                order.CreateOrderResult.SveaOrderId,
                orderTwo.CreateOrderResult.SveaOrderId
            };

            DeliverOrdersBuilder builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
                .SetOrderIds(orderIdsToDeliver)
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrders().DoRequest();
            Assert.IsFalse(delivery.Accepted);
            Assert.That(delivery.ResultCode, Is.EqualTo(50000));    // will use the first order clientid for both, and orders belong to different clients...
            Assert.That(delivery.ErrorMessage, Is.EqualTo("Client is not authorized for this method."));
        }
        [Test] public void Test_DeliverOrders_DeliverPaymentPlanOrderRows_SetOrderIdAndSingleOrder()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();

            DeliverOrdersBuilder builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                //.SetOrderIds()                
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverPaymentPlanOrders().DoRequest();
            Assert.IsTrue(delivery.Accepted);

            Assert.NotNull(delivery.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber);
            Assert.NotNull(delivery.OrdersDelivered.Single(od => od.SveaOrderId == order.CreateOrderResult.SveaOrderId).DeliveryReferenceNumber);
        }       
        // WebpayAdmin.DeliverOrderRows()
        [Test] public void Test_DeliverOrderRows_DeliverInvoiceOrderRows()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            // deliver first order row and assert the response
            DeliverOrderRowsBuilder builder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetInvoiceDistributionType(DistributionType.POST) // TODO harmonise InvoiceDistributionType w/AdminWS?!
                .SetRowToDeliver(1)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrderRows().DoRequest();
            Assert.IsTrue(delivery.Accepted);
        }
        [Test] public void Test_DeliverOrderRows_DeliverCardOrderRows_Deliver_All_Rows()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var payment = CreateCardOrderWithTwoOrderRows();

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;

            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); //r1, r2: 100.00ex@25*2 => 500.00

            // deliver all order rows
            DeliverOrderRowsBuilder builder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetTransactionId((long) answer.TransactionId)
                // we've checked that answer was accepted, i.e. transactionId exists
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetRowToDeliver(1)
                .SetRowToDeliver(2)
                .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows)
                ;

            ConfirmResponse delivery = builder.DeliverCardOrderRows().DoRequest();
            Assert.IsTrue(delivery.Accepted);

            // query updated order
            CSharp.Order.Handle.QueryOrderBuilder queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(queryConfirmedOrderAnswer.Accepted);
            Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); //r1, r2: 100.00ex@25*2 => 500.00
        }
        [Test] public void Test_DeliverOrderRows_DeliverCardOrderRows_Deliver_First_Of_Two_Rows()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var payment = CreateCardOrderWithTwoOrderRows();

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); //r1, r2: 100.00ex@25*2 => 500.00

            // deliver all order rows
            DeliverOrderRowsBuilder builder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetTransactionId((long) answer.TransactionId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetRowToDeliver(1)
                .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows)
                ;
            ConfirmResponse delivery = builder.DeliverCardOrderRows().DoRequest();
            Assert.IsTrue(delivery.Accepted);

            // query updated order
            QueryOrderBuilder queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(queryConfirmedOrderAnswer.Accepted);
            Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("CONFIRMED"));
            Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.00M)); //r1, 100.00ex@25*2 => 250.00
        }
        // WebpayAdmin.CreditOrderRows()
        [Test] public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingSetRowToCredit()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            // deliver order
            var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetRowToDeliver(1)
                .SetRowToDeliver(2)
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                ;
            AdminWS.DeliveryResponse deliverResponse = deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
            Assert.IsTrue(deliverResponse.Accepted);

           // credit order rows
           CreditOrderRowsBuilder creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
                .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetCountryCode(CountryCode.SE)
                .SetRowToCredit(1)
                .SetRowToCredit(2)
            ;
            AdminWS.DeliveryResponse creditResponse = creditBuilder.CreditInvoiceOrderRows().DoRequest();
            Assert.IsTrue(creditResponse.Accepted);
        }
        [Test] public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddCreditOrderRow()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            // deliver order
            var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetRowToDeliver(1)
                .SetRowToDeliver(2)
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                ;
            AdminWS.DeliveryResponse deliverResponse = deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
            Assert.IsTrue(deliverResponse.Accepted);

            var newExVatCreditOrderRow = new OrderRowBuilder()
                .SetName("NewCreditOrderRow")
                .SetAmountExVat(8.0M)
                .SetVatPercent(25)
                .SetQuantity(1M)
            ;
            var newCreditOrderRows = new List<OrderRowBuilder>();
            newCreditOrderRows.Add(newExVatCreditOrderRow);

            // credit order rows
            CreditOrderRowsBuilder creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
                .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetCountryCode(CountryCode.SE)
                .AddCreditOrderRows(newCreditOrderRows)
            ;
            AdminWS.DeliveryResponse creditResponse = creditBuilder.CreditInvoiceOrderRows().DoRequest();
            Assert.IsTrue(creditResponse.Accepted);
        }
        [Test] public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddNewOrderRowAndMismatchedVatFlagSettingsFails()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            // deliver order
            var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetRowToDeliver(1)
                .SetRowToDeliver(2)
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                ;
            AdminWS.DeliveryResponse deliverResponse = deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
            Assert.IsTrue(deliverResponse.Accepted);

            var newIncVatCreditOrderRow = new OrderRowBuilder()
                .SetName("NewCreditOrderRow")
                .SetAmountIncVat(10.0M)
                .SetVatPercent(25)
                .SetQuantity(1M)
            ;
            var newCreditOrderRows = new List<OrderRowBuilder>();
            newCreditOrderRows.Add(newIncVatCreditOrderRow);

            // credit order rows
            CreditOrderRowsBuilder creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
                .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetCountryCode(CountryCode.SE)
                .AddCreditOrderRows( newCreditOrderRows )
            ;
            AdminWS.DeliveryResponse creditResponse = creditBuilder.CreditInvoiceOrderRows().DoRequest();
            Assert.IsFalse(creditResponse.Accepted);
            Assert.That(creditResponse.ResultCode, Is.EqualTo(50036));
            Assert.That(creditResponse.ErrorMessage, Is.EqualTo("The flag PriceIncludingVat must be used consistently for all order rows in the order."));
        }
        [Test] public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddNewOrderRowSpecifiedExVatAndIncVatSentAsIncVat()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            // deliver order
            var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetRowToDeliver(1)
                .SetRowToDeliver(2)
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                ;
            AdminWS.DeliveryResponse deliverResponse = deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
            Assert.IsTrue(deliverResponse.Accepted);

            var newExVatCreditOrderRow = new OrderRowBuilder()
                .SetName("NewCreditOrderRow")
                .SetAmountIncVat(10M)
                .SetAmountExVat(8.0M)
                //.SetVatPercent(25)
                .SetQuantity(1M)
            ;
            var newCreditOrderRows = new List<OrderRowBuilder>();
            newCreditOrderRows.Add(newExVatCreditOrderRow);

            // credit order rows
            CreditOrderRowsBuilder creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
                .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetCountryCode(CountryCode.SE)
                .AddCreditOrderRows(newCreditOrderRows)
            ;
            AdminWS.DeliveryResponse creditResponse = creditBuilder.CreditInvoiceOrderRows().DoRequest();
            Assert.IsFalse(creditResponse.Accepted);
            Assert.That(creditResponse.ResultCode, Is.EqualTo(50036));
            Assert.That(creditResponse.ErrorMessage, Is.EqualTo("The flag PriceIncludingVat must be used consistently for all order rows in the order."));
        }
        [Test] public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUndeliveredRowFails()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();

            // deliver order
            var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetRowToDeliver(1)
                //.SetRowToDeliver(2)
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                ;
            AdminWS.DeliveryResponse deliverResponse = deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
            Assert.IsTrue(deliverResponse.Accepted);

            // credit order rows
            CreditOrderRowsBuilder creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
                 .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
                 .SetInvoiceDistributionType(DistributionType.POST)
                 .SetCountryCode(CountryCode.SE)
                 .SetRowToCredit(1)
                 .SetRowToCredit(2)
             ;
            AdminWS.DeliveryResponse creditResponse = creditBuilder.CreditInvoiceOrderRows().DoRequest();
            Assert.IsFalse(creditResponse.Accepted);
            Assert.That(creditResponse.ResultCode, Is.EqualTo(20010));
            Assert.That(creditResponse.ErrorMessage, Is.EqualTo("All rows must belong to the invoice"));
        }
        [Test] public void Test_CreditOrderRows_CreditPaymentPlanOrderRows_Credit_UsingSetRowToCredit()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);

            // TODO copy create-deliver to WebpayConnectionIntegrationTest
            // deliver order
            var deliverBuilder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
                ;
            var deliverResponse = deliverBuilder.DeliverPaymentPlanOrder().DoRequest();
            Assert.IsTrue(deliverResponse.Accepted);

            // credit order rows
            CreditOrderRowsBuilder creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
                 .SetContractNumber(deliverResponse.DeliverOrderResult.PaymentPlanResultDetails.ContractNumber)
                 .SetCountryCode(CountryCode.SE)
                 .SetRowToCredit(1)
             ;
            AdminWS.CancelPaymentPlanRowsResponse creditResponse = creditBuilder.CreditPaymentPlanOrderRows().DoRequest();
            Assert.IsTrue(creditResponse.Accepted);
        }
        [Test] public void Test_CreditOrderRows_CreditPaymentPlanOrderRows_UsingAddCreditOrderRow()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);

            // TODO copy create-deliver to WebpayConnectionIntegrationTest
            // deliver order
            var deliverBuilder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
                ;
            var deliverResponse = deliverBuilder.DeliverPaymentPlanOrder().DoRequest();
            Assert.IsTrue(deliverResponse.Accepted);

            var newExVatCreditOrderRow = new OrderRowBuilder()
                .SetName("NewCreditOrderRow")
                .SetAmountExVat(8.0M)
                .SetVatPercent(25)
                .SetQuantity(1M)
            ;
            var newCreditOrderRows = new List<OrderRowBuilder>();
            newCreditOrderRows.Add(newExVatCreditOrderRow);

            // credit order rows
            CreditOrderRowsBuilder creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
                 .SetContractNumber(deliverResponse.DeliverOrderResult.PaymentPlanResultDetails.ContractNumber)
                 .SetCountryCode(CountryCode.SE)
                 .AddCreditOrderRows(newCreditOrderRows)
             ;
            AdminWS.CancelPaymentPlanRowsResponse creditResponse = creditBuilder.CreditPaymentPlanOrderRows().DoRequest();
            Assert.IsTrue(creditResponse.Accepted);
        }
        // WebpayAdmin.AddOrderRows()
        [Test] public void Test_AddOrderRows_AddInvoiceOrderRows_OriginalAndUpdatedOrdersSpecifiedExVat()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // add order rows
            //var firstOrderRowPriceIncVat = 80M;
            var firstOrderRowPriceExVat = 64M;
            var firstOrderRowName = "New row #1";
            var firstOrderRowDescription = "This should be the third order row!";

            var firstOrderRow = new OrderRowBuilder()
                //.SetAmountIncVat(firstOrderRowPriceIncVat)
                .SetAmountExVat(firstOrderRowPriceExVat)
                .SetVatPercent(25M)
                .SetQuantity(1M)
                .SetDiscountPercent(10)
                .SetName(firstOrderRowName)
                .SetDescription(firstOrderRowDescription)
                ;

            var secondOrderRowPriceExVat = 32M;
            var secondOrderRowName = "New row #2";
            var secondOrderRowDescription = "This should be the fourth order row!";

            var secondOrderRow = new OrderRowBuilder(firstOrderRow); // uses copy constructor
            secondOrderRow
                .SetAmountExVat(secondOrderRowPriceExVat)
                .SetName(secondOrderRowName)
                .SetDescription(secondOrderRowDescription)
            ;

            AddOrderRowsBuilder builder = WebpayAdmin.AddOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddOrderRow(firstOrderRow)
                .AddOrderRow(secondOrderRow)
                ;
            // then select the corresponding request class and send request
            AdminWS.AddOrderRowsResponse addition = builder.AddInvoiceOrderRows().DoRequest();
            Assert.True(addition.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryInvoiceOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.IsFalse((bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).PriceIncludingVat);   // row #3
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).PricePerUnit, Is.EqualTo(firstOrderRowPriceExVat));
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).Description, Is.EqualTo(firstOrderRowName + ": " + firstOrderRowDescription));
            Assert.IsFalse((bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).PriceIncludingVat);   // row #4
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).PricePerUnit, Is.EqualTo(secondOrderRowPriceExVat));
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).Description, Is.EqualTo(secondOrderRowName + ": " + secondOrderRowDescription));
        }
        [Test] public void Test_AddOrderRows_AddPaymentPlanOrderRows_OriginalAndUpdatedOrdersSpecifiedExVat()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // add order rows
            //var firstOrderRowPriceIncVat = 80M;
            var firstOrderRowPriceExVat = 64M;
            var firstOrderRowName = "New row #1";
            var firstOrderRowDescription = "This should be the third order row!";

            var firstOrderRow = new OrderRowBuilder()
                //.SetAmountIncVat(firstOrderRowPriceIncVat)
                .SetAmountExVat(firstOrderRowPriceExVat)
                .SetVatPercent(25M)
                .SetQuantity(1M)
                .SetDiscountPercent(10)
                .SetName(firstOrderRowName)
                .SetDescription(firstOrderRowDescription)
                ;

            var secondOrderRowPriceExVat = 32M;
            var secondOrderRowName = "New row #2";
            var secondOrderRowDescription = "This should be the fourth order row!";

            var secondOrderRow = new OrderRowBuilder(firstOrderRow); // uses copy constructor
            secondOrderRow
                .SetAmountExVat(secondOrderRowPriceExVat)
                .SetName(secondOrderRowName)
                .SetDescription(secondOrderRowDescription)
            ;

            AddOrderRowsBuilder builder = WebpayAdmin.AddOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddOrderRows( new List<OrderRowBuilder>() { firstOrderRow, secondOrderRow } )
                ;
            // then select the corresponding request class and send request
            AdminWS.AddOrderRowsResponse addition = builder.AddPaymentPlanOrderRows().DoRequest();
            Assert.True(addition.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.IsFalse((bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).PriceIncludingVat);   // row #3
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).PricePerUnit, Is.EqualTo(firstOrderRowPriceExVat));
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).Description, Is.EqualTo(firstOrderRowName + ": " + firstOrderRowDescription));
            Assert.IsFalse((bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).PriceIncludingVat);   // row #4
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).PricePerUnit, Is.EqualTo(secondOrderRowPriceExVat));
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).Description, Is.EqualTo(secondOrderRowName + ": " + secondOrderRowDescription));
        }
        // also check added rows specified w/differing PriceIncludingVatFlag are handled correctly
        [Test] public void Test_AddOrderRows_AddInvoiceOrderRows_OriginalAndUpdatedOrdersHasDifferentPriceIncludingVatFlagReturnsError()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // add order rows
            var firstOrderRowPriceIncVat = 80M;
            //var firstOrderRowPriceExVat = 64M;
            var firstOrderRowName = "New row #1";
            var firstOrderRowDescription = "This should be the third order row!";

            var firstOrderRow = new OrderRowBuilder()
                .SetAmountIncVat(firstOrderRowPriceIncVat)
                //.SetAmountExVat(firstOrderRowPriceExVat)
                .SetVatPercent(25M)
                .SetQuantity(1M)
                .SetDiscountPercent(10)
                .SetName(firstOrderRowName)
                .SetDescription(firstOrderRowDescription)
                ;

            var secondOrderRowPriceExVat = 32M;
            var secondOrderRowName = "New row #2";
            var secondOrderRowDescription = "This should be the fourth order row!";

            var secondOrderRow = new OrderRowBuilder(firstOrderRow); // uses copy constructor
            secondOrderRow
                .SetAmountExVat(secondOrderRowPriceExVat)
                .SetName(secondOrderRowName)
                .SetDescription(secondOrderRowDescription)
            ;

            AddOrderRowsBuilder builder = WebpayAdmin.AddOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddOrderRow(firstOrderRow)
                .AddOrderRow(secondOrderRow)
                ;
            // then select the corresponding request class and send request
            AdminWS.AddOrderRowsResponse addition = builder.AddInvoiceOrderRows().DoRequest();
            Assert.IsFalse(addition.Accepted);
            Assert.That(addition.ResultCode,Is.EqualTo(50036));
            Assert.That(addition.ErrorMessage,Is.EqualTo("The flag PriceIncludingVat must be used consistently for all order rows in the order."));
        }
        // WebpayAdmin.UpdateOrderRows()
        [Test] public void Test_UpdateOrderRows_UpdateInvoiceOrderRows_OriginalAndUpdatedOrdersSpecifiedExVat()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // update order
            var updatedOrderRowIndex = 2; // i.e. 2nd row, as NumberedOrderRows are 1-indexed.
            //var updatedOrderRowPriceIncVat = 80M;
            var updatedOrderRowPriceExVat = 64M;
            var updatedOrderRowName = "New row #1";
            var updatedOrderRowDescription = "Replaces second original order row!";

            var updatedOrderRow = new NumberedOrderRowBuilder()
                .SetRowNumber(updatedOrderRowIndex) 
                //.SetAmountIncVat(updatedOrderRowPriceIncVat)
                .SetAmountExVat(updatedOrderRowPriceExVat)
                .SetVatPercent(25M)
                .SetQuantity(1M)
                .SetDiscountPercent(10)
                .SetName(updatedOrderRowName)
                .SetDescription(updatedOrderRowDescription)
                ;

            UpdateOrderRowsBuilder update = WebpayAdmin.UpdateOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddUpdateOrderRow(updatedOrderRow)
                ;
            // then select the corresponding request class and send request
            AdminWS.UpdateOrderRowsResponse updateResponse = update.UpdateInvoiceOrderRows().DoRequest();
            Assert.True(updateResponse.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryInvoiceOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.IsFalse((bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PriceIncludingVat);   //
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex-1).PricePerUnit, Is.EqualTo(updatedOrderRowPriceExVat));
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex-1).Description, Is.EqualTo(updatedOrderRowName+": "+updatedOrderRowDescription));
        }
        [Test] public void Test_UpdateOrderRows_UpdateInvoiceOrderRows_OriginalAndUpdatedOrdersSpecifiedIncVat()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRowsSpecifiedIncVat();
            Assert.True(order.Accepted);

            // update order
            var updatedOrderRowIndex = 2; // i.e. 2nd row, as NumberedOrderRows are 1-indexed.
            var updatedOrderRowPriceIncVat = 80M;
            //var updatedOrderRowPriceExVat = 64M;
            var updatedOrderRowName = "New row #1";
            var updatedOrderRowDescription = "Replaces second original order row!";

            var updatedOrderRow = new NumberedOrderRowBuilder()
                .SetRowNumber(updatedOrderRowIndex)
                .SetAmountIncVat(updatedOrderRowPriceIncVat)
                //.SetAmountExVat(updatedOrderRowPriceExVat)
                .SetVatPercent(25M)
                .SetQuantity(1M)
                .SetDiscountPercent(10)
                .SetName(updatedOrderRowName)
                .SetDescription(updatedOrderRowDescription)
                ;

            UpdateOrderRowsBuilder update = WebpayAdmin.UpdateOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddUpdateOrderRow(updatedOrderRow)
                ;
            // then select the corresponding request class and send request
            AdminWS.UpdateOrderRowsResponse updateResponse = update.UpdateInvoiceOrderRows().DoRequest();
            Assert.True(updateResponse.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryInvoiceOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.IsTrue((bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PriceIncludingVat);
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PricePerUnit, Is.EqualTo(updatedOrderRowPriceIncVat));
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).Description, Is.EqualTo(updatedOrderRowName + ": " + updatedOrderRowDescription));
        }
        [Test] public void Test_UpdateOrderRows_UpdateInvoiceOrderRows_OriginalAndUpdatedOrdersHasDifferentPriceIncludingVatFlagFails()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRowsSpecifiedIncVat();
            Assert.True(order.Accepted);

            // update order
            var updatedOrderRowIndex = 2; // i.e. 2nd row, as NumberedOrderRows are 1-indexed.
            //var updatedOrderRowPriceIncVat = 80M;
            var updatedOrderRowPriceExVat = 64M;
            var updatedOrderRowName = "New row #1";
            var updatedOrderRowDescription = "Replaces second original order row!";

            var updatedOrderRow = new NumberedOrderRowBuilder()
                .SetRowNumber(updatedOrderRowIndex)
                //.SetAmountIncVat(updatedOrderRowPriceIncVat)
                .SetAmountExVat(updatedOrderRowPriceExVat)
                .SetVatPercent(25M)
                .SetQuantity(1M)
                .SetDiscountPercent(10)
                .SetName(updatedOrderRowName)
                .SetDescription(updatedOrderRowDescription)
                ;

            UpdateOrderRowsBuilder update = WebpayAdmin.UpdateOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddUpdateOrderRow(updatedOrderRow)
                ;
            // then select the corresponding request class and send request
            AdminWS.UpdateOrderRowsResponse updateResponse = update.UpdateInvoiceOrderRows().DoRequest();
            Assert.False(updateResponse.Accepted);
            Assert.That(updateResponse.ResultCode, Is.EqualTo(50036));
            Assert.That(updateResponse.ErrorMessage, Is.EqualTo("The flag PriceIncludingVat must be used consistently for all order rows in the order."));
        }
        [Test] public void Test_UpdateOrderRows_UpdatePaymentPlanOrderRows_OriginalAndUpdatedOrdersSpecifiedExVat()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // update order
            var updatedOrderRowIndex = 2; // i.e. 2nd row, as NumberedOrderRows are 1-indexed.
            //var updatedOrderRowPriceIncVat = 80M;
            var updatedOrderRowPriceExVat = 64M;
            var updatedOrderRowName = "New row #1";
            var updatedOrderRowDescription = "Replaces second original order row!";

            var updatedOrderRow = new NumberedOrderRowBuilder()
                .SetRowNumber(updatedOrderRowIndex)
                //.SetAmountIncVat(updatedOrderRowPriceIncVat)
                .SetAmountExVat(updatedOrderRowPriceExVat)
                .SetVatPercent(25M)
                .SetQuantity(1M)
                .SetDiscountPercent(10)
                .SetName(updatedOrderRowName)
                .SetDescription(updatedOrderRowDescription)
                ;

            UpdateOrderRowsBuilder update = WebpayAdmin.UpdateOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddUpdateOrderRow(updatedOrderRow)
                ;
            // then select the corresponding request class and send request
            AdminWS.UpdateOrderRowsResponse updateResponse = update.UpdatePaymentPlanOrderRows().DoRequest();
            Assert.True(updateResponse.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.IsFalse((bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PriceIncludingVat);   //
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PricePerUnit, Is.EqualTo(updatedOrderRowPriceExVat));
            Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).Description, Is.EqualTo(updatedOrderRowName + ": " + updatedOrderRowDescription));
        }
        // WebpayAdmin.CancelOrderRows()
        [Test] public void Test_CancelOrderRows_CancelInvoiceOrderRows_CancelAllRows()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithOneOrderRow();
            Assert.True(order.Accepted);

            // cancel order rows
            var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetRowToCancel(1)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.CancelOrderRowsResponse cancellationResponse = cancellation.CancelInvoiceOrderRows().DoRequest();
            Assert.IsTrue(cancellationResponse.Accepted);
        }
        [Test] public void Test_CancelOrderRows_CancelInvoiceOrderRows_CancelFirstOfTwoRows()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // cancel order rows
            var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetRowToCancel(1)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.CancelOrderRowsResponse cancellationResponse = cancellation.CancelInvoiceOrderRows().DoRequest();
            Assert.IsTrue(cancellationResponse.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryInvoiceOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Orders.First().OrderRows.ElementAt(0).Status, Is.EqualTo("Cancelled"));
            Assert.That(answer.Orders.First().OrderRows.ElementAt(1).Status, Is.EqualTo("NotDelivered"));
        }
        [Test] public void Test_CreditOrderRows_CreditInvoiceOrderRows_CancelDeliveredRowFails()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // deliver order
            DeliverOrdersBuilder builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrders().DoRequest();
            Assert.IsTrue(delivery.Accepted);

            // cancel order rows
            var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetRowToCancel(1)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.CancelOrderRowsResponse cancellationResponse = cancellation.CancelInvoiceOrderRows().DoRequest();
            Assert.IsFalse(cancellationResponse.Accepted);
            Assert.That(cancellationResponse.ResultCode, Is.EqualTo(20000));
            Assert.That(cancellationResponse.ErrorMessage, Is.EqualTo("Order is closed."));
        }
        [Test] public void Test_CancelOrderRows_CancelPaymentPlanOrderRows_CancelAllRows()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
            Assert.True(order.Accepted);

            // cancel order rows
            var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetRowToCancel(1)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.CancelOrderRowsResponse cancellationResponse = cancellation.CancelPaymentPlanOrderRows().DoRequest();
            Assert.IsTrue(cancellationResponse.Accepted);
        }
        [Test] public void Test_CancelOrderRows_CancelPaymentPlanOrderRows_CancelFirstOfTwoRows()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
            Assert.True(order.Accepted);

            // cancel order rows
            var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetRowToCancel(1)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.CancelOrderRowsResponse cancellationResponse = cancellation.CancelPaymentPlanOrderRows().DoRequest();
            Assert.IsTrue(cancellationResponse.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                ;
            AdminWS.GetOrdersResponse answer = queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Orders.First().OrderRows.ElementAt(0).Status, Is.EqualTo("Cancelled"));
            Assert.That(answer.Orders.First().OrderRows.ElementAt(1).Status, Is.EqualTo("NotDelivered"));
        }
        [Test] public void Test_CancelOrderRows_CancelCardOrderRows_CancellingAllRowsGivesOrderStatusAnnulled()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var payment = CreateCardOrderWithTwoOrderRows();
            // TODO fix Assert.IsTrue(payment.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;

            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); //r1, r2: 100.00ex@25*2 => 500.00

            // cancel all order rows
            CancelOrderRowsBuilder builder = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetTransactionId((long)answer.TransactionId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetRowToCancel(1)
                .SetRowToCancel(2)
                .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows)
                ;
            LowerAmountResponse cancellation = builder.CancelCardOrderRows().DoRequest();
            Assert.IsTrue(cancellation.Accepted);

            // query updated order
            CSharp.Order.Handle.QueryOrderBuilder queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(queryConfirmedOrderAnswer.Accepted);
            Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("ANNULLED"));
            Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(0.0M)); //r1, r2: 100.00ex@25*2 => 500.00
        }
        [Test] public void Test_CancelOrderRows_CancelCardOrderRows_CancellingFirstOfTwoRows()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var payment = CreateCardOrderWithTwoOrderRows();
            // TODO fix Assert.IsTrue(payment.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;

            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); //r1, r2: 100.00ex@25*2 => 500.00

            // cancel all order rows
            CancelOrderRowsBuilder builder = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetTransactionId((long)answer.TransactionId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetRowToCancel(1)
                //.SetRowToCancel(2)
                .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows)
                ;
            LowerAmountResponse cancellation = builder.CancelCardOrderRows().DoRequest();
            Assert.IsTrue(cancellation.Accepted);

            // query updated order
            CSharp.Order.Handle.QueryOrderBuilder queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(queryConfirmedOrderAnswer.Accepted);
            Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("AUTHORIZED"));
            Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.0M)); //r2: 100.00ex@25*2 => 250.00
        }        
        // also check card orders specified using exvat/incvat are handled correctly
        [Test] public void Test_CancelOrderRows_CancelCardOrderRows_CancellingFirstOfTwoRows_SpecifiedIncVatAndVatPercent()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var payment = CreateCardOrderWithTwoOrderRowsSpecifiedIncVatAndVatPercent();
            // TODO fix Assert.IsTrue(payment.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;

            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); //r1, r2: 100.00ex@25*2 => 500.00

            // cancel all order rows
            CancelOrderRowsBuilder builder = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetTransactionId((long)answer.TransactionId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetRowToCancel(1)
                //.SetRowToCancel(2)
                .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows)
                ;
            LowerAmountResponse cancellation = builder.CancelCardOrderRows().DoRequest();
            Assert.IsTrue(cancellation.Accepted);

            // query updated order
            CSharp.Order.Handle.QueryOrderBuilder queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(queryConfirmedOrderAnswer.Accepted);
            Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("AUTHORIZED"));
            Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.0M)); //r2: 100.00ex@25*2 => 250.00
        }
        [Test] public void Test_CancelOrderRows_CancelCardOrderRows_CancellingFirstOfTwoRows_SpecifiedIncVatAndExVat()
        {
            // create card order
            // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
            var payment = CreateCardOrderWithTwoOrderRowsSpecifiedIncVatAndExVat();
            // TODO fix Assert.IsTrue(payment.Accepted);

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;

            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); //r1, r2: 100.00ex@25*2 => 500.00

            // cancel all order rows
            CancelOrderRowsBuilder builder = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
                .SetTransactionId((long)answer.TransactionId)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetRowToCancel(1)
                //.SetRowToCancel(2)
                .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows)
                ;
            LowerAmountResponse cancellation = builder.CancelCardOrderRows().DoRequest();
            Assert.IsTrue(cancellation.Accepted);

            // query updated order
            CSharp.Order.Handle.QueryOrderBuilder queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(queryConfirmedOrderAnswer.Accepted);
            Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("AUTHORIZED"));
            Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.0M)); //r2: 100.00ex@25*2 => 250.00
        }
        // WebpayAdmin.CancelOrder()
        [Test] public void Test_CancelOrder_CancelInvoiceOrder()
        {
            // create order
            var order = TestingTool.CreateInvoiceOrderWithTwoOrderRows();
            Assert.IsTrue(order.Accepted);

            // do cancelOrder request and assert the response
            CancelOrderBuilder cancelOrderBuilder = WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId) 
                .SetCountryCode(CountryCode.SE)
            ;
            AdminWS.CancelOrderResponse cancellation = cancelOrderBuilder.CancelInvoiceOrder().DoRequest();
            Assert.IsTrue(cancellation.Accepted);
        }
        [Test] public void Test_CancelOrder_CancelPaymentPlanOrder()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);
           
            // do cancelOrder request and assert the response
            CancelOrderBuilder cancelOrderBuilder = WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
            ;
            AdminWS.CancelOrderResponse cancellation = cancelOrderBuilder.CancelPaymentPlanOrder().DoRequest();
            Assert.IsTrue(cancellation.Accepted);
        }
        [Test] public void Test_CancelOrder_CancelCardOrder()
        {
            // create order
            var payment = CreateCardOrderWithTwoOrderRows();

            // do cancelOrder request and assert the response
            CancelOrderBuilder cancelOrderBuilder = WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
            ;
            AnnulResponse cancellation = cancelOrderBuilder.CancelCardOrder().DoRequest();
            Assert.IsTrue(cancellation.Accepted);

            // query updated order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(payment.TransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            Assert.That(answer.Transaction.Status, Is.EqualTo("ANNULLED")); // TODO make enum w/Transaction statuses
        }
        // WebpayAdmin.CreditAmount()
        [Test] public void Test_CreditAmount_CreditPaymentPlanAmount()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);

            // deliver paymentplan
            var contract = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetCountryCode(CountryCode.SE)
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .DeliverPaymentPlanOrder().DoRequest()
                ;
            Assert.IsTrue(contract.Accepted);

            // credit amount
            CreditAmountBuilder creditAmountBuilder = WebpayAdmin.CreditAmount(SveaConfig.GetDefaultConfig())
                .SetContractNumber(contract.DeliverOrderResult.PaymentPlanResultDetails.ContractNumber)
                .SetCountryCode(CountryCode.SE)
                .SetDescription("test of credit amount")
                .SetAmountIncVat(100.00M)
                ;
            AdminWS.CancelPaymentPlanAmountResponse response = creditAmountBuilder.CreditPaymentPlanAmount().DoRequest();
            Assert.IsTrue(response.Accepted);            
        }
        [Test] public void Test_CreditAmount_CreditPaymentPlanAmount_CreditUndeliveredPaymentPlanFails()
        {
            // create order
            var order = TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);

            // credit amount
            CreditAmountBuilder creditAmountBuilder = WebpayAdmin.CreditAmount(SveaConfig.GetDefaultConfig())
                .SetContractNumber(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .SetDescription("test of credit amount")
                .SetAmountIncVat(100.00M)
                ;
            AdminWS.CancelPaymentPlanAmountResponse response = creditAmountBuilder.CreditPaymentPlanAmount().DoRequest();
            Assert.IsFalse(response.Accepted);
            Assert.That(response.ResultCode, Is.EqualTo(27006));
            Assert.That(response.ErrorMessage, Is.EqualTo("No paymentplan exists with the provided id."));

        }
        [Test, Ignore("")]
        public void Test_CreditAmount_CreditCardAmount()
        {
            // create order
            // use an existing captured order (status SUCCESS), as we can't do a
            // capture on an order via the webservice
            var capturedTransactionId = 625718L;

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(capturedTransactionId)
                .SetCountryCode(CountryCode.SE)
                ;

            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            var before = answer.Transaction.CreditedAmount;

            // credit amount
            var amountToCredit = 1.00M;
            CreditAmountBuilder creditAmountBuilder = WebpayAdmin.CreditAmount(SveaConfig.GetDefaultConfig())
                .SetContractNumber(capturedTransactionId)
                .SetCountryCode(CountryCode.SE)
                .SetDescription("test of credit amount")
                .SetAmountIncVat(amountToCredit)
                ;
            CreditResponse response = creditAmountBuilder.CreditCardAmount().DoRequest();
            Assert.IsTrue(response.Accepted);

            // query updated order
            QueryOrderBuilder queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(capturedTransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(queryConfirmedOrderAnswer.Accepted);
            var after = queryConfirmedOrderAnswer.Transaction.CreditedAmount;
            Assert.That(after, Is.EqualTo(before + amountToCredit));
        }
        [Test, Ignore("")]
        public void Test_CreditAmount_CreditDirectBankAmount()
        {
            // create order
            // use an existing captured order (status SUCCESS), as we can't do a
            // capture on an order via the webservice
            var capturedTransactionId = 590801L;

            // query order
            QueryOrderBuilder queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(capturedTransactionId)
                .SetCountryCode(CountryCode.SE)
                ;

            QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(answer.Accepted);
            var before = answer.Transaction.CreditedAmount;

            // credit amount
            var amountToCredit = 1.00M;
            CreditAmountBuilder creditAmountBuilder = WebpayAdmin.CreditAmount(SveaConfig.GetDefaultConfig())
                .SetContractNumber(capturedTransactionId)
                .SetCountryCode(CountryCode.SE)
                .SetDescription("test of credit amount")
                .SetAmountIncVat(amountToCredit)
                ;
            CreditResponse response = creditAmountBuilder.CreditDirectBankAmount().DoRequest();
            Assert.IsTrue(response.Accepted);

            // query updated order
            QueryOrderBuilder queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
                .SetTransactionId(capturedTransactionId)
                .SetCountryCode(CountryCode.SE)
                ;
            QueryResponse queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
            Assert.IsTrue(queryConfirmedOrderAnswer.Accepted);
            var after = queryConfirmedOrderAnswer.Transaction.CreditedAmount;
            Assert.That(after, Is.EqualTo(before + amountToCredit));
        }
   }
}