using Webpay.Integration.Config;
using Webpay.Integration.Util.Testing;
using WebpayWS;

namespace Webpay.Integration.IntegrationTest.Webservice.GetPaymentPlanParams;

[TestFixture]
public class GetPaymentPlanParamsTest
{
    [Test]
    public async Task TestGetPaymentPlanParams()
    {
        var response = await WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig()).SetCountryCode(TestingTool.DefaultTestCountryCode).DoRequestAsync();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
        Assert.That(response.CampaignCodes.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task TestResultGetPaymentPlanParams()
    {
        var response = await WebpayConnection
            .GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DoRequestAsync();

        Assert.That(response.Accepted);
        Assert.That(response.CampaignCodes[0].CampaignCode, Is.EqualTo(223060));
        Assert.That(response.CampaignCodes[0].Description, Is.EqualTo("Köp nu betala om 3 månader (räntefritt)"));
        Assert.That(response.CampaignCodes[0].PaymentPlanType, Is.EqualTo(PaymentPlanTypeCode.InterestAndAmortizationFree));
        Assert.That(response.CampaignCodes[0].ContractLengthInMonths, Is.EqualTo(3));
        Assert.That(response.CampaignCodes[0].InitialFee, Is.EqualTo(0));
        Assert.That(response.CampaignCodes[0].NotificationFee, Is.EqualTo(29));
        Assert.That(response.CampaignCodes[0].InterestRatePercent, Is.EqualTo(0));
        Assert.That(response.CampaignCodes[0].NumberOfInterestFreeMonths, Is.EqualTo(3));
        Assert.That(response.CampaignCodes[0].NumberOfPaymentFreeMonths, Is.EqualTo(3));
        Assert.That(response.CampaignCodes[0].FromAmount, Is.EqualTo(1000));
        Assert.That(response.CampaignCodes[0].ToAmount, Is.EqualTo(50000));
    }

    [Test]
    public async Task TestPaymentPlanRequestReturnsAcceptedResult()
    {
        var paymentPlanParam = await WebpayConnection
            .GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DoRequestAsync();

        var code = paymentPlanParam.CampaignCodes[0].CampaignCode;

        CreateOrderEuResponse response = await WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePaymentPlanPayment(code)
            .DoRequestAsync();

        Assert.That(response.Accepted);
    }
}