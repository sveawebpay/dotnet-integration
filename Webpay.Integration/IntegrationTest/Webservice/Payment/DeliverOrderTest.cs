using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.IntegrationTest.Webservice.Payment;

[TestFixture]
public class DeliverOrderTest
{
    [Test]
    public async Task TestDeliverPaymentPlanOrderDoRequestAsync()
    {
        var response = await WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .SetOrderId(54086L)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DeliverPaymentPlanOrder()
            .DoRequestAsync();

        Assert.That(response.ErrorMessage, Is.EqualTo("Currently unable to modify order, please try again later."));
    }

    [Test]
    public async Task TestDeliverInvoiceOrderResultAsync()
    {
        var orderId = await CreateExVatInvoiceAndReturnOrderId();

        var response = await WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .SetOrderId(orderId)
            .SetNumberOfCreditDays(1)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DeliverInvoiceOrder()
            .DoRequestAsync();

        Assert.That(response.Accepted, Is.True);
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.Post));
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr, Is.Not.Null);
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr.Length, Is.GreaterThan(0));
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.LowestAmountToPay, Is.EqualTo(0.0));
    }

    [Test]
    public async Task TestDeliverInvoiceOrderWithEInvoiceB2BResult()
    {
        var orderId = await CreateNorwegianExVatInvoiceAndReturnOrderId();

        var response = await WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .SetOrderId(orderId)
            .SetNumberOfCreditDays(1)
            .SetInvoiceDistributionType(DistributionType.EINVOICEB2B)
            .SetCountryCode(CountryCode.NO)
            .DeliverInvoiceOrder()
            .DoRequestAsync();

        Assert.That(response.Accepted, Is.True);
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.EInvoiceB2B));
    }

    [Test]
    public async Task TestDeliverInvoiceOrderCreatedInclVatDeliveredInclVatAsync()
    {
        var orderId = await CreateIncVatOrderAndReturnOrderId();

        var response = await WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
            .SetOrderId(orderId)
            .SetNumberOfCreditDays(1)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DeliverInvoiceOrder()
            .DoRequestAsync();

        Assert.That(response.Accepted, Is.True);
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.Post));
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr, Is.Not.Null);
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr.Length, Is.GreaterThan(0));
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.LowestAmountToPay, Is.EqualTo(0.0));
    }

    [Test]
    public async Task TestDeliverInvoiceOrderCreatedExclVatDeliveredInclVatRetriesWithExVatAsync()
    {
        var orderId = await CreateExVatInvoiceAndReturnOrderId();

        var response = await WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
            .SetOrderId(orderId)
            .SetNumberOfCreditDays(1)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DeliverInvoiceOrder()
            .DoRequestAsync();

        Assert.That(response.Accepted, Is.True);
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.Post));
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr, Is.Not.Null);
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr.Length, Is.GreaterThan(0));
        Assert.That(response.DeliverOrderResult.InvoiceResultDetails.LowestAmountToPay, Is.EqualTo(0.0));
    }

    [Test]
    public async Task TestDeliverPaymentPlanOrderResult()
    {
        var paymentPlanParamResponse = await WebpayConnection
            .GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DoRequestAsync();

        var code = paymentPlanParamResponse.CampaignCodes[0].CampaignCode;

        var createOrderResponse = await WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetCustomerReference(TestingTool.DefaultTestClientOrderNumber)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePaymentPlanPayment(code)
            .DoRequestAsync();

        var deliverOrderResponse = await WebpayConnection
            .DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .SetOrderId(createOrderResponse.CreateOrderResult.SveaOrderId)
            .SetNumberOfCreditDays(1)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DeliverPaymentPlanOrder()
            .DoRequestAsync();

        Assert.That(deliverOrderResponse.Accepted, Is.True);
    }

    private async Task<long> CreateExVatInvoiceAndReturnOrderId()
    {
        var response = await WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddCustomerDetails(
                Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber)
            )
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequestAsync();

        return response.CreateOrderResult.SveaOrderId;
    }

    private static async Task<long> CreateIncVatOrderAndReturnOrderId()
    {
        var response = await WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
            .AddCustomerDetails(
                Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber)
            )
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequestAsync();

        return response.CreateOrderResult.SveaOrderId;
    }

    private async Task<long> CreateNorwegianExVatInvoiceAndReturnOrderId()
    {
        var response = await WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddCustomerDetails(
                Item.CompanyCustomer()
                    .SetNationalIdNumber("923313850")
            )
            .SetCountryCode(CountryCode.NO)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequestAsync();

        return response.CreateOrderResult.SveaOrderId;
    }
}