using Webpay.Integration.Config;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Test.Order;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.Test.Hosted.Payment;

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
        var excluded = _order
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .UsePayPageCardOnly()
            .ConfigureExcludedPaymentMethod()
            .GetExcludedPaymentMethod();

        Assert.That(excluded.Count, Is.EqualTo(21));
    }

    [Test]
    public void TestBuildCardPayment()
    {
        var form = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
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

        var xml = form.GetXmlMessage();

        const string expectedAmount = "18750";
        const string expectedVat = "3750";

        var amount = xml.Substring(xml.IndexOf("<amount>", StringComparison.InvariantCulture) + 8,
                                      expectedAmount.Length);
        var vat = xml.Substring(xml.IndexOf("<vat>", StringComparison.InvariantCulture) + 5,
                                   expectedVat.Length);

        Assert.That(amount, Is.EqualTo(expectedAmount));
        Assert.That(vat, Is.EqualTo(expectedVat));
    }

    [Test]
    public void TestBuildCardPaymentWithCustomer()
    {
        var form = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
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

        var xml = form.GetXmlMessage();

        const string expectedCustomer = "<customer><ssn>194605092222</ssn><firstname>Tess</firstname><lastname>Persson</lastname><initials>SB</initials><phone>0811111111</phone><email>test@svea.com</email><address>Testgatan</address><housenumber>1</housenumber><address2>c/o Eriksson, Erik</address2><zip>99999</zip><city>Stan</city><country>SE</country></customer>";

        var startIndex = xml.IndexOf("<customer>", StringComparison.InvariantCulture);
        var endIndex = xml.IndexOf("</customer>", StringComparison.InvariantCulture)+11;
        var customer = xml.Substring(startIndex,
                                      endIndex- startIndex);
        Assert.That(xml.Contains("<customer>"), Is.True);
        Assert.That(customer, Is.EqualTo(expectedCustomer));
    }

    [Test]
    public void TestBuildCardPaymentWithOutCustomer()
    {
        var form = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                         .AddFee(TestingTool.CreateExVatBasedShippingFee())
                         .AddDiscount(TestingTool.CreateRelativeDiscount())
                         .SetCountryCode(TestingTool.DefaultTestCountryCode)
                         .SetOrderDate(TestingTool.DefaultTestDate)
                         .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                         .SetCurrency(TestingTool.DefaultTestCurrency)
                         .UsePaymentMethod(PaymentMethod.SVEACARDPAY_PF)
                         .SetReturnUrl("http://myurl.se")
                         .GetPaymentForm();

        var xml = form.GetXmlMessage();

        Assert.That(xml.Contains("<customer>"), Is.False);
    }

    [Test]
    public void TestBuildCardPaymentDe()
    {
        var form = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
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

        var xml = form.GetXmlMessage();

        const string expectedAmount = "18750";
        const string expectedVat = "3750";

        var amount = xml.Substring(xml.IndexOf("<amount>", StringComparison.InvariantCulture) + 8,
                                      expectedAmount.Length);
        var vat = xml.Substring(xml.IndexOf("<vat>", StringComparison.InvariantCulture) + 5,
                                   expectedVat.Length);

        Assert.That(amount, Is.EqualTo(expectedAmount));
        Assert.That(vat, Is.EqualTo(expectedVat));
    }


    [Test]
    public void TestSetAuthorization()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

        const string expectedMerchantId = "1110";
        const string expectedSecretWord = "1f8bcd8a564073f7156efd2522d5998f5487a1dcd19e1e120276fb1fb7e233a6059c45d6eb44a8d7342a4989bbb95acd4708051bbc145bda43ae0dd3503928db";

        Assert.That(form.GetMerchantId(), Is.EqualTo(expectedMerchantId));
        Assert.That(form.GetSecretWord(), Is.EqualTo(expectedSecretWord));
    }
}