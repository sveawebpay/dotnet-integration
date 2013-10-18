using NUnit.Framework;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Hosted
{
    [TestFixture]
    public class HostedPaymentsRequestTest
    {
        [Test]
        public void TestDoCardPaymentRequest()
        {
            PaymentForm form = WebpayConnection.CreateOrder()
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
                                               .AddFee(Item.InvoiceFee()
                                                           .SetName("Svea fee")
                                                           .SetDescription("Fee for invoice")
                                                           .SetAmountExVat(50)
                                                           .SetUnit("st")
                                                           .SetVatPercent(25)
                                                           .SetDiscountPercent(0))
                                               .AddDiscount(Item.RelativeDiscount()
                                                                .SetDiscountId("1")
                                                                .SetName("Relative")
                                                                .SetDescription("RelativeDiscount")
                                                                .SetUnit("st")
                                                                .SetDiscountPercent(50))
                                               .AddCustomerDetails(Item.CompanyCustomer()
                                                                       .SetVatNumber("2345234")
                                                                       .SetNationalIdNumber("4608142222")
                                                                       .SetCompanyName("TestCompagniet"))
                                               .SetCountryCode(CountryCode.SE)
                                               .SetOrderDate("2012-12-12")
                                               .SetClientOrderNumber("33")
                                               .SetCurrency(Currency.SEK)
                                               .UsePayPageCardOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            Assert.NotNull(form);
        }
    }
}