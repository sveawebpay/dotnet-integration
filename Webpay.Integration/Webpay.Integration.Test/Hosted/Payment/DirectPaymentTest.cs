using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Security;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.Test.Hosted.Payment;

[TestFixture]
public class DirectPaymentTest
{
    [Test]
    public void TestConfigureExcludedPaymentMethodSe()
    {
        var excluded = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                       .UsePayPageDirectBankOnly()
                                       .ConfigureExcludedPaymentMethod()
                                       .GetExcludedPaymentMethod();

        Assert.That(excluded.Count, Is.EqualTo(19));
    }

    [Test]
    public void TestConfigureExcludedPaymentMethodNo()
    {
        var excluded = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                .SetCountryCode(CountryCode.NO)
                                                .UsePayPageDirectBankOnly()
                                                .ConfigureExcludedPaymentMethod()
                                                .GetExcludedPaymentMethod();

        Assert.That(excluded.Count, Is.EqualTo(23));
    }

    [Test]
    public void TestBuildDirectBankPayment()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                   .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                   .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                   .AddFee(TestingTool.CreateExVatBasedInvoiceFee())
                                   .AddDiscount(TestingTool.CreateRelativeDiscount())
                                   .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
                                   .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                   .SetOrderDate(TestingTool.DefaultTestDate)
                                   .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                   .SetCurrency(TestingTool.DefaultTestCurrency)
                                   .UsePayPageDirectBankOnly()
                                   .SetReturnUrl("http://myurl.se")
                                   .GetPaymentForm();

        var base64Payment = form.GetXmlMessageBase64();
        var html = Base64Util.DecodeBase64String(base64Payment);

        Assert.That(html.Contains("<amount>18750</amount>"), Is.True);
    }

    [Test]
    public void TestBuildDirectBankPaymentNotSe()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                   .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                   .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                   .AddFee(TestingTool.CreateExVatBasedInvoiceFee())
                                   .AddDiscount(TestingTool.CreateRelativeDiscount())
                                   .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
                                   .SetCountryCode(CountryCode.DE)
                                   .SetOrderDate(TestingTool.DefaultTestDate)
                                   .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                   .SetCurrency(TestingTool.DefaultTestCurrency)
                                   .UsePayPageDirectBankOnly()
                                   .SetReturnUrl("http://myurl.se")
                                   .GetPaymentForm();

        var base64Payment = form.GetXmlMessageBase64();
        var html = Base64Util.DecodeBase64String(base64Payment);

        Assert.That(html.Contains("<amount>18750</amount>"), Is.True);
    }
}