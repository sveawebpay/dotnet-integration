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

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
            Assert.AreEqual(3, response.CampaignCodes.Length);
            Assert.AreEqual(213060, response.CampaignCodes[0].CampaignCode);
            Assert.AreEqual(310012, response.CampaignCodes[1].CampaignCode);
            Assert.AreEqual(410024, response.CampaignCodes[2].CampaignCode);
        }

        [Test]
        public void TestResultGetPaymentPlanParams()
        {
            GetPaymentPlanParamsEuResponse response = WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                                                                      .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                      .DoRequest();

            Assert.IsTrue(response.Accepted);
            Assert.AreEqual(213060, response.CampaignCodes[0].CampaignCode);
            Assert.AreEqual("Köp nu betala om 3 månader (räntefritt)", response.CampaignCodes[0].Description);
            Assert.AreEqual(PaymentPlanTypeCode.InterestAndAmortizationFree, response.CampaignCodes[0].PaymentPlanType);
            Assert.AreEqual(3, response.CampaignCodes[0].ContractLengthInMonths);
            Assert.AreEqual(100, response.CampaignCodes[0].InitialFee);
            Assert.AreEqual(29, response.CampaignCodes[0].NotificationFee);
            Assert.AreEqual(0, response.CampaignCodes[0].InterestRatePercent);
            Assert.AreEqual(3, response.CampaignCodes[0].NumberOfInterestFreeMonths);
            Assert.AreEqual(3, response.CampaignCodes[0].NumberOfPaymentFreeMonths);
            Assert.AreEqual(1000, response.CampaignCodes[0].FromAmount);
            Assert.AreEqual(50000, response.CampaignCodes[0].ToAmount);
        }
    }
}