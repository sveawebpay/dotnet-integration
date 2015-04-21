using System;
using System.Net;
using System.Web;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin
{
    [TestFixture]
    public class HostedAdminTest
    {
        [Test]
        public void TestPreparedPaymentRequest()
        {
            Uri uri = PrepareRegularPayment(PaymentMethod.NORDEASE, CreateCustomerRefNo());

            Assert.That(uri.AbsoluteUri, Is.StringMatching(".*\\/preparedpayment\\/[0-9]+"));
        }

        [Test, Ignore]
        public void TestGetRecurringPaymentUrl()
        {
            var prepareRecurPayment = PrepareRecurPayment(PaymentMethod.KORTCERT, SubscriptionType.RECURRING);
        }

        [Test, Ignore]
        public void TestGetRecurringCapturePaymentUrl()
        {
            var prepareRecurPayment = PrepareRecurPayment(PaymentMethod.KORTCERT, SubscriptionType.RECURRINGCAPTURE);
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
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.KORTCERT, CreateCustomerRefNo()));

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .Annul(new Annul(
                    transactionId: payment.TransactionId
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
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

            var hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
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

            var hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/confirm/transactionid").InnerText, Is.EqualTo("12341234"));
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/confirm/capturedate").InnerText, Is.EqualTo("2015-05-22"));
        }

        [Test]
        public void TestGetPaymentMethods()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .GetPaymentMethods(new GetPaymentMethods(
                    merchantId: 1130
                ));

            var hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/getpaymentmethods/merchantid").InnerText, Is.EqualTo("1130"));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));

            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/paymentmethods").InnerXml, 
                Is.EqualTo("<paymentmethod>BANKAXESS</paymentmethod><paymentmethod>DBNORDEASE</paymentmethod><paymentmethod>DBSEBSE</paymentmethod><paymentmethod>KORTCERT</paymentmethod><paymentmethod>SVEAINVOICEEU_SE</paymentmethod><paymentmethod>SVEASPLITEU_SE</paymentmethod>"));
        }

        [Test]
        public void TestGetReconciliationReport()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .GetReconciliationReport(new GetReconciliationReport(
                    date: new DateTime(2015, 04, 17)
                ));

            var hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/getreconciliationreport/date").InnerText, Is.EqualTo("2015-04-17"));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));

            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/reconciliation").InnerXml,
                Is.StringStarting("<reconciliationtransaction><transactionid>598268</transactionid><customerrefno>ti-3-183-Nakkilankatu-A3</customerrefno><paymentmethod>KORTCERT</paymentmethod><amount>28420</amount><currency>SEK</currency><time>2015-04-17 00:15:22 CEST</time></reconciliationtransaction>"));
        }

        [Test]
        public void TestLowerAmount()
        {
            var customerRefNo = CreateCustomerRefNo();
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.KORTCERT, customerRefNo));

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .LowerAmount(new LowerAmount(
                    transactionId: payment.TransactionId,
                    amountToLower: 666
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/loweramount/transactionid").InnerText, Is.EqualTo(payment.TransactionId + ""));
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/loweramount/amounttolower").InnerText, Is.EqualTo("666"));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/customerrefno").InnerText, Is.EqualTo(customerRefNo));
        }



        [Test]
        public void TestQueryTransactionIdDirectPayment()
        {
            var customerRefNo = CreateCustomerRefNo();
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.NORDEASE, customerRefNo));
            var now = DateTime.Now;

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .Query(new QueryByTransactionId(
                    transactionId: payment.TransactionId
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/query/transactionid").InnerText, Is.EqualTo(payment.TransactionId + ""));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/customerrefno").InnerText, Is.EqualTo(customerRefNo));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/merchantid").InnerText, Is.EqualTo("1130"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/status").InnerText, Is.EqualTo("SUCCESS"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/amount").InnerText, Is.EqualTo("25000"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/currency").InnerText, Is.EqualTo("SEK"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/vat").InnerText, Is.EqualTo("5000"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/capturedamount").InnerText, Is.EqualTo("25000"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/authorizedamount").InnerText, Is.EqualTo("25000"));

            var created = DateTime.Parse(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/created").InnerText);
            Assert.That(created.Year, Is.EqualTo(now.Year));
            Assert.That(created.Month, Is.EqualTo(now.Month));
            Assert.That(created.Day, Is.EqualTo(now.Day));

            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/creditstatus").InnerText, Is.EqualTo("CREDNONE"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/creditedamount").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/merchantresponsecode").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/paymentmethod").InnerText, Is.EqualTo("DBNORDEASE"));

            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/customer/firstname").InnerXml, Is.EqualTo("TestCompagniet"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/customer/ssn").InnerXml, Is.EqualTo("2345234"));

        }

        [Test]
        public void TestQueryCustomerRefNoDirectPayment()
        {
            var customerRefNo = CreateCustomerRefNo();
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.NORDEASE, customerRefNo));
            var now = DateTime.Now;

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .Query(new QueryByCustomerRefNo(
                    customerRefNo: customerRefNo
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/query/customerrefno").InnerText, Is.EqualTo(customerRefNo));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/transaction/customerrefno").InnerText, Is.EqualTo(customerRefNo));
        }

        [Test]
        public void TestRecur()
        {
            const string customerRefNo = "Customer reference number or client order number";
            const string subscriptionId = "The subscription id";
            const long amount = 66600L;
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .Recur(new Recur(
                    customerRefNo: customerRefNo,
                    subscriptionId: subscriptionId,
                    currency: Currency.SEK,
                    amount: amount 
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/recur/currency").InnerText, Is.EqualTo(Currency.SEK.ToString()));
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/recur/amount").InnerText, Is.EqualTo(amount + ""));
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/recur/customerrefno").InnerText, Is.EqualTo(customerRefNo));
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/recur/subscriptionid").InnerText, Is.EqualTo(subscriptionId));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();

            //Call to non-existing subscription
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("322")); 
        }

        private static Uri PrepareRegularPayment(PaymentMethod paymentMethod, string createCustomerRefNo)
        {
            return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetClientOrderNumber(createCustomerRefNo)
                .SetCurrency(TestingTool.DefaultTestCurrency)
                .UsePaymentMethod(paymentMethod)
                .___SetSimulatorCode_ForTestingOnly("0")
                .SetReturnUrl(
                    "https://test.sveaekonomi.se/webpay/public/static/testlandingpage.html")
                .PreparePayment("127.0.0.1");
        }

        private static string CreateCustomerRefNo()
        {
            return "1" + Guid.NewGuid().ToString().Replace("-", "");
        }

        private static Uri PrepareRecurPayment(PaymentMethod paymentMethod, SubscriptionType subscriptionType)
        {
            return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetClientOrderNumber("1" + Guid.NewGuid().ToString().Replace("-", ""))
                .SetCurrency(TestingTool.DefaultTestCurrency)
                .UsePaymentMethod(paymentMethod)
                .SetSubscriptionType(subscriptionType)
                .___SetSimulatorCode_ForTestingOnly("0")
                .SetReturnUrl(
                    "https://test.sveaekonomi.se/webpay/public/static/testlandingpage.html")
                .PreparePayment("127.0.0.1");
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