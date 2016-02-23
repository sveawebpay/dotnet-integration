using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin;
using Webpay.Integration.CSharp.Order.Handle;
using static Webpay.Integration.CSharp.Util.Testing.TestingTool;

namespace Webpay.Integration.CSharp.IntegrationTest
{
    [TestFixture]
    public class WebpayAdminIntegrationTest
    {
        private static PaymentResponse CreateCardOrderWithTwoOrderRows()
        {
            var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
            var payment = HostedAdminTest.MakePreparedPayment(HostedAdminTest.PrepareRegularPaymentWithTwoRows(PaymentMethod.SVEACARDPAY, customerRefNo));
            return payment;
        }

        /// WebPayAdmin.queryOrder() ---------------------------------------------------------------------------------------
        // .queryInvoiceOrder
        [Test]
        public void Test_QueryOrder_QueryInvoiceOrder()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();
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

        // .queryPaymentPlanOrder
        [Test]
        public void Test_QueryOrder_QueryPaymentPlanOrder()
        {
            // create order
            var order = CreatePaymentPlanOrderWithOneOrderRow();
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

        // .queryCardOrder
        [Test]
        public void Test_QueryOrder_QueryCardOrder()
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

        // .queryDirectBankOrder
        [Test]
        public void Test_QueryOrder_QueryDirectBankOrder()
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


        ///  DeliverOrdersBuilder request = WebpayAdmin.DeliverOrders(config)
        ///   .SetOrderId()                 // optional, order id to deliver
        ///   .SetOrderIds()                // optional, note that order ids must all be of the same type (invoice, part payment) 
        ///   .SetCountryCode()             // required
        ///   .SetInvoiceDistributionType() // required for invoice only, will apply to all invoice orders
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.DeliverInvoiceOrders().DoRequest();     // returns AdminWS.DeliveryResponse
        ///  response = request.DeliverPaymentPlanOrders().DoRequest();  // returns AdminWS.DeliveryResponse

        // / WebPayAdmin.deliverOrder()
        // ---------------------------------------------------------------------------------
        // .deliverInvoiceOrder
        [Test]
        public void Test_DeliverOrders_DeliverInvoiceOrderRows_SetOrderIdAndSingleOrder()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();

            DeliverOrdersBuilder builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                //.SetOrderIds()                
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrders().DoRequest();
            Assert.IsTrue(delivery.Accepted);

            Assert.NotNull(delivery.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber);

            var referenceNumber = from dr in delivery.OrdersDelivered
                where dr.SveaOrderId.Equals(order.CreateOrderResult.SveaOrderId)
                select dr.DeliveryReferenceNumber;
            Assert.NotNull(referenceNumber);

            Assert.NotNull( delivery.OrdersDelivered.Single(od => od.SveaOrderId == order.CreateOrderResult.SveaOrderId).DeliveryReferenceNumber );
        }

        [Test]
        public void Test_DeliverOrders_DeliverInvoiceOrderRows_WithSetOrderIdsAndMultipleOrders()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();
            var orderTwo = CreateInvoiceOrderWithTwoOrderRows();
            var orderIdsToDeliver = new List<long>
            {
                order.CreateOrderResult.SveaOrderId,
                orderTwo.CreateOrderResult.SveaOrderId
            };

            DeliverOrdersBuilder builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
                //.SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetOrderIds(orderIdsToDeliver)                
                .SetCountryCode(CountryCode.SE)
                .SetInvoiceDistributionType(DistributionType.POST)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrders().DoRequest();
            Assert.IsTrue(delivery.Accepted);

            var referenceNumbers = from od in delivery.OrdersDelivered
                                  where orderIdsToDeliver.Contains(od.SveaOrderId)
                                  select od.DeliveryReferenceNumber;
            Assert.That(referenceNumbers.Count(), Is.EqualTo(2));

            Assert.That(delivery.OrdersDelivered.Select( od => orderIdsToDeliver.Contains(od.SveaOrderId) ).Count, Is.EqualTo(2));
        }

        [Test]
        public void Test_DeliverOrders_DeliverInvoiceOrderRows_WithMixedSetOrderIdxAndMultipleOrders()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();
            var orderTwo = CreateInvoiceOrderWithTwoOrderRows();
            var orderThree = CreateInvoiceOrderWithTwoOrderRows();
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

