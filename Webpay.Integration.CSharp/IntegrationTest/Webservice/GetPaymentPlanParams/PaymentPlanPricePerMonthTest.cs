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
            Assert.That(result[0]["pricePerMonth"], Is.EqualTo(80));
            Assert.That(result[1]["campaignCode"], Is.EqualTo(222065));
            Assert.That(result[1]["pricePerMonth"], Is.EqualTo(2000));
            Assert.That(result[2]["campaignCode"], Is.EqualTo(222066));
            Assert.That(result[2]["pricePerMonth"], Is.EqualTo(2000));
            Assert.That(result[3]["campaignCode"], Is.EqualTo(223060));
            Assert.That(result[3]["pricePerMonth"], Is.EqualTo(2029));
            Assert.That(result[4]["campaignCode"], Is.EqualTo(223065));
            Assert.That(result[4]["pricePerMonth"], Is.EqualTo(2000));
            Assert.That(result[5]["campaignCode"], Is.EqualTo(223066));
            Assert.That(result[5]["pricePerMonth"], Is.EqualTo(2000));
            Assert.That(result[6]["campaignCode"], Is.EqualTo(310012));
            Assert.That(result[6]["pricePerMonth"], Is.EqualTo(202));
            Assert.That(result[7]["campaignCode"], Is.EqualTo(410012));
            Assert.That(result[7]["pricePerMonth"], Is.EqualTo(214));
            Assert.That(result[8]["campaignCode"], Is.EqualTo(410024));
            Assert.That(result[8]["pricePerMonth"], Is.EqualTo(129));
        }

        [Test]
        public void TestBuildPriceCalculatorWithLowPrice()
        {
            GetPaymentPlanParamsEuResponse paymentPlanParams = GetParamsForTesting();

            List<Dictionary<string, long>> result = WebpayConnection.PaymentPlanPricePerMonth(99.0M, paymentPlanParams);

            Assert.That(result.Count == 0, Is.True);
        }
    }
}