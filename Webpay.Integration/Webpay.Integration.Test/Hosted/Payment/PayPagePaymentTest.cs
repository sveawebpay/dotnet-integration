using System.Text.RegularExpressions;
using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.Test.Hosted.Payment;

[TestFixture]
public class PayPagePaymentTest
{
    [Test]
    public void TestSetExcludePaymentMethod()
    {
        var excludePaymentMethod = new List<PaymentMethod>
        {
            PaymentMethod.INVOICE,
            PaymentMethod.NORDEASE
        };

        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage()
            .ExcludePaymentMethod(excludePaymentMethod);

        var expectedValues = new List<string>
        {
            InvoiceType.INVOICESE.Value,
            InvoiceType.INVOICEEUSE.Value,
            InvoiceType.INVOICENO.Value,
            InvoiceType.INVOICEDK.Value,
            InvoiceType.INVOICEFI.Value,
            InvoiceType.INVOICENL.Value,
            InvoiceType.INVOICEDE.Value,
            PaymentMethod.NORDEASE.Value,
        };

        Assert.That(payPagePayment.GetExcludedPaymentMethod(), Is.EqualTo(expectedValues));
    }

    [Test]
    public void TestSetExcludePaymentMethodDefaultConfigurationNoExcluded()
    {
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .UsePayPage()
            .ExcludePaymentMethod();

        payPagePayment.IncludePaymentMethod();

        Assert.That(payPagePayment.GetExcludedPaymentMethod().Count, Is.EqualTo(24));
        Assert.That(payPagePayment.GetIncludedPaymentMethod().Count, Is.EqualTo(0));
    }

    [Test]
    public void TestDefaultSe()
    {
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage();

        payPagePayment.ConfigureExcludedPaymentMethod();

        Assert.That(payPagePayment.GetExcludedPaymentMethod().Count, Is.EqualTo(0));
    }

    [Test]
    public void TestSetPaymentMethode()
    {
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage()
            .SetPaymentMethod(PaymentMethod.INVOICE);

        payPagePayment.ConfigureExcludedPaymentMethod();

        Assert.That(payPagePayment.GetPaymentMethod(), Is.EqualTo(InvoiceType.INVOICEEUSE.Value));
    }

    [Test]
    public void TestSetPaymentMethodDe()
    {
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.DE)
            .UsePayPage()
            .SetPaymentMethod(PaymentMethod.INVOICE);

        payPagePayment.ConfigureExcludedPaymentMethod();

