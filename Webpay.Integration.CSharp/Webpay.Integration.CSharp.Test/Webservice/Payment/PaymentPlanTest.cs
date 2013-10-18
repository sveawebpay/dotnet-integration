using NUnit.Framework;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
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
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddFee(Item.ShippingFee()
                                                                       .SetShippingId("33")
                                                                       .SetName("shipping")
                                                                       .SetDescription("Specification")
                                                                       .SetAmountExVat(50)
                                                                       .SetUnit("st")
                                                                       .SetVatPercent(25)
                                                                       .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber("194605092222")
                                                                                   .SetBirthDate("19231212")
                                                                                   .SetName("Tess", "Testson")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("9999")
                                                                                   .SetLocality("Stan"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
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
                                .AddOrderRow(Item.OrderRow()
                                                 .SetArticleNumber("1")
                                                 .SetQuantity(2)
                                                 .SetAmountExVat(100.00M)
                                                 .SetDescription("Specification")
                                                 .SetName("Prod")
                                                 .SetUnit("st")
                                                 .SetVatPercent(25)
                                                 .SetDiscountPercent(0))
                                .AddFee(Item.ShippingFee()
                                            .SetShippingId("33")
                                            .SetName("shipping")
                                            .SetDescription("Specification")
                                            .SetAmountExVat(50)
                                            .SetUnit("st")
                                            .SetVatPercent(25)
                                            .SetDiscountPercent(0))
                                .AddCustomerDetails(Item.CompanyCustomer()
                                                        .SetNationalIdNumber("194605092222")
                                                        .SetCompanyName("Företaget")
                                                        .SetEmail("test@svea.com")
                                                        .SetPhoneNumber("999999")
                                                        .SetIpAddress("123.123.123")
                                                        .SetStreetAddress("Gatan", "23")
                                                        .SetCoAddress("c/o Eriksson")
                                                        .SetZipCode("9999")
                                                        .SetLocality("Stan"))
                                .SetCountryCode(CountryCode.SE)
                                .SetOrderDate("2012-12-12")
                                .SetClientOrderNumber("33")
                                .SetCurrency(Currency.SEK)
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