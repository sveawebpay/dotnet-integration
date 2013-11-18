using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Util.Security;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.Test.Hosted.Helper
{
    [TestFixture]
    public class PaymentFormTest
    {
        private const string SecretWord = "secret";
        private const string MerchantId = "1234";

        [Test]
        public void TestSetFormDirectBank()
        {
            string base64Payment = Base64Util.EncodeBase64String("0");
            string mac = HashUtil.CreateHash(base64Payment + SecretWord);
            PaymentForm form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                               .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                               .UsePayPageDirectBankOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();
            form.SetMessageBase64(base64Payment)
                .SetMerchantId(MerchantId)
                .SetSecretWord(SecretWord)
                .SetForm();

            string expected = "<form name=\"paymentForm\" id=\"paymentForm\" method=\"post\" action=\""
                              + form.GetPayPageUrl()
                              + "\">"
                              + "<input type=\"hidden\" name=\"merchantid\" value=\""
                              + MerchantId
                              + "\" />"
                              + "<input type=\"hidden\" name=\"message\" value=\""
                              + base64Payment
                              + "\" />"
                              + "<input type=\"hidden\" name=\"mac\" value=\""
                              + mac
                              + "\" />"
                              +
                              "<noscript><p>Javascript är inaktiverat i er webbläsare, ni får dirigera om till paypage manuellt</p></noscript>"
                              + "<input type=\"submit\" name=\"submit\" value=\"Betala\" />"
                              + "</form>";

            Assert.That(form.GetCompleteForm(), Is.EqualTo(expected));
        }

        [Test]
        public void TestSetFormCard()
        {
            string base64Payment = Base64Util.EncodeBase64String("0");
            string mac = HashUtil.CreateHash(base64Payment + SecretWord);
            PaymentForm form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                               .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                               .UsePayPageCardOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();
            form.SetMessageBase64(base64Payment)
                .SetMerchantId(MerchantId)
                .SetSecretWord(SecretWord)
                .SetForm();

            string expected = "<form name=\"paymentForm\" id=\"paymentForm\" method=\"post\" action=\""
                              + form.GetPayPageUrl()
                              + "\">"
                              + "<input type=\"hidden\" name=\"merchantid\" value=\""
                              + MerchantId
                              + "\" />"
                              + "<input type=\"hidden\" name=\"message\" value=\""
                              + base64Payment
                              + "\" />"
                              + "<input type=\"hidden\" name=\"mac\" value=\""
                              + mac
                              + "\" />"
                              +
                              "<noscript><p>Javascript är inaktiverat i er webbläsare, ni får dirigera om till paypage manuellt</p></noscript>"
                              + "<input type=\"submit\" name=\"submit\" value=\"Betala\" />"
                              + "</form>";

            Assert.That(form.GetCompleteForm(), Is.EqualTo(expected));
        }

        [Test]
        public void TestSetFormCardNoCustomerDetails()
        {
            string base64Payment = Base64Util.EncodeBase64String("0");
            string mac = HashUtil.CreateHash(base64Payment + SecretWord);
            PaymentForm form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                               .UsePayPageCardOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();
            form.SetMessageBase64(base64Payment)
                .SetMerchantId(MerchantId)
                .SetSecretWord(SecretWord)
                .SetForm();

            string expected = "<form name=\"paymentForm\" id=\"paymentForm\" method=\"post\" action=\""
                              + form.GetPayPageUrl()
                              + "\">"
                              + "<input type=\"hidden\" name=\"merchantid\" value=\""
                              + MerchantId
                              + "\" />"
                              + "<input type=\"hidden\" name=\"message\" value=\""
                              + base64Payment
                              + "\" />"
                              + "<input type=\"hidden\" name=\"mac\" value=\""
                              + mac
                              + "\" />"
                              +
                              "<noscript><p>Javascript är inaktiverat i er webbläsare, ni får dirigera om till paypage manuellt</p></noscript>"
                              + "<input type=\"submit\" name=\"submit\" value=\"Betala\" />"
                              + "</form>";

            Assert.That(form.GetCompleteForm(), Is.EqualTo(expected));
        }

        [Test]
        public void TestSetHtmlFields()
        {
            string base64Payment = Base64Util.EncodeBase64String("0");
            string mac = HashUtil.CreateHash(base64Payment + SecretWord);

            PaymentForm form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                               .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                               .UsePayPageDirectBankOnly()
                                               .SetReturnUrl("http://myurl.se")
                                               .GetPaymentForm();

            form.SetMessageBase64(base64Payment)
                .SetMerchantId(MerchantId)
                .SetSecretWord(SecretWord)
                .SetHtmlFields();

            var formHtmlFields = form.GetFormHtmlFields();
            string url = form.GetPayPageUrl();


            Assert.That(formHtmlFields["form_start_tag"], 
                Is.EqualTo("<form name=\"paymentForm\" id=\"paymentForm\" method=\"post\" action=\"" + url + "\">"));

            Assert.That(formHtmlFields["input_merchantId"],
                Is.EqualTo("<input type=\"hidden\" name=\"merchantid\" value=\"" + MerchantId + "\" />"));

            Assert.That(formHtmlFields["input_message"], 
                Is.EqualTo("<input type=\"hidden\" name=\"message\" value=\"" + base64Payment + "\" />"));

            Assert.That(formHtmlFields["input_mac"], Is.EqualTo("<input type=\"hidden\" name=\"mac\" value=\"" + mac + "\" />"));

            Assert.That(formHtmlFields["noscript_p_tag"], 
                Is.EqualTo("<noscript><p>Javascript är inaktiverat i er webbläsare, ni får dirigera om till paypage manuellt</p></noscript>"));

            Assert.That(formHtmlFields["input_submit"], Is.EqualTo("<input type=\"submit\" name=\"submit\" value=\"Betala\" />"));

            Assert.That(formHtmlFields["form_end_tag"], Is.EqualTo("</form>"));
        }
    }
}