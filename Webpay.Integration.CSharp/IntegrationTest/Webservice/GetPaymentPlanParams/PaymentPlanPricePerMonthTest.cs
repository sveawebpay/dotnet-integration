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

            List<Dictionary<string, long>> result = WebpayConnection.PaymentPlanPricePerMonth(11200.0M, paymentPlanParams);

            Assert.That(result[0]["campaignCode"], Is.EqualTo(223060));
            Assert.That(result[0]["pricePerMonth"], Is.EqualTo(11229));
            Assert.That(result[1]["campaignCode"], Is.EqualTo(223067));
            Assert.That(result[1]["pricePerMonth"], Is.EqualTo(11229));
            Assert.That(result[2]["campaignCode"], Is.EqualTo(310003));
            Assert.That(result[2]["pricePerMonth"], Is.EqualTo(3843));
            Assert.That(result[3]["campaignCode"], Is.EqualTo(310006));
            Assert.That(result[3]["pricePerMonth"], Is.EqualTo(1961));
            Assert.That(result[4]["campaignCode"], Is.EqualTo(310012));
            Assert.That(result[4]["pricePerMonth"], Is.EqualTo(993));
            Assert.That(result[5]["campaignCode"], Is.EqualTo(410018));
            Assert.That(result[5]["pricePerMonth"], Is.EqualTo(737));
            Assert.That(result[6]["campaignCode"], Is.EqualTo(410024));
            Assert.That(result[6]["pricePerMonth"], Is.EqualTo(574));
            Assert.That(result[7]["campaignCode"], Is.EqualTo(410048));
            Assert.That(result[7]["pricePerMonth"], Is.EqualTo(289));
            Assert.That(result[8]["campaignCode"], Is.EqualTo(410060));
            Assert.That(result[8]["pricePerMonth"], Is.EqualTo(240));
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