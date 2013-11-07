using NUnit.Framework;
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
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                           .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                                           .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UsePaymentPlanPayment(1337L)
                                                           .PrepareRequest();

            Assert.AreEqual(1337L, request.CreateOrderInformation.CreatePaymentPlanDetails.CampaignCode);
            Assert.AreEqual(false,
                            request.CreateOrderInformation.CreatePaymentPlanDetails.SendAutomaticGiroPaymentForm);
        }

        [Test]
        public void TestPaymentPlanFailCompanyCustomer()
        {
            try
            {
                WebpayConnection.CreateOrder()
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
                Assert.AreEqual("ERROR - CompanyCustomer is not allowed to use payment plan option.", ex.Message);
            }
        }
    }
}