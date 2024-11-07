using Webpay.Integration.Config;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.Test.Hosted;

[TestFixture]
public class HostedPaymentsRequestTest
{
    [Test]
    public void TestDoCardPaymentRequest()
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

        Assert.That(form, Is.Not.Null);
    }
}