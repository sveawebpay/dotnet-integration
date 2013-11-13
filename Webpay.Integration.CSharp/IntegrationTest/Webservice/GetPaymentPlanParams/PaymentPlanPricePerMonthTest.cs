using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.GetPaymentPlanParams
{
    [TestFixture]
    public class PaymentPlanPricePerMonthTest
    {
        private GetPaymentPlanParamsEuResponse GetParamsForTesting()
        {
            var request = WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig());
            GetPaymentPlanParamsEuResponse response = request
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DoRequest();

            return response;
        }

        [Test]
        public void TestBuildPriceCalculator()
        {
            GetPaymentPlanParamsEuResponse paymentPlanParams = GetParamsForTesting();

            List<Dictionary<string, long>> result = WebpayConnection.PaymentPlanPricePerMonth(2000.0M, paymentPlanParams);

            Assert.AreEqual(213060, result[0]["campaignCode"]);
            Assert.AreEqual(2029, result[0]["pricePerMonth"]);
            Assert.AreEqual(202, result[1]["pricePerMonth"]);
        }

        [Test]
        public void TestBuildPriceCalculatorWithLowPrice()
        {
            GetPaymentPlanParamsEuResponse paymentPlanParams = GetParamsForTesting();

            List<Dictionary<string, long>> result = WebpayConnection.PaymentPlanPricePerMonth(200.0M, paymentPlanParams);

            Assert.IsTrue(result.Count == 0);
        }
    }
}