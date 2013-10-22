using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Hosted.Payment;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Test.Util;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Hosted.Payment
{
    [TestFixture]
    public class PayPagePaymentTest
    {
        [Test]
        public void TestSetExcludePaymentMethod()
        {
            var excludePaymentMethod = new List<PaymentMethod> {PaymentMethod.INVOICE, PaymentMethod.NORDEASE};

            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage()
                                                            .ExcludePaymentMethod(excludePaymentMethod);

            CollectionAssert.AreEqual(new List<string>
                {
                    InvoiceType.INVOICESE.Value,
                    InvoiceType.INVOICEEUSE.Value,
                    InvoiceType.INVOICENO.Value,
                    InvoiceType.INVOICEDK.Value,
                    InvoiceType.INVOICEFI.Value,
                    InvoiceType.INVOICENL.Value,
                    InvoiceType.INVOICEDE.Value,
                    PaymentMethod.NORDEASE.Value,
                },
                                      payPagePayment.GetExcludedPaymentMethod());
        }

        [Test]
        public void TestSetExcludePaymentMethodDefaultConfigurationNoExcluded()
        {
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .UsePayPage()
                                                            .ExcludePaymentMethod();
            payPagePayment.IncludePaymentMethod();

            Assert.AreEqual(23, payPagePayment.GetExcludedPaymentMethod().Count);
            Assert.AreEqual(0, payPagePayment.GetIncludedPaymentMethod().Count);
        }

        [Test]
        public void TestDefaultSe()
        {
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage();

            payPagePayment.ConfigureExcludedPaymentMethod();

            Assert.AreEqual(0, payPagePayment.GetExcludedPaymentMethod().Count);
        }

        [Test]
        public void TestSetPaymentMethode()
        {
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage()
                                                            .SetPaymentMethod(PaymentMethod.INVOICE);

            payPagePayment.ConfigureExcludedPaymentMethod();

            Assert.AreEqual(InvoiceType.INVOICEEUSE.Value,
                            payPagePayment.GetPaymentMethod());
        }

        [Test]
        public void TestSetPaymentMethodDe()
        {
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(CountryCode.DE)
                                                            .UsePayPage()
                                                            .SetPaymentMethod(PaymentMethod.INVOICE);

            payPagePayment.ConfigureExcludedPaymentMethod();

            Assert.AreEqual(InvoiceType.INVOICEDE.Value,
                            payPagePayment.GetPaymentMethod());
        }

        [Test]
        public void TestSetPaymentMethodPaymentPlanSe()
        {
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage()
                                                            .SetPaymentMethod(PaymentMethod.PAYMENTPLAN);

            payPagePayment.ConfigureExcludedPaymentMethod();

            Assert.AreEqual(PaymentPlanType.PAYMENTPLANEUSE.Value,
                            payPagePayment.GetPaymentMethod());
        }

        [Test]
        public void TestExcludePaymentPlanSe()
        {
            var list = new List<PaymentMethod> {PaymentMethod.PAYMENTPLAN};
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage()
                                                            .ExcludePaymentMethod(list);

            payPagePayment.ConfigureExcludedPaymentMethod();

            Assert.AreEqual(PaymentPlanType.PAYMENTPLANEUSE.Value,
                            payPagePayment.GetExcludedPaymentMethod()[0]);
        }

        [Test]
        public void TestSetPaymentMethodPaymentPlanNl()
        {
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(CountryCode.NL)
                                                            .UsePayPage()
                                                            .SetPaymentMethod(PaymentMethod.PAYMENTPLAN);
            payPagePayment.ConfigureExcludedPaymentMethod();

            Assert.AreEqual(PaymentPlanType.PAYMENTPLANNL.Value,
                            payPagePayment.GetPaymentMethod());
        }

        [Test]
        public void TestExcludeCardPaymentMethod()
        {
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage()
                                                            .ExcludeCardPaymentMethod();

            Assert.AreEqual(2, payPagePayment.GetExcludedPaymentMethod().Count);
            Assert.AreEqual(PaymentMethod.KORTCERT.Value,
                            payPagePayment.GetExcludedPaymentMethod()[0]);
            Assert.AreEqual(PaymentMethod.SKRILL.Value,
                            payPagePayment.GetExcludedPaymentMethod()[1]);
        }

        [Test]
        public void TestIncludeCardPaymentMethod()
        {
            var includedPaymentMethod = new List<PaymentMethod> {PaymentMethod.KORTCERT, PaymentMethod.SKRILL};
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage()
                                                            .IncludePaymentMethod(includedPaymentMethod);

            Assert.AreEqual(2, payPagePayment.GetIncludedPaymentMethod().Count);
        }

        [Test]
        public void TestExcludeDirectPaymentMethodSmall()
        {
            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage()
                                                            .ExcludeDirectPaymentMethod();

            Assert.AreEqual(6, payPagePayment.GetExcludedPaymentMethod().Count);
            Assert.AreEqual(0, payPagePayment.GetIncludedPaymentMethod().Count);
        }

        [Test]
        public void TestIncludePaymentMethod()
        {
            var includedPaymentMethod = new List<PaymentMethod>
                {
                    PaymentMethod.KORTCERT,
                    PaymentMethod.SKRILL,
                    PaymentMethod.INVOICE,
                    PaymentMethod.PAYMENTPLAN,
                    PaymentMethod.SWEDBANKSE,
                    PaymentMethod.SHBSE,
                    PaymentMethod.SEBFTGSE,
                    PaymentMethod.SEBSE,
                    PaymentMethod.NORDEASE
                };

            PayPagePayment payPagePayment = WebpayConnection.CreateOrder()
                                                            .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                            .UsePayPage()
                                                            .IncludePaymentMethod(includedPaymentMethod);

            Assert.AreEqual(14, payPagePayment.GetExcludedPaymentMethod().Count);
            Assert.AreEqual("SVEAINVOICESE", payPagePayment.GetExcludedPaymentMethod()[0]);
            Assert.AreEqual("SVEASPLITSE", payPagePayment.GetExcludedPaymentMethod()[1]);
            Assert.AreEqual("SVEAINVOICEEU_DE", payPagePayment.GetExcludedPaymentMethod()[2]);
            Assert.AreEqual("SVEASPLITEU_DE", payPagePayment.GetExcludedPaymentMethod()[3]);
            Assert.AreEqual("SVEAINVOICEEU_DK", payPagePayment.GetExcludedPaymentMethod()[4]);
            Assert.AreEqual("PAYPAL", payPagePayment.GetExcludedPaymentMethod()[12]);
            Assert.AreEqual("BANKAXESS", payPagePayment.GetExcludedPaymentMethod()[13]);
        }

        [Test]
        public void TestBuildPayPagePaymentWithExcludePaymentMethod()
        {
            var paymentMethods = new List<PaymentMethod> {PaymentMethod.INVOICE, PaymentMethod.KORTCERT};

            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddFee(Item.ShippingFee()
                                                           .SetAmountExVat(50)
                                                           .SetShippingId("33")
                                                           .SetDescription("Specification")
                                                           .SetVatPercent(25))
                                               .AddDiscount(TestingTool.CreateRelativeDiscount())
                                               .AddCustomerDetails(
                                                   Item.IndividualCustomer().SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetOrderDate(TestingTool.DefaultTestDate)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePayPage()
                                               .ExcludePaymentMethod(paymentMethods)
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string xml = form.GetXmlMessage();
            const string expected =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><!--Message generated by Integration package C#--><payment><currency>SEK</currency><amount>18750</amount><vat>3750</vat><lang>en</lang><returnurl>http://myurl.se</returnurl><iscompany>false</iscompany><customer><ssn>194605092222</ssn><country>SE</country></customer><orderrows><row><sku>1</sku><name>Prod</name><description>Specification</description><amount>12500</amount><vat>2500</vat><quantity>2</quantity><unit>st</unit></row><row><sku>33</sku><name /><description>Specification</description><amount>6250</amount><vat>1250</vat><quantity>1</quantity></row><row><sku>1</sku><name>Relative</name><description>RelativeDiscount</description><amount>-12500</amount><vat>-2500</vat><quantity>1</quantity><unit>st</unit></row></orderrows><excludepaymentMethods><exclude>SVEAINVOICESE</exclude><exclude>SVEAINVOICEEU_SE</exclude><exclude>SVEAINVOICEEU_NO</exclude><exclude>SVEAINVOICEEU_DK</exclude><exclude>SVEAINVOICEEU_FI</exclude><exclude>SVEAINVOICEEU_NL</exclude><exclude>SVEAINVOICEEU_DE</exclude><exclude>KORTCERT</exclude></excludepaymentMethods><addinvoicefee>false</addinvoicefee></payment>";
            Assert.AreEqual(expected, xml);
        }

        [Test]
        public void TestPayPagePaymentExcludeCardPayments()
        {
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddDiscount(TestingTool.CreateRelativeDiscount())
                                               .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetOrderDate(TestingTool.DefaultTestDate)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePayPage()
                                               .ExcludeCardPaymentMethod()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string xml = form.GetXmlMessage();
            string paymentMethod = xml.Substring(xml.IndexOf("KORTCERT", System.StringComparison.InvariantCulture),
                                                 "KORTCERT".Length);
            string paymentMethod2 = xml.Substring(xml.IndexOf("SKRILL", System.StringComparison.InvariantCulture),
                                                  "SKRILL".Length);

            Assert.AreEqual(PaymentMethod.KORTCERT.Value, paymentMethod);
            Assert.AreEqual("SKRILL", paymentMethod2);
        }

        [Test]
        public void TestExcludeDirectPaymentMethodLarge()
        {
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddDiscount(TestingTool.CreateRelativeDiscount())
                                               .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetOrderDate(TestingTool.DefaultTestDate)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePayPage()
                                               .ExcludeDirectPaymentMethod()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();


            string xml = form.GetXmlMessage();
            string paymentMethod = xml.Substring(xml.IndexOf("DBNORDEASE", System.StringComparison.InvariantCulture),
                                                 "DBNORDEASE".Length);
            Assert.AreEqual(PaymentMethod.NORDEASE.Value, paymentMethod);
        }

        [Test]
        public void TestIncludePaymentPlan()
        {
            var paymentMethods = new List<PaymentMethod> {PaymentMethod.PAYMENTPLAN, PaymentMethod.SKRILL};
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddDiscount(TestingTool.CreateRelativeDiscount())
                                               .AddCustomerDetails(
                                                   Item.IndividualCustomer().SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetOrderDate(TestingTool.DefaultTestDate)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePayPage()
                                               .IncludePaymentMethod(paymentMethods)
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string xml = form.GetXmlMessage();
            string paymentMethod = xml.Substring(
                xml.IndexOf("SVEAINVOICESE", System.StringComparison.InvariantCulture), "SVEAINVOICESE".Length);
            //check to see if the first value is one of the excluded ones
            Assert.AreEqual(InvoiceType.INVOICESE.Value, paymentMethod);
        }

        [Test]
        public void TestPayPagePaymentIncludePaymentMethod()
        {
            var paymentMethods = new List<PaymentMethod> {PaymentMethod.KORTCERT, PaymentMethod.SKRILL};
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddDiscount(TestingTool.CreateRelativeDiscount())
                                               .AddCustomerDetails(
                                                   Item.IndividualCustomer().SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetOrderDate(TestingTool.DefaultTestDate)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePayPage()
                                               .IncludePaymentMethod(paymentMethods)
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string xml = form.GetXmlMessage();
            string paymentMethod = xml.Substring(
                xml.IndexOf("SVEAINVOICESE", System.StringComparison.InvariantCulture), "SVEAINVOICESE".Length);
            //check to see if the first value is one of the excluded ones
            Assert.AreEqual(InvoiceType.INVOICESE.Value, paymentMethod);
        }

        [Test]
        public void TestPayPagePaymentIncludePaymentMethodEmpty()
        {
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddDiscount(TestingTool.CreateRelativeDiscount())
                                               .AddCustomerDetails(
                                                   Item.IndividualCustomer().SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetOrderDate(TestingTool.DefaultTestDate)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePayPage()
                                               .IncludePaymentMethod()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string xml = form.GetXmlMessage();
            string paymentMethod = xml.Substring(
                xml.IndexOf("SVEAINVOICESE", System.StringComparison.InvariantCulture), "SVEAINVOICESE".Length);
            string paymentMethod2 = xml.Substring(
                xml.IndexOf("DBSWEDBANKSE", System.StringComparison.InvariantCulture), "DBSWEDBANKSE".Length);

            Assert.AreEqual(InvoiceType.INVOICESE.Value, paymentMethod);
            Assert.AreEqual(PaymentMethod.SWEDBANKSE.Value,
                            paymentMethod2);
        }
    }
}