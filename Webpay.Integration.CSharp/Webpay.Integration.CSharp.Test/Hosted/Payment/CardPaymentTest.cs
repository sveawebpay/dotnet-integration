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

            Assert.That(excluded.Count, Is.EqualTo(21));
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

            Assert.That(amount, Is.EqualTo(expectedAmount));
            Assert.That(vat, Is.EqualTo(expectedVat));
        }

        [Test]
        public void TestBuildCardPaymentWithCustomer()
        {
            PaymentForm form = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                     .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                     .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                     .AddDiscount(TestingTool.CreateRelativeDiscount())
                                     .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                     .SetOrderDate(TestingTool.DefaultTestDate)
                                     .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                     .SetCurrency(TestingTool.DefaultTestCurrency)
                                     .UsePaymentMethod(PaymentMethod.SVEACARDPAY_PF)
                                     .SetReturnUrl("http://myurl.se")
                                     .GetPaymentForm();

            string xml = form.GetXmlMessage();

            const string expectedCustomer = "<customer><ssn>194605092222</ssn><firstname>Tess</firstname><lastname>Persson</lastname><initials>SB</initials><phone>0811111111</phone><email>test@svea.com</email><address>Testgatan</address><housenumber>1</housenumber><address2>c/o Eriksson, Erik</address2><zip>99999</zip><city>Stan</city><country>SE</country></customer>";

            var startIndex = xml.IndexOf("<customer>", System.StringComparison.InvariantCulture);
            var endIndex = xml.IndexOf("</customer>", System.StringComparison.InvariantCulture)+11;
            string customer = xml.Substring(startIndex,
                                          endIndex- startIndex);
            Assert.That(xml.Contains("<customer>"), Is.True);
            Assert.That(customer, Is.EqualTo(expectedCustomer));
        }

        [Test]
        public void TestBuildCardPaymentWithOutCustomer()
        {
            PaymentForm form = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                     .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                     .AddDiscount(TestingTool.CreateRelativeDiscount())
                                     .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                     .SetOrderDate(TestingTool.DefaultTestDate)
                                     .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                     .SetCurrency(TestingTool.DefaultTestCurrency)
                                     .UsePaymentMethod(PaymentMethod.SVEACARDPAY_PF)
                                     .SetReturnUrl("http://myurl.se")
                                     .GetPaymentForm();

            string xml = form.GetXmlMessage();

            Assert.That(xml.Contains("<customer>"), Is.False);
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

            Assert.That(amount, Is.EqualTo(expectedAmount));
            Assert.That(vat, Is.EqualTo(expectedVat));
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

            Assert.That(form.GetMerchantId(), Is.EqualTo(expectedMerchantId));
            Assert.That(form.GetSecretWord(), Is.EqualTo(expectedSecretWord));
        }
    }
}