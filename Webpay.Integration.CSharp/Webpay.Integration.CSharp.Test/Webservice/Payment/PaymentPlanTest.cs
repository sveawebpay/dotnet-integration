using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Test.Webservice.Payment
{
    [TestFixture]
    public class PaymentPlanTest
    {
        [Test]
        public void TestPaymentPlanRequestObjectSpecifics()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                           .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                                           .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UsePaymentPlanPayment(1337L)
                                                           .PrepareRequest();

            Assert.That(request.CreateOrderInformation.CreatePaymentPlanDetails.CampaignCode, Is.EqualTo(1337L));
            Assert.That(request.CreateOrderInformation.CreatePaymentPlanDetails.SendAutomaticGiroPaymentForm, Is.False);
        }

        [Test]
        public void TestPaymentPlanFailCompanyCustomer()
        {
            try
            {
                WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .SetOrderDate(TestingTool.DefaultTestDate)
                                .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                .SetCurrency(TestingTool.DefaultTestCurrency)
                                .UsePaymentPlanPayment(1337L)
                                .PrepareRequest();
                Assert.Fail("Expected exception not thrown.");
            }
            catch (SveaWebPayException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("ERROR - CompanyCustomer is not allowed to use payment plan option."));
            }
        }
    }
}