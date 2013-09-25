using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Test.Webservice.GetPaymentPlanParams
{
    [TestFixture]
    public class GetPaymentPlanParamsTest
    {
        [Test]
        public void TestGetPaymentPlanParams()
        {
            GetPaymentPlanParamsEuResponse response =
                WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                                .SetCountrycode(CountryCode.SE)
                                .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
            Assert.AreEqual(3, response.CampaignCodes.Length);
            Assert.AreEqual(213060, response.CampaignCodes[0].CampaignCode);
            Assert.AreEqual(310012, response.CampaignCodes[1].CampaignCode);
            Assert.AreEqual(410024, response.CampaignCodes[2].CampaignCode);
        }
    }
}