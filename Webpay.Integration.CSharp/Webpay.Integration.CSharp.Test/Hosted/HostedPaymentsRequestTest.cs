using NUnit.Framework;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Test.Util;

namespace Webpay.Integration.CSharp.Test.Hosted
{
    [TestFixture]
    public class HostedPaymentsRequestTest
    {
        [Test]
        public void TestDoCardPaymentRequest()
        {
            PaymentForm form = WebpayConnection.CreateOrder()
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

            Assert.NotNull(form);
        }
    }
}