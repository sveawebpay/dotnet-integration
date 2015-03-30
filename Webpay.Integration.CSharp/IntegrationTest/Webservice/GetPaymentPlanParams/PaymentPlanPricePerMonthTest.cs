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

            Assert.That(result[0]["campaignCode"], Is.EqualTo(213060));
            Assert.That(result[0]["pricePerMonth"], Is.EqualTo(2029));
            Assert.That(result[1]["campaignCode"], Is.EqualTo(223060));
            Assert.That(result[1]["pricePerMonth"], Is.EqualTo(2029));
            Assert.That(result[2]["campaignCode"], Is.EqualTo(310012));
            Assert.That(result[2]["pricePerMonth"], Is.EqualTo(202));
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