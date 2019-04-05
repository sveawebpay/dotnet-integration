using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.GetPaymentPlanParams
{
    [TestFixture]
    public class GetPaymentPlanParamsTest
    {
        [Test]
        public void TestGetPaymentPlanParams()
        {
            GetPaymentPlanParamsEuResponse response =
                WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .DoRequest();

            Assert.That(response.ResultCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.CampaignCodes.Length, Is.EqualTo(10));
            Assert.That(response.CampaignCodes[0].CampaignCode, Is.EqualTo(213060));
            Assert.That(response.CampaignCodes[1].CampaignCode, Is.EqualTo(222065));
            Assert.That(response.CampaignCodes[2].CampaignCode, Is.EqualTo(222066));
            Assert.That(response.CampaignCodes[3].CampaignCode, Is.EqualTo(223060));
            Assert.That(response.CampaignCodes[4].CampaignCode, Is.EqualTo(223065));
            Assert.That(response.CampaignCodes[5].CampaignCode, Is.EqualTo(223066));
            Assert.That(response.CampaignCodes[6].CampaignCode, Is.EqualTo(310012));
            Assert.That(response.CampaignCodes[7].CampaignCode, Is.EqualTo(410012));
            Assert.That(response.CampaignCodes[8].CampaignCode, Is.EqualTo(410024));
            Assert.That(response.CampaignCodes[9].CampaignCode, Is.EqualTo(996699));
        }

        [Test]
        public void TestResultGetPaymentPlanParams()
        {
            GetPaymentPlanParamsEuResponse response = WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                                                                      .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                      .DoRequest();

            Assert.IsTrue(response.Accepted);
            Assert.That(response.CampaignCodes[0].CampaignCode, Is.EqualTo(213060));
            Assert.That(response.CampaignCodes[0].Description, Is.EqualTo("Dela upp betalningen på 60 månader"));
            Assert.That(response.CampaignCodes[0].PaymentPlanType, Is.EqualTo(PaymentPlanTypeCode.Standard));
            Assert.That(response.CampaignCodes[0].ContractLengthInMonths, Is.EqualTo(60));
            Assert.That(response.CampaignCodes[0].InitialFee, Is.EqualTo(100));
            Assert.That(response.CampaignCodes[0].NotificationFee, Is.EqualTo(29));
            Assert.That(response.CampaignCodes[0].InterestRatePercent, Is.EqualTo(16.75M));
            Assert.That(response.CampaignCodes[0].NumberOfInterestFreeMonths, Is.EqualTo(3));
            Assert.That(response.CampaignCodes[0].NumberOfPaymentFreeMonths, Is.EqualTo(3));
            Assert.That(response.CampaignCodes[0].FromAmount, Is.EqualTo(1000));
            Assert.That(response.CampaignCodes[0].ToAmount, Is.EqualTo(50000));
        }

        [Test]
        public void TestPaymentPlanRequestReturnsAcceptedResult()
        {
            GetPaymentPlanParamsEuResponse paymentPlanParam = WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                              .DoRequest();
            long code = paymentPlanParam.CampaignCodes[0].CampaignCode;

            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
                                                             .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .UsePaymentPlanPayment(code)
                                                             .DoRequest();

            Assert.IsTrue(response.Accepted);
        }
    }
}