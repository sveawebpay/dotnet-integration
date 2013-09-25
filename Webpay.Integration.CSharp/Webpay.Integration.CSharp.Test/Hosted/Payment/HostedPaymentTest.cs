using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Hosted.Payment
{
    [TestFixture]
    public class HostedPaymentTest
    {
        [Test]
        public void TestCalculateRequestValuesNullExtraRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetCountryCode(CountryCode.SE)
                                                       .SetClientOrderNumber("nr22")
                                                       .SetCurrency(Currency.SEK)
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddFee(Item.ShippingFee())
                                                       .AddDiscount(Item.FixedDiscount())
                                                       .AddDiscount(Item.RelativeDiscount());

            var payment = new FakeHostedPayment(order);
            payment
                .SetReturnUrl("myurl")
                .CalculateRequestValues();

            Assert.AreEqual(500, payment.GetAmount());
        }

        [Test]
        public void TestVatPercentAndAmountIncVatCalculation()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetCountryCode(CountryCode.SE)
                                                       .SetClientOrderNumber("nr22")
                                                       .SetCurrency(Currency.SEK)
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(5)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1));

            order.SetShippingFeeRows(null);
            order.SetFixedDiscountRows(null);
            order.SetRelativeDiscountRows(null);
            var payment = new FakeHostedPayment(order);
            payment
                .SetReturnUrl("myUrl")
                .CalculateRequestValues();

            Assert.AreEqual(500, payment.GetAmount());
        }

        [Test]
        public void TestAmountIncVatAndvatPercentShippingFee()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetCountryCode(CountryCode.SE)
                                                       .SetClientOrderNumber("nr22")
                                                       .SetCurrency(Currency.SEK)
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(5)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddFee(Item.ShippingFee()
                                                                   .SetAmountIncVat(5)
                                                                   .SetVatPercent(25));

            order.SetFixedDiscountRows(null);
            order.SetRelativeDiscountRows(null);
            var payment = new FakeHostedPayment(order);
            payment
                .SetReturnUrl("myUrl")
                .CalculateRequestValues();

            Assert.AreEqual(1000L, payment.GetAmount());
        }

        [Test]
        public void TestAmountIncVatAndAmountExVatCalculation()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetCountryCode(CountryCode.SE)
                                                       .SetClientOrderNumber("nr22")
                                                       .SetCurrency(Currency.SEK)
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetAmountIncVat(5)
                                                                        .SetQuantity(1));

            order.SetShippingFeeRows(null);
            order.SetFixedDiscountRows(null);
            order.SetRelativeDiscountRows(null);
            var payment = new FakeHostedPayment(order);
            payment
                .SetReturnUrl("myurl")
                .CalculateRequestValues();


            Assert.That(payment.GetAmount(), Is.EqualTo(500L));
        }

        [Test]
        public void TestCreatePaymentForm()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetCountryCode(CountryCode.SE)
                                                       .SetClientOrderNumber("nr22")
                                                       .SetCurrency(Currency.SEK)
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetNationalIdNumber("666666")
                                                                               .SetEmail("test@svea.com")
                                                                               .SetPhoneNumber("999999")
                                                                               .SetIpAddress("123.123.123.123")
                                                                               .SetStreetAddress("Gatan", "23")
                                                                               .SetCoAddress("c/o Eriksson")
                                                                               .SetZipCode("9999")
                                                                               .SetLocality("Stan"));

            var payment = new FakeHostedPayment(order);
            payment.SetReturnUrl("myurl");

            PaymentForm form = payment.GetPaymentForm();

            var formHtmlFields = form.GetFormHtmlFields();
            Assert.AreEqual("</form>", formHtmlFields["form_end_tag"]);
        }

        [Test]
        public void TestExcludeInvoicesAndAllInstallmentsAllCountries()
        {
            var payment = new FakeHostedPayment(null);
            var exclude = new ExcludePayments();
            List<string> excludedPaymentMethod = payment.GetExcludedPaymentMethod();
            excludedPaymentMethod.AddRange(exclude.ExcludeInvoicesAndPaymentPlan());

            var expected = new List<string>
                {
                    InvoiceType.INVOICESE.Value,
                    InvoiceType.INVOICEEUSE.Value,
                    PaymentPlanType.PAYMENTPLANSE.Value,
                    PaymentPlanType.PAYMENTPLANEUSE.Value,
                    InvoiceType.INVOICEDE.Value,
                    PaymentPlanType.PAYMENTPLANDE.Value,
                    InvoiceType.INVOICEDK.Value,
                    PaymentPlanType.PAYMENTPLANDK.Value,
                    InvoiceType.INVOICEFI.Value,
                    PaymentPlanType.PAYMENTPLANFI.Value,
                    InvoiceType.INVOICENL.Value,
                    PaymentPlanType.PAYMENTPLANNL.Value,
                    InvoiceType.INVOICENO.Value,
                    PaymentPlanType.PAYMENTPLANNO.Value
                };

            CollectionAssert.AreEqual(expected, excludedPaymentMethod);
        }
    }
}