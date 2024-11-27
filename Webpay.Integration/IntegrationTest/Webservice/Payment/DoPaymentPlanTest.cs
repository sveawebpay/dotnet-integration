using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.IntegrationTest.Webservice.Payment;

[TestFixture]
public class DoPaymentPlanTest
{
    [Test]
    public async Task TestPaymentPlanRequestReturnsAcceptedResult()
    {
        var paymentPlanParam = await WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DoRequestAsync();
        var code = paymentPlanParam.CampaignCodes[0].CampaignCode;

        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePaymentPlanPayment(code)
            .DoRequestAsync();

        Assert.That(response.Accepted, Is.True);
    }

    [Test]
    public async Task TestDeliverPaymentPlanOrderResult()
    {
        var orderId = await createPaymentPlanAndReturnOrderId();

        var response = await WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
            .SetOrderId(orderId)
            .SetNumberOfCreditDays(1)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DeliverPaymentPlanOrder()
            .DoRequestAsync();

        Assert.That(response.Accepted, Is.True);
    }

    private async Task<long> createPaymentPlanAndReturnOrderId()
    {
        var paymentPlanParam = await WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DoRequestAsync();
        var code = paymentPlanParam.CampaignCodes[0].CampaignCode;

        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePaymentPlanPayment(code)
            .DoRequestAsync();

        return response.CreateOrderResult.SveaOrderId;
    }
}