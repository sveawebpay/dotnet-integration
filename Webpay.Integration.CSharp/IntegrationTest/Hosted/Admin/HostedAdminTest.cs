using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Payment;

namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin
{
    [TestFixture]
    public class HostedAdminTest
    {
        /// <summary>
        /// Here or in the othere tests?
        /// </summary>
        [Test]
        public void TestPreparedPaymentRequest()
        {
            Uri uri = PrepareRegularPayment(PaymentMethod.NORDEASE);

            Assert.That(uri.AbsoluteUri, Is.StringMatching(".*\\/preparedpayment\\/[0-9]+"));
        }

        private static Uri PrepareRegularPayment(PaymentMethod paymentMethod)
        {
            return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetClientOrderNumber("1" + Guid.NewGuid().ToString().Replace("-", ""))
                .SetCurrency(TestingTool.DefaultTestCurrency)
                .UsePaymentMethod(paymentMethod)
                .___SetSimulatorCode_ForTestingOnly("0")
                .SetReturnUrl(
                    "https://test.sveaekonomi.se/webpay/public/static/testlandingpage.html")
                .PreparePayment("127.0.0.1");
        }

        private static Uri PrepareRecurPayment(PaymentMethod paymentMethod, RecurringPayment recurringPayment)
        {
            return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetClientOrderNumber("1" + Guid.NewGuid().ToString().Replace("-", ""))
                .SetCurrency(TestingTool.DefaultTestCurrency)
                .UsePaymentMethod(paymentMethod)
                .CardPayment()
                .SetRecurMode(recurringPayment)
                .SetReturnUrl(
                    "https://test.sveaekonomi.se/webpay/public/static/testlandingpage.html")
                .PreparePayment("127.0.0.1");
        }

        [Test, Ignore]
        public void TestGetRecurringPaymentUrl()
        {
            var prepareRecurPayment = PrepareRecurPayment(PaymentMethod.KORTCERT, RecurringPayment.RECURRING);
        }

        [Test, Ignore]
        public void TestGetRecurringCapturePaymentUrl()
        {
            var prepareRecurPayment = PrepareRecurPayment(PaymentMethod.KORTCERT, RecurringPayment.RECURRINGCAPTURE);
        }

        [Test, Ignore]
        public void TestSemiManualCancelRecurSubscription()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .CancelRecurSubscription(new CancelRecurSubscription(
                    subscriptionId: "3352"
                ))
                .DoRequest();
        }

        [Test, Ignore]
        public void TestSemiManualConfirm()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .Confirm(new Confirm(
                    transactionId: 598683,
                    captureDate: DateTime.Now
                ))
                .DoRequest();
        }


        [Test]
        public void TestAnnul()
        {
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.KORTCERT));

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .Annul(new Annul(
                    transactionId: payment.TransactionId
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.Request();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/annul/transactionid").InnerText, Is.EqualTo(payment.TransactionId + ""));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
        }

        [Test]
        public void TestCancelRecurSubscription()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .CancelRecurSubscription(new CancelRecurSubscription(
                    subscriptionId: "12341234"
                ));

            var hostedAdminRequest = preparedHostedAdminRequest.Request();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/cancelrecursubscription/subscriptionid").InnerText, Is.EqualTo("12341234"));
        }

        [Test]
        public void TestConfirm()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .Confirm(new Confirm(
                    transactionId: 12341234,
                    captureDate: new DateTime(2015, 05, 22)
                ));

            var hostedAdminRequest = preparedHostedAdminRequest.Request();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/confirm/transactionid").InnerText, Is.EqualTo("12341234"));
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/confirm/capturedate").InnerText, Is.EqualTo("2015-05-22"));
        }

        private PaymentResponse MakePreparedPayment(Uri preparePayment)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(preparePayment);
            webRequest.AllowAutoRedirect = false;
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var location = new Uri(webResponse.Headers["Location"]);
            var nameValueCollection = HttpUtility.ParseQueryString(location.Query);
            var messageBase64 = nameValueCollection["response"];
            var merchantId = nameValueCollection["merchantid"];
            var mac = nameValueCollection["mac"];

            return new PaymentResponse(messageBase64, mac, merchantId);
        }
    }
}