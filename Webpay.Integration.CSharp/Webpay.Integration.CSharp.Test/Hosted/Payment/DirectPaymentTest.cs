using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Test.Hosted.Payment
{
    [TestFixture]
    public class DirectPaymentTest
    {
        [Test]
        public void TestConfigureExcludedPaymentMethodSe()
        {
            List<string> excluded = WebpayConnection.CreateOrder()
                                                    .SetCountryCode(CountryCode.SE)
                                                    .UsePayPageDirectBankOnly()
                                                    .ConfigureExcludedPaymentMethod()
                                                    .GetExcludedPaymentMethod();

            Assert.AreEqual(18, excluded.Count);
        }

        [Test]
        public void TestConfigureExcludedPaymentMethodNo()
        {
            List<string> excluded = WebpayConnection.CreateOrder()
                                                    .SetCountryCode(CountryCode.NO)
                                                    .UsePayPageDirectBankOnly()
                                                    .ConfigureExcludedPaymentMethod()
                                                    .GetExcludedPaymentMethod();

            Assert.AreEqual(22, excluded.Count);
        }

        [Test]
        public void TestBuildDirectBankPayment()
        {
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(Item.OrderRow()
                                                                .SetAmountExVat(new decimal(100.00))
                                                                .SetArticleNumber("1")
                                                                .SetQuantity(2)
                                                                .SetUnit("st")
                                                                .SetDescription("Specification")
                                                                .SetVatPercent(25)
                                                                .SetDiscountPercent(0)
                                                                .SetName("Prod"))
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
                                                                       .SetCompanyName("TestCompagniet"))
                                               .SetCountryCode(CountryCode.SE)
                                               .SetOrderDate("2012-12-12")
                                               .SetClientOrderNumber("33")
                                               .SetCurrency(Currency.SEK)
                                               .UsePayPageDirectBankOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string base64Payment = form.GetXmlMessageBase64();
            string html = Base64Util.DecodeBase64String(base64Payment);

            Assert.True(html.Contains("<amount>18750</amount>"));
        }

        [Test]
        public void TestBuildDirectBankPaymentNotSe()
        {
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(Item.OrderRow()
                                                                .SetAmountExVat(new decimal(100.00))
                                                                .SetArticleNumber("1")
                                                                .SetQuantity(2)
                                                                .SetUnit("st")
                                                                .SetDescription("Specification")
                                                                .SetVatPercent(25)
                                                                .SetDiscountPercent(0)
                                                                .SetName("Prod"))
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
                                                                       .SetCompanyName("TestCompagniet"))
                                               .SetCountryCode(CountryCode.DE)
                                               .SetOrderDate("2012-12-12")
                                               .SetClientOrderNumber("33")
                                               .SetCurrency(Currency.SEK)
                                               .UsePayPageDirectBankOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string base64Payment = form.GetXmlMessageBase64();
            string html = Base64Util.DecodeBase64String(base64Payment);

            Assert.True(html.Contains("<amount>18750</amount>"));
        }
    }
}