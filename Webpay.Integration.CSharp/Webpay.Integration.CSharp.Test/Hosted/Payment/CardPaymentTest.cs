using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Test.Order;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.Test.Hosted.Payment
{
    [TestFixture]
    public class CardPaymentTest
    {
        private CreateOrderBuilder _order;

        [SetUp]
        public void SetUp()
        {
            _order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig()).SetValidator(new VoidValidator());
        }

        [Test]
        public void TestConfigureExcludedPaymentMethodSe()
        {
            List<string> excluded = _order
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .UsePayPageCardOnly()
                .ConfigureExcludedPaymentMethod()
                .GetExcludedPaymentMethod();

            Assert.AreEqual(21, excluded.Count);
        }

        [Test]
        public void TestBuildCardPayment()
        {
            PaymentForm form = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                     .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                     .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                     .AddDiscount(TestingTool.CreateRelativeDiscount())
                                     .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                     .SetOrderDate(TestingTool.DefaultTestDate)
                                     .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                     .SetCurrency(TestingTool.DefaultTestCurrency)
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
            PaymentForm form = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                     .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                     .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                     .AddDiscount(TestingTool.CreateRelativeDiscount())
                                     .SetCountryCode(CountryCode.DE)
                                     .SetOrderDate(TestingTool.DefaultTestDate)
                                     .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                     .SetCurrency(TestingTool.DefaultTestCurrency)
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
            PaymentForm form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                               .AddFee(TestingTool.CreateExVatBasedInvoiceFee())
                                               .AddDiscount(TestingTool.CreateRelativeDiscount())
                                               .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetOrderDate(TestingTool.DefaultTestDate)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
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