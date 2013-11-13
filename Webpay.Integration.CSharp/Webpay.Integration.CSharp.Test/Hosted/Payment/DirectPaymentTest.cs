using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Security;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.Test.Hosted.Payment
{
    [TestFixture]
    public class DirectPaymentTest
    {
        [Test]
        public void TestConfigureExcludedPaymentMethodSe()
        {
            List<string> excluded = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                    .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                    .UsePayPageDirectBankOnly()
                                                    .ConfigureExcludedPaymentMethod()
                                                    .GetExcludedPaymentMethod();

            Assert.AreEqual(18, excluded.Count);
        }

        [Test]
        public void TestConfigureExcludedPaymentMethodNo()
        {
            List<string> excluded = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                    .SetCountryCode(CountryCode.NO)
                                                    .UsePayPageDirectBankOnly()
                                                    .ConfigureExcludedPaymentMethod()
                                                    .GetExcludedPaymentMethod();

            Assert.AreEqual(22, excluded.Count);
        }

        [Test]
        public void TestBuildDirectBankPayment()
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
                                               .UsePayPageDirectBankOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string base64Payment = form.GetXmlMessageBase64();
            string html = Base64Util.DecodeBase64String(base64Payment);

            Assert.True(html.Contains("<amount>18750</amount>"));
        }

        [Test]
        public void TestBuildDirectBankPaymentNotSe()
        {
            PaymentForm form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                               .AddFee(TestingTool.CreateExVatBasedInvoiceFee())
                                               .AddDiscount(TestingTool.CreateRelativeDiscount())
                                               .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                               .SetCountryCode(CountryCode.DE)
                                               .SetOrderDate(TestingTool.DefaultTestDate)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePayPageDirectBankOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            string base64Payment = form.GetXmlMessageBase64();
            string html = Base64Util.DecodeBase64String(base64Payment);

            Assert.True(html.Contains("<amount>18750</amount>"));
        }
    }
}