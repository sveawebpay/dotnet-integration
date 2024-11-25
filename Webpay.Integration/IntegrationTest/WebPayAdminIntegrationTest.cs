using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.IntegrationTest.Hosted.Admin;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.IntegrationTest;

[TestFixture]
public class WebpayAdminIntegrationTest
{
    private static async Task<PaymentResponse> CreateCardOrderWithTwoOrderRows()
    {
        var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
        var payment = await HostedAdminTest.MakePreparedPayment(
            HostedAdminTest.PrepareRegularPaymentWithTwoRowsSpecifiedExVatAndVatPercent(PaymentMethod.SVEACARDPAY, customerRefNo));
        return payment;
    }

    private static async Task<PaymentResponse> CreateCardOrderWithTwoOrderRowsSpecifiedIncVatAndVatPercent()
    {
        var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
        var payment = await HostedAdminTest.MakePreparedPayment(
            HostedAdminTest.PrepareRegularPaymentWithTwoRowsSpecifiedIncVatAndVatPercent(PaymentMethod.SVEACARDPAY, customerRefNo));
        return payment;
    }

    private static async Task<PaymentResponse> CreateCardOrderWithTwoOrderRowsSpecifiedIncVatAndExVat()
    {
        var customerRefNo = HostedAdminTest.CreateCustomerRefNo();
        var payment = await HostedAdminTest.MakePreparedPayment(
            HostedAdminTest.PrepareRegularPaymentWithTwoRowsSpecifiedIncVatAndExVat(PaymentMethod.SVEACARDPAY, customerRefNo));
        return payment;
    }

    [Test]
    public async Task Test_UpdateOrder_UpdateInvoiceOrder()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var clientOrderNumberText = "Updated clientOrderNumber";
        var notesText = "Updated notes";

        var updateBuilder = new UpdateOrderBuilder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetClientOrderNumber(clientOrderNumberText)
            .SetNotes(notesText);

        var updateResponse = await updateBuilder.UpdateInvoiceOrder().DoRequest();
        Assert.That(updateResponse.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryInvoiceOrder().DoRequest();

        Assert.That(answer.ResultCode == 0);
        Assert.That(answer.Orders.FirstOrDefault().ClientOrderId, Is.EqualTo(clientOrderNumberText));
        Assert.That(answer.Orders.FirstOrDefault().Notes, Is.EqualTo(notesText));
    }

    [Test]
    public async Task Test_UpdateOrder_UpdateInvoiceOrder_TooLongClientOrderNumberReturnsError()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var maxClientOrderNumberLength = 29;
        var maxNotesLength = 200;

        var clientOrderNumberText = "Updated clientOrderNumber";
        var notesText = "Updated notes";
        var newClientOrderNumber = clientOrderNumberText.PadRight(maxClientOrderNumberLength + 1, '.');
        var newNotes = notesText.PadRight(maxNotesLength + 1, '.');

        var updateBuilder = new UpdateOrderBuilder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetClientOrderNumber(newClientOrderNumber)
            .SetNotes(newNotes);