            Assert.That(delivery.OrdersDelivered.Select(od => orderIdsToDeliver.Contains(od.SveaOrderId) || od.SveaOrderId == orderThree.CreateOrderResult.SveaOrderId).Count, Is.EqualTo(3));
        }



        // / WebPayAdmin.deliverOrderRows()
        // ---------------------------------------------------------------------------------
        // .deliverInvoiceOrderRows
        [Test]
        public void Test_DeliverOrderRows_DeliverInvoiceOrderRows()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();

            // deliver first order row and assert the response
            DeliverOrderRowsBuilder builder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(DefaultTestCountryCode)
                .SetInvoiceDistributionType(DistributionType.POST) // TODO harmonise InvoiceDistributionType w/AdminWS?!
                .SetRowToDeliver(1)
                ;
            AdminWS.DeliveryResponse delivery = builder.DeliverInvoiceOrderRows().DoRequest();
            Assert.IsTrue(delivery.Accepted);
        }

        // .deliverCardOrderRows
        [Test]
        public void Test_DeliverOrderRows_DeliverCardOrderRows_Deliver_All_Rows()
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
                .SetCountryCode(DefaultTestCountryCode)
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

        //public void test_deliverOrderRows_deliverCardOrderRows_deliver_first_of_two_rows()
        [Test]
        public void Test_DeliverOrderRows_DeliverCardOrderRows_Deliver_First_Of_Two_Rows()
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
                .SetCountryCode(DefaultTestCountryCode)
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
            Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.00M)); //r1, 100.00ex@25*2 => 250.00
        }


        // / WebpayAdmin.CreditOrderRows()
        // ---------------------------------------------------------------------------------
        // .CreditInvoiceOrderRows
        [Test]
        public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingSetRowToCredit()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();

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

        [Test]
        public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddCreditOrderRow()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();

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

        [Test]
        public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddNewOrderRowAndMismatchedVatFlagSettingsFails()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();

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

        [Test]
        public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddNewOrderRowSpecifiedExVatAndIncVatSentAsIncVat()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();

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

        [Test]
        public void Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUndeliveredRowFails()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();

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

        // .CreditPaymentPlanOrderRows
        [Test]
        public void Test_CreditOrderRows_CreditPaymentPlanOrderRows_Credit_UsingSetRowToCredit()
        {
            // create order
            var order = CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);

            // TODO copy create-deliver to WebpayConnectionIntegrationTest
            // deliver order
            var deliverBuilder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddOrderRow(CreatePaymentPlanOrderRow())
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

        [Test]
        public void Test_CreditOrderRows_CreditPaymentPlanOrderRows_UsingAddCreditOrderRow()
        {
            // create order
            var order = CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);

            // TODO copy create-deliver to WebpayConnectionIntegrationTest
            // deliver order
            var deliverBuilder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
                .AddOrderRow(CreatePaymentPlanOrderRow())
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

        // / WebPayAdmin.cancelOrder()
        // --------------------------------------------------------------------------------------
        // .cancelInvoiceOrder
        [Test]
        public void Test_CancelOrder_CancelInvoiceOrder()
        {
            // create order
            var order = CreateInvoiceOrderWithTwoOrderRows();
            Assert.IsTrue(order.Accepted);

            // do cancelOrder request and assert the response
            CancelOrderBuilder cancelOrderBuilder = WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId) 
                .SetCountryCode(CountryCode.SE)
            ;
            AdminWS.CancelOrderResponse cancellation = cancelOrderBuilder.CancelInvoiceOrder().DoRequest();
            Assert.IsTrue(cancellation.Accepted);
        }

        // .cancelPaymentPlanOrder
        [Test]
        public void Test_CancelOrder_CancelPaymentPlanOrder()
        {
            // create order
            var order = CreatePaymentPlanOrderWithOneOrderRow();
            Assert.IsTrue(order.Accepted);
           
            // do cancelOrder request and assert the response
            CancelOrderBuilder cancelOrderBuilder = WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
                .SetOrderId(order.CreateOrderResult.SveaOrderId)
                .SetCountryCode(CountryCode.SE)
            ;
            AdminWS.CancelOrderResponse cancellation = cancelOrderBuilder.CancelPaymentPlanOrder().DoRequest();
            Assert.IsTrue(cancellation.Accepted);
        }

        // .cancelCardOrder
        [Test]
        public void Test_CancelOrder_CancelCardOrder()
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

        // / WebpayAdmin.CreditAmount()
        // --------------------------------------------------------------------------------------
        // .CreditPaymentPlanAmount
        [Test]
        public void Test_CreditAmount_CreditPaymentPlanAmount()
        {
            // create order
            var order = CreatePaymentPlanOrderWithOneOrderRow();
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

        [Test]
        public void Test_CreditAmount_CreditPaymentPlanAmount_CreditUndeliveredPaymentPlanFails()
        {
            // create order
            var order = CreatePaymentPlanOrderWithOneOrderRow();
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

        // .CreditCardAmount
        [Test]
        public void Test_CreditAmount_CreditCardAmount()
        {
            // create order
            // use an existing captured order (status SUCCESS), as we can't do a
            // capture on an order via the webservice
            var capturedTransactionId = 610262L;

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

        // .CreditDirectBankAmount
        [Test]
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