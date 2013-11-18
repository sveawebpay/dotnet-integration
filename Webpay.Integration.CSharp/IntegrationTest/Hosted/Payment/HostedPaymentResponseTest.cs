using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Security;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Payment
{
    [TestFixture]
    public class HostedPaymentResponseTest
    {
        private const string ExpectedResponseStart = "<html><head><SCRIPT LANGUAGE='JavaScript'>";

        [Test]
        public void TestDoCardPaymentRequest()
        {
            PaymentForm form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(Guid.NewGuid().ToString().Replace("-", ""))
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePayPageCardOnly()
                                               .SetReturnUrl(
                                                   "https://test.sveaekonomi.se/webpay/admin/merchantresponsetest.xhtml")
                                               .GetPaymentForm();

            var postResponse = PostRequest(form);
            Assert.That(postResponse.Item1, Is.EqualTo("OK"));
        }

        [Test]
        public void TestDoNordeaSePaymentRequest()
        {
            PaymentForm form = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                               .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                               .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(Guid.NewGuid().ToString().Replace("-", ""))
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .UsePaymentMethod(PaymentMethod.NORDEASE)
                                               .SetReturnUrl(
                                                   "https://test.sveaekonomi.se/webpay/admin/merchantresponsetest.xhtml")
                                               .GetPaymentForm();

            var postResponse = PostRequest(form);
            Assert.That(postResponse.Item1, Is.EqualTo("OK"));
            Assert.That(postResponse.Item2.StartsWith(ExpectedResponseStart));
        }

        private static Tuple<string, string> PostRequest(PaymentForm form)
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig());

            form.SetMacSha512(
                HashUtil.CreateHash(form.GetXmlMessageBase64() +
                                    order.GetConfig().GetSecretWord(PaymentType.HOSTED, order.GetCountryCode())));

            string data = "mac=" + HttpUtility.UrlEncode(form.GetMacSha512()) +
                          "&message=" + HttpUtility.UrlEncode(form.GetXmlMessageBase64()) +
                          "&merchantid=" + HttpUtility.UrlEncode(form.GetMerchantId());
            byte[] formData = Encoding.UTF8.GetBytes(data);

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
}