        Assert.That(payPagePayment.GetPaymentMethod(), Is.EqualTo(InvoiceType.INVOICEDE.Value));
    }

    [Test]
    public void TestSetPaymentMethodPaymentPlanSe()
    {
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage()
            .SetPaymentMethod(PaymentMethod.PAYMENTPLAN);

        payPagePayment.ConfigureExcludedPaymentMethod();

        Assert.That(payPagePayment.GetPaymentMethod(), Is.EqualTo(PaymentPlanType.PAYMENTPLANEUSE.Value));
    }

    [Test]
    public void TestExcludePaymentPlanSe()
    {
        var excludeList = new List<PaymentMethod> { PaymentMethod.PAYMENTPLAN };
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage()
            .ExcludePaymentMethod(excludeList);

        payPagePayment.ConfigureExcludedPaymentMethod();

        Assert.That(payPagePayment.GetExcludedPaymentMethod()[0], Is.EqualTo(PaymentPlanType.PAYMENTPLANEUSE.Value));
    }

    [Test]
    public void TestSetPaymentMethodPaymentPlanNl()
    {
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(CountryCode.NL)
            .UsePayPage()
            .SetPaymentMethod(PaymentMethod.PAYMENTPLAN);

        payPagePayment.ConfigureExcludedPaymentMethod();

        Assert.That(payPagePayment.GetPaymentMethod(), Is.EqualTo(PaymentPlanType.PAYMENTPLANNL.Value));
    }

    [Test]
    public void TestExcludeCardPaymentMethod()
    {
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage()
            .ExcludeCardPaymentMethod();

        Assert.That(payPagePayment.GetExcludedPaymentMethod().Count, Is.EqualTo(3));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[0], Is.EqualTo(PaymentMethod.KORTCERT.Value));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[1], Is.EqualTo(PaymentMethod.SVEACARDPAY.Value));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[2], Is.EqualTo(PaymentMethod.SKRILL.Value));
    }

    [Test]
    public void TestIncludeCardPaymentMethod()
    {
        var includedPaymentMethod = new List<PaymentMethod>
        {
            PaymentMethod.KORTCERT,
            PaymentMethod.SKRILL
        };

        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage()
            .IncludePaymentMethod(includedPaymentMethod);

        Assert.That(payPagePayment.GetIncludedPaymentMethod().Count, Is.EqualTo(2));
    }

    [Test]
    public void TestExcludeDirectPaymentMethodSmall()
    {
        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage()
            .ExcludeDirectPaymentMethod();

        Assert.That(payPagePayment.GetExcludedPaymentMethod().Count, Is.EqualTo(6));
        Assert.That(payPagePayment.GetIncludedPaymentMethod().Count, Is.EqualTo(0));
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

        var payPagePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPage()
            .IncludePaymentMethod(includedPaymentMethod);

        Assert.That(payPagePayment.GetExcludedPaymentMethod().Count, Is.EqualTo(15));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[0], Is.EqualTo("SVEAINVOICESE"));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[1], Is.EqualTo("SVEASPLITSE"));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[2], Is.EqualTo("SVEAINVOICEEU_DE"));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[3], Is.EqualTo("SVEASPLITEU_DE"));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[4], Is.EqualTo("SVEAINVOICEEU_DK"));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[12], Is.EqualTo("SVEACARDPAY"));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[13], Is.EqualTo("PAYPAL"));
        Assert.That(payPagePayment.GetExcludedPaymentMethod()[14], Is.EqualTo("BANKAXESS"));
    }

    [Test]
    public void TestBuildPayPagePaymentWithExcludePaymentMethod()
    {
        var paymentMethods = new List<PaymentMethod>
        {
            PaymentMethod.INVOICE,
            PaymentMethod.KORTCERT
        };

        var form = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddFee(Item.ShippingFee()
                .SetAmountExVat(50)
                .SetShippingId("33")
                .SetDescription("Specification")
                .SetVatPercent(25))
            .AddDiscount(TestingTool.CreateRelativeDiscount())
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePayPage()
            .ExcludePaymentMethod(paymentMethods)
            .SetReturnUrl("http://myurl.se")
            .GetPaymentForm();

        var xml = form.GetXmlMessage();

        const string regexPattern =
            @"<\?xml version=\""1\.0\"" encoding=\""utf-8\""\?><!--Message generated by Integration package C#-->" +
            @"<payment><customerrefno>[a-zA-Z0-9\-]+</customerrefno><currency>SEK</currency>" +
            @"<amount>18750</amount><vat>3750</vat><lang>en</lang>" +
            @"<returnurl>http://myurl.se</returnurl><iscompany>false</iscompany>" +
            @"<customer><ssn>194605092222</ssn><country>SE</country></customer>" +
            @"<orderrows><row><sku>1</sku><name>Prod</name><description>Specification</description>" +
            @"<amount>12500</amount><vat>2500</vat><quantity>2</quantity>" +
            @"<unit>st</unit></row><row><sku>33</sku><name /><description>Specification</description>" +
            @"<amount>6250</amount><vat>1250</vat><quantity>1</quantity></row>" +
            @"<row><sku>1</sku><name>Relative</name><description>RelativeDiscount</description>" +
            @"<amount>-12500</amount><vat>-2500</vat><quantity>1</quantity><unit>st</unit></row>" +
            @"</orderrows><excludepaymentMethods><exclude>SVEAINVOICESE</exclude>" +
            @"<exclude>SVEAINVOICEEU_SE</exclude><exclude>SVEAINVOICEEU_NO</exclude>" +
            @"<exclude>SVEAINVOICEEU_DK</exclude><exclude>SVEAINVOICEEU_FI</exclude>" +
            @"<exclude>SVEAINVOICEEU_NL</exclude><exclude>SVEAINVOICEEU_DE</exclude>" +
            @"<exclude>KORTCERT</exclude></excludepaymentMethods>" +
            @"<addinvoicefee>false</addinvoicefee></payment>";

        Assert.That(xml, Does.Match(new Regex(regexPattern, RegexOptions.Singleline)));
    }

    [Test]
    public void TestPayPagePaymentExcludeCardPayments()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

        var xml = form.GetXmlMessage();
        var paymentMethod = xml.Substring(
            xml.IndexOf("KORTCERT", StringComparison.InvariantCulture), "KORTCERT".Length);
        var paymentMethod2 = xml.Substring(
            xml.IndexOf("SKRILL", StringComparison.InvariantCulture), "SKRILL".Length);

        Assert.That(paymentMethod, Is.EqualTo(PaymentMethod.KORTCERT.Value));
        Assert.That(paymentMethod2, Is.EqualTo("SKRILL"));
    }

    [Test]
    public void TestExcludeDirectPaymentMethodLarge()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

        var xml = form.GetXmlMessage();
        var paymentMethod = xml.Substring(
            xml.IndexOf("DBNORDEASE", StringComparison.InvariantCulture), "DBNORDEASE".Length);

        Assert.That(paymentMethod, Is.EqualTo(PaymentMethod.NORDEASE.Value));
    }

    [Test]
    public void TestIncludePaymentPlan()
    {
        var paymentMethods = new List<PaymentMethod>
        {
            PaymentMethod.PAYMENTPLAN,
            PaymentMethod.SKRILL
        };

        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

        var xml = form.GetXmlMessage();
        var paymentMethod = xml.Substring(
            xml.IndexOf("SVEAINVOICESE", StringComparison.InvariantCulture), "SVEAINVOICESE".Length);

        Assert.That(paymentMethod, Is.EqualTo(InvoiceType.INVOICESE.Value));
    }

    [Test]
    public void TestPayPagePaymentIncludePaymentMethod()
    {
        var paymentMethods = new List<PaymentMethod>
        {
            PaymentMethod.KORTCERT,
            PaymentMethod.SKRILL
        };

        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

        var xml = form.GetXmlMessage();
        var paymentMethod = xml.Substring(
            xml.IndexOf("SVEAINVOICESE", StringComparison.InvariantCulture), "SVEAINVOICESE".Length);

        Assert.That(paymentMethod, Is.EqualTo(InvoiceType.INVOICESE.Value));
    }

    [Test]
    public void TestPayPagePaymentIncludePaymentMethodEmpty()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

        var xml = form.GetXmlMessage();
        var paymentMethod = xml.Substring(
            xml.IndexOf("SVEAINVOICESE", StringComparison.InvariantCulture), "SVEAINVOICESE".Length);
        var paymentMethod2 = xml.Substring(
            xml.IndexOf("DBSWEDBANKSE", StringComparison.InvariantCulture), "DBSWEDBANKSE".Length);

        Assert.That(paymentMethod, Is.EqualTo(InvoiceType.INVOICESE.Value));
        Assert.That(paymentMethod2, Is.EqualTo(PaymentMethod.SWEDBANKSE.Value));
    }
}