        var updateResponse = await updateBuilder.UpdateInvoiceOrder().DoRequest();
        Assert.That(updateResponse.ResultCode != 0);
        Assert.That(updateResponse.ResultCode, Is.EqualTo(20035));
        Assert.That(updateResponse.ErrorMessage, Is.EqualTo("The field Notes can't contain more than 200 characters."));
    }

    [Test]
    public async Task Test_UpdateOrder_UpdatePaymentPlanOrder()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var clientOrderNumberText = "Updated clientOrderNumber";
        var notesText = "Updated notes";

        var updateBuilder = new UpdateOrderBuilder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetClientOrderNumber(clientOrderNumberText)
            .SetNotes(notesText); // Will be ignored for paymentplan order

        var updateResponse = await updateBuilder.UpdatePaymentPlanOrder().DoRequest();
        Assert.That(updateResponse.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
        Assert.That(answer.ResultCode == 0);
        Assert.That(answer.Orders.FirstOrDefault().ClientOrderId, Is.EqualTo(clientOrderNumberText));
        Assert.That(answer.Orders.FirstOrDefault().Notes, Is.Null); // No change compared with before the update order request
    }

    [Test]
    public async Task Test_QueryOrder_QueryInvoiceOrder()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryInvoiceOrder().DoRequest();
        Assert.That(answer.ResultCode == 0);
        Assert.That(order.CreateOrderResult.SveaOrderId, Is.EqualTo(answer.Orders.First().SveaOrderId));
    }

    [Test]
    public async Task Test_QueryOrder_QueryPaymentPlanOrder()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
        Assert.That(order.Accepted);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
        Assert.That(answer.ResultCode == 0);
        Assert.That(order.CreateOrderResult.SveaOrderId, Is.EqualTo(answer.Orders.First().SveaOrderId));
    }

    // TODO
    //[Test]
    //public async Task Test_QueryOrder_QueryCardOrder()
    //{
    //    // create card order
    //    var payment = CreateCardOrderWithTwoOrderRows();

    //    // query order
    //    var queryOrderBuilder = await WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
    //        .SetOrderId(payment.TransactionId)
    //        .SetCountryCode(CountryCode.SE);

    //    //QueryResponse answer = await queryOrderBuilder.QueryCardOrder().DoRequest();
    //    QueryResponse answer = queryOrderBuilder.QueryCardOrder().DoRequest();
    //    Assert.That(answer.Accepted);
    //    Assert.That(answer.TransactionId, Is.EqualTo(payment.TransactionId));
    //    Assert.That(answer.Transaction.NumberedOrderRows.First().GetName(), Is.EqualTo("Prod"));
    //    Assert.That(answer.Transaction.NumberedOrderRows.First().GetAmountExVat(), Is.EqualTo(100M));
    //    Assert.That(answer.Transaction.NumberedOrderRows.First().GetVatPercent(), Is.EqualTo(25));
    //    Assert.That(answer.Transaction.NumberedOrderRows.First().GetAmountIncVat(), Is.EqualTo(125M));
    //    Assert.That(answer.Transaction.NumberedOrderRows.First().GetDescription(), Is.EqualTo("Specification"));
    //    Assert.That(answer.Transaction.NumberedOrderRows.First().GetQuantity(), Is.EqualTo(2));
    //    Assert.That(answer.Transaction.NumberedOrderRows.First().GetArticleNumber(), Is.EqualTo("1"));
    //    Assert.That(answer.Transaction.NumberedOrderRows.First().GetUnit(), Is.EqualTo("st"));
    //}

    // TODO
    //[Test]
    //public async Task Test_QueryOrder_QueryDirectBankOrder()
    //{
    //    // create card order
    //    // TODO change to use CreateOrder().UseCardPayment.GetPaymentUrl() to set up test
    //    var payment = CreateCardOrderWithTwoOrderRows();

    //    // query order
    //    var queryOrderBuilder = await WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
    //        .SetTransactionId(payment.TransactionId)
    //        .SetCountryCode(CountryCode.SE);
    //    //Webpay.Integration.Hosted.Admin.HostedAdminResponse answer = queryOrderBuilder.QueryDirectBankOrder().DoRequest();
    //    //QueryResponse answer = await queryOrderBuilder.QueryDirectBankOrder().DoRequest();
    //    QueryResponse answer = queryOrderBuilder.QueryDirectBankOrder().DoRequest();
    //    Assert.That(answer.Accepted);
    //    Assert.That(answer.TransactionId, Is.EqualTo(payment.TransactionId));
    //}

    [Test]
    public async Task Test_DeliverOrders_DeliverInvoiceOrderRows_SetOrderIdAndSingleOrder()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST);

        var delivery = await builder.DeliverInvoiceOrders().DoRequest();

        Assert.That(delivery.ResultCode == 0);
        Assert.That(delivery.OrdersDelivered.Single(od => od.SveaOrderId == order.CreateOrderResult.SveaOrderId).DeliveryReferenceNumber != null);
    }

    [Test]
    public async Task Test_DeliverOrders_DeliverInvoiceOrderRows_WithSetOrderIdsAndMultipleOrders()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        var orderTwo = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var orderIdsToDeliver = new List<long> { order.CreateOrderResult.SveaOrderId, orderTwo.CreateOrderResult.SveaOrderId };

        var builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
            .SetOrderIds(orderIdsToDeliver)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST);

        var delivery = await builder.DeliverInvoiceOrders().DoRequest();

        Assert.That(delivery.ResultCode == 0);
        Assert.That(delivery.OrdersDelivered.Select(od => orderIdsToDeliver.Contains(od.SveaOrderId)).Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task Test_DeliverOrders_DeliverInvoiceOrderRows_WithMixedSetOrderIdxAndMultipleOrders()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        var orderTwo = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        var orderThree = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var orderIdsToDeliver = new List<long> { order.CreateOrderResult.SveaOrderId, orderTwo.CreateOrderResult.SveaOrderId };

        var builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
            .SetOrderIds(orderIdsToDeliver)
            .SetOrderId(orderThree.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST);

        var delivery = await builder.DeliverInvoiceOrders().DoRequest();
        Assert.That(delivery.ResultCode == 0);

        var referenceNumbers = from od in delivery.OrdersDelivered
                               where orderIdsToDeliver.Contains(od.SveaOrderId) || od.SveaOrderId == orderThree.CreateOrderResult.SveaOrderId
                               select od.DeliveryReferenceNumber;
        Assert.That(referenceNumbers.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task Test_DeliverOrders_DeliverInvoiceOrderRows_WithMixedOrderTypesFails()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        var orderTwo = await TestingTool.CreatePaymentPlanOrderWithOneOrderRow();

        var orderIdsToDeliver = new List<long> { order.CreateOrderResult.SveaOrderId, orderTwo.CreateOrderResult.SveaOrderId };

        var builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
            .SetOrderIds(orderIdsToDeliver)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST);

        var delivery = await builder.DeliverInvoiceOrders().DoRequest();

        Assert.That(delivery.ResultCode != 0);
        Assert.That(delivery.ResultCode, Is.EqualTo(20004));
        Assert.That(delivery.ErrorMessage, Is.EqualTo("No order found for orderId: " + orderTwo.CreateOrderResult.SveaOrderId.ToString()));
    }

    [Test]
    public async Task Test_DeliverOrders_DeliverPaymentPlanOrderRows_SetOrderIdAndSingleOrder()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();

        var builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var delivery = await builder.DeliverPaymentPlanOrders().DoRequest();

        Assert.That(delivery.ResultCode == 0);
        Assert.That(delivery.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber != null);
        Assert.That(delivery.OrdersDelivered.Single(od => od.SveaOrderId == order.CreateOrderResult.SveaOrderId).DeliveryReferenceNumber != null);
    }

    [Test]
    public async Task Test_DeliverOrderRows_DeliverInvoiceOrderRows()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var builder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetInvoiceDistributionType(DistributionType.POST) // TODO harmonize InvoiceDistributionType w/AdminWS?
            .SetRowToDeliver(1);

        var delivery = await builder.DeliverInvoiceOrderRows().DoRequest();

        Assert.That(delivery.ResultCode == 0);
    }

    [Test]
    public async Task Test_DeliverOrderRows_DeliverCardOrderRows_Deliver_All_Rows()
    {
        var payment = await CreateCardOrderWithTwoOrderRows();

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); // r1, r2: 100.00ex@25*2 => 500.00

        var builder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
            .SetTransactionId((long)answer.TransactionId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetRowToDeliver(1)
            .SetRowToDeliver(2)
            .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows);

        var delivery = builder.DeliverCardOrderRows().DoRequest();
        Assert.That(delivery.Accepted);

        var queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(queryConfirmedOrderAnswer.Accepted);
        Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); // r1, r2: 100.00ex@25*2 => 500.00
    }

    [Test]
    public async Task Test_DeliverOrderRows_DeliverCardOrderRows_Deliver_First_Of_Two_Rows()
    {
        var payment = await CreateCardOrderWithTwoOrderRows();

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); // r1, r2: 100.00ex@25*2 => 500.00

        var builder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
            .SetTransactionId((long)answer.TransactionId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetRowToDeliver(1)
            .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows);

        var delivery = builder.DeliverCardOrderRows().DoRequest();
        Assert.That(delivery.Accepted);

        var queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(queryConfirmedOrderAnswer.Accepted);
        Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("CONFIRMED"));
        Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.00M)); // r1, 100.00ex@25*2 => 250.00
    }

    [Test]
    public async Task Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingSetRowToCredit()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetRowToDeliver(1)
            .SetRowToDeliver(2)
            .SetOrderId(order.CreateOrderResult.SveaOrderId);

        var deliverResponse = await deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
        Assert.That(deliverResponse.ResultCode == 0);

        var creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
            .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(CountryCode.SE)
            .SetRowToCredit(1)
            .SetRowToCredit(2);

        var creditResponse = await creditBuilder.CreditInvoiceOrderRows().DoRequest();
        Assert.That(creditResponse.ResultCode == 0);
    }

    [Test]
    public async Task Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddCreditOrderRow()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetRowToDeliver(1)
            .SetRowToDeliver(2)
            .SetOrderId(order.CreateOrderResult.SveaOrderId);

        var deliverResponse = await deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
        Assert.That(deliverResponse.ResultCode == 0);

        var newExVatCreditOrderRow = new OrderRowBuilder()
            .SetName("NewCreditOrderRow")
            .SetAmountExVat(8.0M)
            .SetVatPercent(25)
            .SetQuantity(1M);

        var newCreditOrderRows = new List<OrderRowBuilder> { newExVatCreditOrderRow };

        var creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
            .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(CountryCode.SE)
            .AddCreditOrderRows(newCreditOrderRows);

        var creditResponse = await creditBuilder.CreditInvoiceOrderRows().DoRequest();
        Assert.That(creditResponse.ResultCode == 0);
    }

    [Test]
    public async Task Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddNewOrderRowAndMismatchedVatFlagSettingsFails()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetRowToDeliver(1)
            .SetRowToDeliver(2)
            .SetOrderId(order.CreateOrderResult.SveaOrderId);

        var deliverResponse = await deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
        Assert.That(deliverResponse.ResultCode == 0);

        var newIncVatCreditOrderRow = new OrderRowBuilder()
            .SetName("NewCreditOrderRow")
            .SetAmountIncVat(10.0M)
            .SetVatPercent(25)
            .SetQuantity(1M);

        var newCreditOrderRows = new List<OrderRowBuilder> { newIncVatCreditOrderRow };

        var creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
            .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(CountryCode.SE)
            .AddCreditOrderRows(newCreditOrderRows);

        var creditResponse = await creditBuilder.CreditInvoiceOrderRows().DoRequest();
        Assert.That(creditResponse.ResultCode != 0);
        Assert.That(creditResponse.ResultCode, Is.EqualTo(50036));
        Assert.That(creditResponse.ErrorMessage, Is.EqualTo("The flag PriceIncludingVat must be used consistently for all order rows in the order."));
    }

    [Test]
    public async Task Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUsingAddNewOrderRowSpecifiedExVatAndIncVatSentAsIncVat()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetRowToDeliver(1)
            .SetRowToDeliver(2)
            .SetOrderId(order.CreateOrderResult.SveaOrderId);

        var deliverResponse = await deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
        Assert.That(deliverResponse.ResultCode == 0);

        var newExVatCreditOrderRow = new OrderRowBuilder()
            .SetName("NewCreditOrderRow")
            .SetAmountIncVat(10M)
            .SetAmountExVat(8.0M)
            .SetQuantity(1M);

        var newCreditOrderRows = new List<OrderRowBuilder> { newExVatCreditOrderRow };

        var creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
            .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(CountryCode.SE)
            .AddCreditOrderRows(newCreditOrderRows);

        var creditResponse = await creditBuilder.CreditInvoiceOrderRows().DoRequest();
        Assert.That(creditResponse.ResultCode != 0);
        Assert.That(creditResponse.ResultCode, Is.EqualTo(50036));
        Assert.That(creditResponse.ErrorMessage, Is.EqualTo("The flag PriceIncludingVat must be used consistently for all order rows in the order."));
    }

    [Test]
    public async Task Test_CreditOrderRows_CreditInvoiceOrderRows_CreditUndeliveredRowFails()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var deliverBuilder = WebpayAdmin.DeliverOrderRows(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetRowToDeliver(1)
            .SetOrderId(order.CreateOrderResult.SveaOrderId);

        var deliverResponse = await deliverBuilder.DeliverInvoiceOrderRows().DoRequest();
        Assert.That(deliverResponse.ResultCode == 0);

        var creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
            .SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(CountryCode.SE)
            .SetRowToCredit(1)
            .SetRowToCredit(2);

        var creditResponse = await creditBuilder.CreditInvoiceOrderRows().DoRequest();
        Assert.That(creditResponse.ResultCode != 0);
        Assert.That(creditResponse.ResultCode, Is.EqualTo(20010));
        Assert.That(creditResponse.ErrorMessage, Is.EqualTo("All rows must belong to the invoice"));
    }

    [Test]
    public async Task Test_CreditOrderRows_CreditPaymentPlanOrderRows_Credit_UsingSetRowToCredit()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
        Assert.That(order.Accepted);

        var deliverBuilder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow());

        var deliverResponse = await deliverBuilder.DeliverPaymentPlanOrder().DoRequest();
        Assert.That(deliverResponse.Accepted);

        var creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
            .SetContractNumber(deliverResponse.DeliverOrderResult.PaymentPlanResultDetails.ContractNumber)
            .SetCountryCode(CountryCode.SE)
            .SetRowToCredit(1);

        var creditResponse = await creditBuilder.CreditPaymentPlanOrderRows().DoRequest();
        Assert.That(creditResponse.ResultCode == 0);
    }

    [Test]
    public async Task Test_CreditOrderRows_CreditPaymentPlanOrderRows_UsingAddCreditOrderRow()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
        Assert.That(order.Accepted);

        var deliverBuilder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow());

        var deliverResponse = await deliverBuilder.DeliverPaymentPlanOrder().DoRequest();
        Assert.That(deliverResponse.Accepted);

        var newExVatCreditOrderRow = new OrderRowBuilder()
            .SetName("NewCreditOrderRow")
            .SetAmountExVat(8.0M)
            .SetVatPercent(25)
            .SetQuantity(1M);

        var newCreditOrderRows = new List<OrderRowBuilder> { newExVatCreditOrderRow };

        var creditBuilder = WebpayAdmin.CreditOrderRows(SveaConfig.GetDefaultConfig())
            .SetContractNumber(deliverResponse.DeliverOrderResult.PaymentPlanResultDetails.ContractNumber)
            .SetCountryCode(CountryCode.SE)
            .AddCreditOrderRows(newCreditOrderRows);

        var creditResponse = await creditBuilder.CreditPaymentPlanOrderRows().DoRequest();
        Assert.That(creditResponse.ResultCode == 0);
    }

    [Test]
    public async Task Test_AddOrderRows_AddInvoiceOrderRows_OriginalAndUpdatedOrdersSpecifiedExVat()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var firstOrderRowPriceExVat = 64M;
        var firstOrderRowName = "New row #1";
        var firstOrderRowDescription = "This should be the third order row!";

        var firstOrderRow = new OrderRowBuilder()
            .SetAmountExVat(firstOrderRowPriceExVat)
            .SetVatPercent(25M)
            .SetQuantity(1M)
            .SetDiscountPercent(10)
            .SetName(firstOrderRowName)
            .SetDescription(firstOrderRowDescription);

        var secondOrderRowPriceExVat = 32M;
        var secondOrderRowName = "New row #2";
        var secondOrderRowDescription = "This should be the fourth order row!";

        var secondOrderRow = new OrderRowBuilder(firstOrderRow)
            .SetAmountExVat(secondOrderRowPriceExVat)
            .SetName(secondOrderRowName)
            .SetDescription(secondOrderRowDescription);

        var builder = WebpayAdmin.AddOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddOrderRow(firstOrderRow)
            .AddOrderRow(secondOrderRow);

        var addition = await builder.AddInvoiceOrderRows().DoRequest();
        Assert.That(addition.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryInvoiceOrder().DoRequest();
        Assert.That(answer.ResultCode == 0);
        Assert.That(!(bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).PriceIncludingVat);   // row #3
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).PricePerUnit, Is.EqualTo(firstOrderRowPriceExVat));
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).Description, Is.EqualTo(firstOrderRowName + ": " + firstOrderRowDescription));
        Assert.That(!(bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).PriceIncludingVat);   // row #4
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).PricePerUnit, Is.EqualTo(secondOrderRowPriceExVat));
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).Description, Is.EqualTo(secondOrderRowName + ": " + secondOrderRowDescription));
    }

    [Test]
    public async Task Test_AddOrderRows_AddPaymentPlanOrderRows_OriginalAndUpdatedOrdersSpecifiedExVat()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var firstOrderRowPriceExVat = 64M;
        var firstOrderRowName = "New row #1";
        var firstOrderRowDescription = "This should be the third order row!";

        var firstOrderRow = new OrderRowBuilder()
            .SetAmountExVat(firstOrderRowPriceExVat)
            .SetVatPercent(25M)
            .SetQuantity(1M)
            .SetDiscountPercent(10)
            .SetName(firstOrderRowName)
            .SetDescription(firstOrderRowDescription);

        var secondOrderRowPriceExVat = 32M;
        var secondOrderRowName = "New row #2";
        var secondOrderRowDescription = "This should be the fourth order row!";

        var secondOrderRow = new OrderRowBuilder(firstOrderRow); // uses copy constructor
        secondOrderRow
            .SetAmountExVat(secondOrderRowPriceExVat)
            .SetName(secondOrderRowName)
            .SetDescription(secondOrderRowDescription);

        var builder = WebpayAdmin.AddOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddOrderRows(new List<OrderRowBuilder> { firstOrderRow, secondOrderRow });

        var addition = await builder.AddPaymentPlanOrderRows().DoRequest();
        Assert.That(addition.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
        Assert.That(answer.ResultCode == 0);
        Assert.That(!(bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).PriceIncludingVat);   // row #3
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).PricePerUnit, Is.EqualTo(firstOrderRowPriceExVat));
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(2).Description, Is.EqualTo(firstOrderRowName + ": " + firstOrderRowDescription));
        Assert.That(!(bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).PriceIncludingVat);   // row #4
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).PricePerUnit, Is.EqualTo(secondOrderRowPriceExVat));
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(3).Description, Is.EqualTo(secondOrderRowName + ": " + secondOrderRowDescription));
    }

    [Test]
    public async Task Test_AddOrderRows_AddInvoiceOrderRows_OriginalAndUpdatedOrdersHasDifferentPriceIncludingVatFlagReturnsError()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var firstOrderRowPriceIncVat = 80M;
        var firstOrderRowName = "New row #1";
        var firstOrderRowDescription = "This should be the third order row!";

        var firstOrderRow = new OrderRowBuilder()
            .SetAmountIncVat(firstOrderRowPriceIncVat)
            .SetVatPercent(25M)
            .SetQuantity(1M)
            .SetDiscountPercent(10)
            .SetName(firstOrderRowName)
            .SetDescription(firstOrderRowDescription);

        var secondOrderRowPriceExVat = 32M;
        var secondOrderRowName = "New row #2";
        var secondOrderRowDescription = "This should be the fourth order row!";

        var secondOrderRow = new OrderRowBuilder(firstOrderRow); // Uses copy constructor
        secondOrderRow
            .SetAmountExVat(secondOrderRowPriceExVat)
            .SetName(secondOrderRowName)
            .SetDescription(secondOrderRowDescription);

        var builder = WebpayAdmin.AddOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddOrderRow(firstOrderRow)
            .AddOrderRow(secondOrderRow);

        var addition = await builder.AddInvoiceOrderRows().DoRequest();
        Assert.That(addition.ResultCode != 0);
        Assert.That(addition.ResultCode, Is.EqualTo(50036));
        Assert.That(addition.ErrorMessage, Is.EqualTo("The flag PriceIncludingVat must be used consistently for all order rows in the order."));
    }

    [Test]
    public async Task Test_UpdateOrderRows_UpdateInvoiceOrderRows_OriginalAndUpdatedOrdersSpecifiedExVat()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var updatedOrderRowIndex = 2; // i.e. 2nd row, as NumberedOrderRows are 1-indexed.
        var updatedOrderRowPriceExVat = 64M;
        var updatedOrderRowName = "New row #1";
        var updatedOrderRowDescription = "Replaces second original order row!";

        var updatedOrderRow = new NumberedOrderRowBuilder()
            .SetRowNumber(updatedOrderRowIndex)
            .SetAmountExVat(updatedOrderRowPriceExVat)
            .SetVatPercent(25M)
            .SetQuantity(1M)
            .SetDiscountPercent(10)
            .SetName(updatedOrderRowName)
            .SetDescription(updatedOrderRowDescription);

        var update = WebpayAdmin.UpdateOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddUpdateOrderRow(updatedOrderRow);

        var updateResponse = await update.UpdateInvoiceOrderRows().DoRequest();
        Assert.That(updateResponse.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryInvoiceOrder().DoRequest();
        Assert.That(answer.ResultCode == 0);
        Assert.That(!(bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PriceIncludingVat);
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PricePerUnit, Is.EqualTo(updatedOrderRowPriceExVat));
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).Description, Is.EqualTo(updatedOrderRowName + ": " + updatedOrderRowDescription));
    }

    [Test]
    public async Task Test_UpdateOrderRows_UpdateInvoiceOrderRows_OriginalAndUpdatedOrdersSpecifiedIncVat()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRowsSpecifiedIncVat();
        Assert.That(order.Accepted);

        var updatedOrderRowIndex = 2; // i.e. 2nd row, as NumberedOrderRows are 1-indexed.
        var updatedOrderRowPriceIncVat = 80M;
        var updatedOrderRowName = "New row #1";
        var updatedOrderRowDescription = "Replaces second original order row!";

        var updatedOrderRow = new NumberedOrderRowBuilder()
            .SetRowNumber(updatedOrderRowIndex)
            .SetAmountIncVat(updatedOrderRowPriceIncVat)
            .SetVatPercent(25M)
            .SetQuantity(1M)
            .SetDiscountPercent(10)
            .SetName(updatedOrderRowName)
            .SetDescription(updatedOrderRowDescription);

        var update = WebpayAdmin.UpdateOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddUpdateOrderRow(updatedOrderRow);

        var updateResponse = await update.UpdateInvoiceOrderRows().DoRequest();
        Assert.That(updateResponse.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryInvoiceOrder().DoRequest();
        Assert.That(answer.ResultCode == 0);
        Assert.That((bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PriceIncludingVat);
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PricePerUnit, Is.EqualTo(updatedOrderRowPriceIncVat));
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).Description, Is.EqualTo(updatedOrderRowName + ": " + updatedOrderRowDescription));
    }


    [Test]
    public async Task Test_UpdateOrderRows_UpdateInvoiceOrderRows_OriginalAndUpdatedOrdersHasDifferentPriceIncludingVatFlagFails()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRowsSpecifiedIncVat();
        Assert.That(order.Accepted);

        var updatedOrderRowIndex = 2; // i.e., 2nd row, as NumberedOrderRows are 1-indexed.
        var updatedOrderRowPriceExVat = 64M;
        var updatedOrderRowName = "New row #1";
        var updatedOrderRowDescription = "Replaces second original order row!";

        var updatedOrderRow = new NumberedOrderRowBuilder()
            .SetRowNumber(updatedOrderRowIndex)
            .SetAmountExVat(updatedOrderRowPriceExVat)
            .SetVatPercent(25M)
            .SetQuantity(1M)
            .SetDiscountPercent(10)
            .SetName(updatedOrderRowName)
            .SetDescription(updatedOrderRowDescription);

        var update = WebpayAdmin.UpdateOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddUpdateOrderRow(updatedOrderRow);

        var updateResponse = await update.UpdateInvoiceOrderRows().DoRequest();
        Assert.That(updateResponse.ResultCode != 0);
        Assert.That(updateResponse.ResultCode, Is.EqualTo(50036));
        Assert.That(updateResponse.ErrorMessage, Is.EqualTo("The flag PriceIncludingVat must be used consistently for all order rows in the order."));
    }

    [Test]
    public async Task Test_UpdateOrderRows_UpdatePaymentPlanOrderRows_OriginalAndUpdatedOrdersSpecifiedExVat()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var updatedOrderRowIndex = 2; // i.e., 2nd row, as NumberedOrderRows are 1-indexed.
        var updatedOrderRowPriceExVat = 64M;
        var updatedOrderRowName = "New row #1";
        var updatedOrderRowDescription = "Replaces second original order row!";

        var updatedOrderRow = new NumberedOrderRowBuilder()
            .SetRowNumber(updatedOrderRowIndex)
            .SetAmountExVat(updatedOrderRowPriceExVat)
            .SetVatPercent(25M)
            .SetQuantity(1M)
            .SetDiscountPercent(10)
            .SetName(updatedOrderRowName)
            .SetDescription(updatedOrderRowDescription);

        var update = WebpayAdmin.UpdateOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .AddUpdateOrderRow(updatedOrderRow);

        var updateResponse = await update.UpdatePaymentPlanOrderRows().DoRequest();
        Assert.That(updateResponse.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();
        Assert.That(answer.ResultCode == 0);
        Assert.That(!(bool)answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PriceIncludingVat);
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).PricePerUnit, Is.EqualTo(updatedOrderRowPriceExVat));
        Assert.That(answer.Orders.FirstOrDefault().OrderRows.ElementAt(updatedOrderRowIndex - 1).Description, Is.EqualTo(updatedOrderRowName + ": " + updatedOrderRowDescription));
    }

    [Test]
    public async Task Test_CancelOrderRows_CancelInvoiceOrderRows_CancelAllRows()
    {
        var order = await TestingTool.CreateInvoiceOrderWithOneOrderRow();
        Assert.That(order.Accepted);

        var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetRowToCancel(1)
            .SetCountryCode(CountryCode.SE);
        var cancellationResponse = await cancellation.CancelInvoiceOrderRows().DoRequest();
        Assert.That(cancellationResponse.ResultCode == 0);
    }

    [Test]
    public async Task Test_CancelOrderRows_CancelInvoiceOrderRows_CancelFirstOfTwoRows()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetRowToCancel(1)
            .SetCountryCode(CountryCode.SE);
        var cancellationResponse = await cancellation.CancelInvoiceOrderRows().DoRequest();
        Assert.That(cancellationResponse.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);
        var answer = await queryOrderBuilder.QueryInvoiceOrder().DoRequest();

        Assert.That(answer.ResultCode == 0);
        Assert.That(answer.Orders.First().OrderRows.ElementAt(0).Status, Is.EqualTo("Cancelled"));
        Assert.That(answer.Orders.First().OrderRows.ElementAt(1).Status, Is.EqualTo("NotDelivered"));
    }

    [Test]
    public async Task Test_CreditOrderRows_CreditInvoiceOrderRows_CancelDeliveredRowFails()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var builder = WebpayAdmin.DeliverOrders(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST);
        var delivery = await builder.DeliverInvoiceOrders().DoRequest();
        Assert.That(delivery.ResultCode == 0);

        var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetRowToCancel(1)
            .SetCountryCode(CountryCode.SE);
        var cancellationResponse = await cancellation.CancelInvoiceOrderRows().DoRequest();

        Assert.That(cancellationResponse.ResultCode != 0);
        Assert.That(cancellationResponse.ResultCode, Is.EqualTo(20000));
        Assert.That(cancellationResponse.ErrorMessage, Is.EqualTo("Order is closed."));
    }

    [Test]
    public async Task Test_CancelOrderRows_CancelPaymentPlanOrderRows_CancelAllRows()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
        Assert.That(order.Accepted);

        var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetRowToCancel(1)
            .SetCountryCode(CountryCode.SE);
        var cancellationResponse = await cancellation.CancelPaymentPlanOrderRows().DoRequest();

        Assert.That(cancellationResponse.ResultCode == 0);
    }

    [Test]
    public async Task Test_CancelOrderRows_CancelPaymentPlanOrderRows_CancelFirstOfTwoRows()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var cancellation = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetRowToCancel(1)
            .SetCountryCode(CountryCode.SE);
        var cancellationResponse = await cancellation.CancelPaymentPlanOrderRows().DoRequest();
        Assert.That(cancellationResponse.ResultCode == 0);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var answer = await queryOrderBuilder.QueryPaymentPlanOrder().DoRequest();

        Assert.That(answer.ResultCode == 0);
        Assert.That(answer.Orders.First().OrderRows.ElementAt(0).Status, Is.EqualTo("Cancelled"));
        Assert.That(answer.Orders.First().OrderRows.ElementAt(1).Status, Is.EqualTo("NotDelivered"));
    }

    [Test]
    public async Task Test_CancelOrderRows_CancelCardOrderRows_CancellingAllRowsGivesOrderStatusAnnulled()
    {
        var payment = await CreateCardOrderWithTwoOrderRows();

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); // r1, r2: 100.00ex@25*2 => 500.00

        var builder = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetTransactionId((long)answer.TransactionId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetRowToCancel(1)
            .SetRowToCancel(2)
            .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows);

        var cancellation = builder.CancelCardOrderRows().DoRequest();
        Assert.That(cancellation.Accepted);

        var queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();

        Assert.That(queryConfirmedOrderAnswer.Accepted);
        Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("ANNULLED"));
        Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.Null);
    }

    [Test]
    public async Task Test_CancelOrderRows_CancelCardOrderRows_CancellingFirstOfTwoRows()
    {
        var payment = await CreateCardOrderWithTwoOrderRows();

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M)); // r1, r2: 100.00ex@25*2 => 500.00

        var builder = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetTransactionId((long)answer.TransactionId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetRowToCancel(1)
            .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows);

        var cancellation = builder.CancelCardOrderRows().DoRequest();
        Assert.That(cancellation.Accepted);

        var queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();

        Assert.That(queryConfirmedOrderAnswer.Accepted);
        Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("AUTHORIZED"));
        Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.0M)); // r2: 100.00ex@25*2 => 250.00
    }

    [Test]
    public async Task Test_CancelOrderRows_CancelCardOrderRows_CancellingFirstOfTwoRows_SpecifiedIncVatAndVatPercent()
    {
        var payment = await CreateCardOrderWithTwoOrderRowsSpecifiedIncVatAndVatPercent();

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M));

        var builder = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetTransactionId((long)answer.TransactionId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetRowToCancel(1)
            .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows);

        var cancellation = builder.CancelCardOrderRows().DoRequest();
        Assert.That(cancellation.Accepted);

        var queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(queryConfirmedOrderAnswer.Accepted);
        Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("AUTHORIZED"));
        Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.0M));
    }

    [Test]
    public async Task Test_CancelOrderRows_CancelCardOrderRows_CancellingFirstOfTwoRows_SpecifiedIncVatAndExVat()
    {
        var payment = await CreateCardOrderWithTwoOrderRowsSpecifiedIncVatAndExVat();

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        Assert.That(answer.Transaction.AuthorizedAmount, Is.EqualTo(500.00M));

        var builder = WebpayAdmin.CancelOrderRows(SveaConfig.GetDefaultConfig())
            .SetTransactionId((long)answer.TransactionId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetRowToCancel(1)
            .AddNumberedOrderRows(answer.Transaction.NumberedOrderRows);

        var cancellation = builder.CancelCardOrderRows().DoRequest();
        Assert.That(cancellation.Accepted);

        var queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(queryConfirmedOrderAnswer.Accepted);
        Assert.That(queryConfirmedOrderAnswer.Transaction.Status, Is.EqualTo("AUTHORIZED"));
        Assert.That(queryConfirmedOrderAnswer.Transaction.AuthorizedAmount, Is.EqualTo(250.0M));
    }

    [Test]
    public async Task Test_CancelOrder_CancelInvoiceOrder()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();
        Assert.That(order.Accepted);

        var cancelOrderBuilder = WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var cancellation = await cancelOrderBuilder.CancelInvoiceOrder().DoRequest();
        Assert.That(cancellation.ResultCode == 0);
    }

    [Test]
    public async Task Test_CancelOrder_CancelPaymentPlanOrder()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
        Assert.That(order.Accepted);

        var cancelOrderBuilder = WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE);

        var cancellation = await cancelOrderBuilder.CancelPaymentPlanOrder().DoRequest();
        Assert.That(cancellation.ResultCode == 0);
    }

    [Test]
    public async Task Test_CancelOrder_CancelCardOrder()
    {
        var payment = await CreateCardOrderWithTwoOrderRows();

        var cancelOrderBuilder = WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var cancellation = cancelOrderBuilder.CancelCardOrder().DoRequest();
        Assert.That(cancellation.Accepted);

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(payment.TransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        Assert.That(answer.Transaction.Status, Is.EqualTo("ANNULLED")); // TODO make enum w/Transaction statuses
    }

    [Test]
    public async Task Test_CreditAmount_CreditPaymentPlanAmount()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
        Assert.That(order.Accepted);

        var contract = await WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.SE)
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .DeliverPaymentPlanOrder()
            .DoRequest();

        Assert.That(contract.Accepted);

        var creditAmountBuilder = WebpayAdmin.CreditPayment(SveaConfig.GetDefaultConfig())
            .SetContractNumber(contract.DeliverOrderResult.PaymentPlanResultDetails.ContractNumber)
            .SetCountryCode(CountryCode.SE)
            .SetDescription("test of credit amount")
            .SetAmountIncVat(100.00M);

        var response = await creditAmountBuilder.CreditPaymentPlanAmount().DoRequest();
        Assert.That(response.ResultCode == 0);
    }

    [Test]
    public async Task Test_CreditAmount_CreditPaymentPlanAmount_CreditUndeliveredPaymentPlanFails()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithOneOrderRow();
        Assert.That(order.Accepted);

        var creditAmountBuilder = WebpayAdmin.CreditPayment(SveaConfig.GetDefaultConfig())
            .SetContractNumber(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetDescription("test of credit amount")
            .SetAmountIncVat(100.00M);

        var response = await creditAmountBuilder.CreditPaymentPlanAmount().DoRequest();
        Assert.That(response.ResultCode, Is.EqualTo(27006));
        Assert.That(response.ErrorMessage, Is.EqualTo("No paymentplan exists with the provided id."));
    }

    [Test, Ignore("")]
    public async Task Test_CreditAmount_CreditCardAmount()
    {
        // Use an existing captured order (status SUCCESS), as we can't do a capture on an order via the webservice
        var capturedTransactionId = 625718L;

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(capturedTransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        var before = answer.Transaction.CreditedAmount;

        var amountToCredit = 1.00M;
        var creditAmountBuilder = WebpayAdmin.CreditPayment(SveaConfig.GetDefaultConfig())
            .SetContractNumber(capturedTransactionId)
            .SetCountryCode(CountryCode.SE)
            .SetDescription("test of credit amount")
            .SetAmountIncVat(amountToCredit);

        var response = creditAmountBuilder.CreditCardPayment().DoRequest();
        Assert.That(response.Accepted);

        var queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(capturedTransactionId)
            .SetCountryCode(CountryCode.SE);

        var queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(queryConfirmedOrderAnswer.Accepted);
        var after = queryConfirmedOrderAnswer.Transaction.CreditedAmount;
        Assert.That(after, Is.EqualTo(before + amountToCredit));
    }

    [Test, Ignore("")]
    public async Task Test_CreditAmount_CreditDirectBankAmount()
    {
        // Use an existing captured order (status SUCCESS), as we can't do a capture on an order via the webservice
        var capturedTransactionId = 590801L;

        var queryOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(capturedTransactionId)
            .SetCountryCode(CountryCode.SE);

        var answer = queryOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(answer.Accepted);
        var before = answer.Transaction.CreditedAmount;

        var amountToCredit = 1.00M;
        var creditAmountBuilder = WebpayAdmin.CreditPayment(SveaConfig.GetDefaultConfig())
            .SetContractNumber(capturedTransactionId)
            .SetCountryCode(CountryCode.SE)
            .SetDescription("test of credit amount")
            .SetAmountIncVat(amountToCredit);

        var response = creditAmountBuilder.CreditDirectBankPayment().DoRequest();
        Assert.That(response.Accepted);

        var queryConfirmedOrderBuilder = WebpayAdmin.QueryOrder(SveaConfig.GetDefaultConfig())
            .SetTransactionId(capturedTransactionId)
            .SetCountryCode(CountryCode.SE);

        var queryConfirmedOrderAnswer = queryConfirmedOrderBuilder.QueryCardOrder().DoRequest();
        Assert.That(queryConfirmedOrderAnswer.Accepted);
        var after = queryConfirmedOrderAnswer.Transaction.CreditedAmount;
        Assert.That(after, Is.EqualTo(before + amountToCredit));
    }
}