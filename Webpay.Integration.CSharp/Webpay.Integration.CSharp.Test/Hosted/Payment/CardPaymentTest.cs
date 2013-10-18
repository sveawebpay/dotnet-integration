using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Test.Order;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Hosted.Payment
{
    [TestFixture]
    public class CardPaymentTest
    {
        private CreateOrderBuilder _order;

        [SetUp]
        public void SetUp()
        {
            _order = WebpayConnection.CreateOrder().SetValidator(new VoidValidator());
        }

        [Test]
        public void TestConfigureExcludedPaymentMethodSe()
        {
            List<string> excluded = _order
                .SetCountryCode(CountryCode.SE)
                .UsePayPageCardOnly()
                .ConfigureExcludedPaymentMethod()
                .GetExcludedPaymentMethod();

            Assert.AreEqual(21, excluded.Count);
        }

        [Test]
        public void TestBuildCardPayment()
        {
            PaymentForm form = _order.AddOrderRow(Item.OrderRow()
                                                      .SetAmountExVat(100.00M)
                                                      .SetArticleNumber("1")
                                                      .SetQuantity(2)
                                                      .SetUnit("st")
                                                      .SetDescription("Specification")
                                                      .SetVatPercent(25)
                                                      .SetDiscountPercent(0)
                                                      .SetName("Prod"))
                                     .AddCustomerDetails(Item.CompanyCustomer()
                                                             .SetVatNumber("2345234")
                                                             .SetCompanyName("TestCompagniet"))
                                     .AddFee(Item.ShippingFee()
                                                 .SetShippingId("33")
                                                 .SetName("shipping")
                                                 .SetDescription("Specification")
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
                                     .SetCountryCode(CountryCode.SE)
                                     .SetOrderDate("2012-12-12")
                                     .SetClientOrderNumber("33")
                                     .SetCurrency(Currency.SEK)
                                     .UsePayPageCardOnly()
                                     .SetReturnUrl("http://myurl.se")
                                     .GetPaymentForm();

            string xml = form.GetXmlMessage();

            const string expectedAmount = "18750";
            const string expectedVat = "3750";

            string amount = xml.Substring(xml.IndexOf("<amount>", System.StringComparison.InvariantCulture) + 8,
                                          expectedAmount.Length);
            string vat = xml.Substring(xml.IndexOf("<vat>", System.StringComparison.InvariantCulture) + 5,
                                       expectedVat.Length);

            Assert.AreEqual(expectedAmount, amount);
            Assert.AreEqual(expectedVat, vat);
        }

        [Test]
        public void TestBuildCardPaymentDe()
        {
            PaymentForm form = _order.AddOrderRow(Item.OrderRow()
                                                      .SetAmountExVat(100.00M)
                                                      .SetArticleNumber("1")
                                                      .SetQuantity(2)
                                                      .SetUnit("st")
                                                      .SetDescription("Specification")
                                                      .SetVatPercent(25)
                                                      .SetDiscountPercent(0)
                                                      .SetName("Prod"))
                                     .AddCustomerDetails(Item.CompanyCustomer()
                                                             .SetVatNumber("2345234")
                                                             .SetCompanyName("TestCompagniet"))
                                     .AddFee(Item.ShippingFee()
                                                 .SetShippingId("33")
                                                 .SetName("shipping")
                                                 .SetDescription("Specification")
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
                                     .SetCountryCode(CountryCode.DE)
                                     .SetOrderDate("2012-12-12")
                                     .SetClientOrderNumber("33")
                                     .SetCurrency(Currency.SEK)
                                     .UsePayPageCardOnly()
                                     .SetReturnUrl("http://myurl.se")
                                     .GetPaymentForm();

            string xml = form.GetXmlMessage();

            const string expectedAmount = "18750";
            const string expectedVat = "3750";

            string amount = xml.Substring(xml.IndexOf("<amount>", System.StringComparison.InvariantCulture) + 8,
                                          expectedAmount.Length);
            string vat = xml.Substring(xml.IndexOf("<vat>", System.StringComparison.InvariantCulture) + 5,
                                       expectedVat.Length);

            Assert.AreEqual(expectedAmount, amount);
            Assert.AreEqual(expectedVat, vat);
        }


        [Test]
        public void TestSetAuthorization()
        {
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(Item.OrderRow()
                                                                .SetAmountExVat(100.00M)
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
                                                                .SetName("Svea fee")
                                                                .SetDescription("Fee for invoice")
                                                                .SetUnit("st")
                                                                .SetDiscountPercent(0))
                                               .AddCustomerDetails(Item.CompanyCustomer()
                                                                       .SetVatNumber("2345234")
                                                                       .SetCompanyName("TestCompagniet"))
                                               .SetCountryCode(CountryCode.SE)
                                               .SetOrderDate("2012-12-12")
                                               .SetClientOrderNumber("33")
                                               .SetCurrency(Currency.SEK)
                                               .UsePayPageCardOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            const string expectedMerchantId = "1130";
            const string expectedSecretWord =
                "8a9cece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3";

            Assert.AreEqual(expectedMerchantId, form.GetMerchantId());
            Assert.AreEqual(expectedSecretWord, form.GetSecretWord());
        }
    }
}