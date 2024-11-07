using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Webpay.Integration.Config;
using Webpay.Integration.Hosted.Helper;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Security;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.IntegrationTest.Hosted.Payment;

[TestFixture]
public class HostedPaymentResponseTest
{
    private const string ExpectedResponseStart = "<html><head><SCRIPT LANGUAGE='JavaScript'>";

    [Test]
    public void TestDoCardPaymentRequest()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                   .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                   .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                   .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                   .SetClientOrderNumber(Guid.NewGuid().ToString().Replace("-", ""))
                                   .SetCurrency(TestingTool.DefaultTestCurrency)
                                   .UsePayPageCardOnly()
                                   .SetReturnUrl("https://webpaypaymentgatewaystage.svea.com/webpay/admin/merchantresponsetest.xhtml")
                                   .GetPaymentForm();

        var postResponse = PostRequest(form);
        Assert.That(postResponse.Item1, Is.EqualTo("OK"));
    }

    [Test]
    public void TestDoVippsPaymentRequest()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                   .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                   .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                   .SetCountryCode(CountryCode.NO)
                                   .SetClientOrderNumber(Guid.NewGuid().ToString().Replace("-", ""))
                                   .SetCurrency(Currency.NOK)
                                   .SetPayerAlias(TestingTool.DefaultTestVippsPayerAlias)
                                   .SetMessage(TestingTool.DefaultTestPayerAlias)
                                   .UsePaymentMethod(PaymentMethod.VIPPS)
                                   .SetReturnUrl("https://webpaypaymentgatewaystage.svea.com/webpay/admin/merchantresponsetest.xhtml")
                                   .GetPaymentForm();
        var config = SveaConfig.GetDefaultConfig();
        
        var postResponse = PostRequest(form,CountryCode.NO);
        Assert.That(postResponse.Item1, Is.EqualTo("OK"));
    }

    // In order to run this change response type for merchant 1110 to Post from Get
    [Test,Ignore("")]
    public void TestDoMobilePayPaymentRequest()
    {
        var form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                   .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                   .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                   .SetCountryCode(CountryCode.SE)
                                   .SetClientOrderNumber(Guid.NewGuid().ToString().Replace("-", ""))
                                   .SetCurrency(Currency.SEK)
                                   .SetPayerAlias(TestingTool.DefaultTestMobilePayPayerAlias)
                                   .SetMessage(TestingTool.DefaultTestMobilePayPayerAlias)
                                   .UsePaymentMethod(PaymentMethod.MOBILEPAY)
                                   .SetReturnUrl("https://webpaypaymentgatewaystage.svea.com/webpay/admin/merchantresponsetest.xhtml")
                                   .GetPaymentForm();
        var config = SveaConfig.GetDefaultConfig();

        var postResponse = PostRequest(form);
        Assert.That(postResponse.Item1, Is.EqualTo("OK"));
    }

    [Test]
    public void TestPreparedPaymentRequest()
    {
        var uri = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                  .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                  .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                  .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                  .SetClientOrderNumber(Guid.NewGuid().ToString().Replace("-", ""))
                                  .SetCurrency(TestingTool.DefaultTestCurrency)
                                  .UsePayPageCardOnly()
                                  .SetReturnUrl("https://webpaypaymentgatewaystage.svea.com/webpay/admin/merchantresponsetest.xhtml")
                                  .PreparePayment("127.0.0.1");

        Assert.That(uri.AbsoluteUri, Does.Match(".*\\/preparedpayment\\/[0-9]+"));
    }

    [Test]
    public void TestPreparedPaymentRequestCardPayPF()
    {
        var uri = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                  .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                  .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                  .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                  .SetClientOrderNumber(Guid.NewGuid().ToString().Replace("-", ""))
                                  .SetCurrency(TestingTool.DefaultTestCurrency)
                                  .UsePaymentMethod(PaymentMethod.SVEACARDPAY_PF)
                                  .SetReturnUrl("https://webpaypaymentgatewaystage.svea.com/webpay/admin/merchantresponsetest.xhtml")
                                  .PreparePayment("127.0.0.1");

        Assert.That(uri.AbsoluteUri, Does.Match(".*\\/preparedpayment\\/[0-9]+"));
    }

    /*
     <currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>test_1429280602870</customerrefno><returnurl>https://dev.sveaekonomi.se/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><paymentmethod>DBNORDEASE</paymentmethod><simulatorcode>0</simulatorcode>
     */
    private static Tuple<string, string> PostRequest(PaymentForm form, CountryCode country = CountryCode.SE)
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig());
        form.SetMacSha512(HashUtil.CreateHash(form.GetXmlMessageBase64() +
                                order.GetConfig().GetSecretWord(PaymentType.HOSTED, country)));

        var data = "mac=" + HttpUtility.UrlEncode(form.GetMacSha512()) +
                      "&message=" + HttpUtility.UrlEncode(form.GetXmlMessageBase64()) +
                      "&merchantid=" + HttpUtility.UrlEncode(form.GetMerchantId());
        var formData = Encoding.UTF8.GetBytes(data);

        var request = WebRequest.Create(order.GetConfig().GetEndPoint(PaymentType.HOSTED)) as HttpWebRequest;
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = formData.Length;

        using (Stream post = request.GetRequestStream())
        {
            post.Write(formData, 0, formData.Length);
        }

        string result;
        string statusCode;
        using (var response = request.GetResponse() as HttpWebResponse)
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            statusCode = response.StatusCode.ToString();
        }

        return new Tuple<string, string>(statusCode, result);
    }
}