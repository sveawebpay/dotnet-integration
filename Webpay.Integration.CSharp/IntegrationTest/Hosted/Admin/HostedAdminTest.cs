using System;
using System.Net;
using System.Web;
using System.Xml;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Payment;
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
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .CancelRecurSubscription(new CancelRecurSubscription(
                    subscriptionId: "3352"
                ))
                .DoRequest();
        }

        [Test, Ignore]
        public void TestSemiManualConfirm()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
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
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Annul(new Annul(
                    transactionId: payment.TransactionId
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/annul/transactionid").InnerText, Is.EqualTo(payment.TransactionId + ""));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));

            AnnulResponse response = hostedAdminResponse.To(Annul.Response);
        }

        [Test]
        public void TestAnnulResponse()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <transaction id=""598972"">
                    <customerrefno>1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4</customerrefno>
                    </transaction><statuscode>0</statuscode>
                </response>");
            AnnulResponse response = Annul.Response(responseXml);

            Assert.That(response.TransactionId, Is.EqualTo(598972));
            Assert.That(response.CustomerRefNo, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
            Assert.That(response.ClientOrderNumber, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
            Assert.That(response.StatusCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.ErrorMessage, Is.Empty);
        }

        [Test]
        public void TestAnnulResponseFailure()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>107</statuscode>
                </response>");
            AnnulResponse response = Annul.Response(responseXml);

            Assert.That(response.TransactionId, Is.Null);
            Assert.That(response.CustomerRefNo, Is.Null);
            Assert.That(response.ClientOrderNumber, Is.Null);
            Assert.That(response.StatusCode, Is.EqualTo(107));
            Assert.That(response.Accepted, Is.False);
            Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
        }



        [Test]
        public void TestCancelRecurSubscription()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .CancelRecurSubscription(new CancelRecurSubscription(
                    subscriptionId: "12341234"
                ));

            var hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/cancelrecursubscription/subscriptionid").InnerText, Is.EqualTo("12341234"));
        }

        [Test]
        public void TestCancelRecurSubscriptionResponse()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>0</statuscode>
                </response>");
            CancelRecurSubscriptionResponse response = CancelRecurSubscription.Response(responseXml);

            Assert.That(response.StatusCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.ErrorMessage, Is.Empty);
        }

        [Test]
        public void TestCancelRecurSubscriptionResponseFailure()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>101</statuscode>
                </response>");
            CancelRecurSubscriptionResponse response = CancelRecurSubscription.Response(responseXml);

            Assert.That(response.StatusCode, Is.EqualTo(101));
            Assert.That(response.Accepted, Is.False);
            Assert.That(response.ErrorMessage, Is.EqualTo("Invalid XML."));
        }



        [Test]
        public void TestConfirm()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Confirm(new Confirm(
                    transactionId: 12341234,
                    captureDate: new DateTime(2015, 05, 22)
                ));

            var hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/confirm/transactionid").InnerText, Is.EqualTo("12341234"));
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/confirm/capturedate").InnerText, Is.EqualTo("2015-05-22"));
        }

        [Test]
        public void TestConfirmResponse()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <transaction id=""598972"">
                        <customerrefno>1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4</customerrefno>
                    </transaction>
                    <statuscode>0</statuscode>
                </response>");
            ConfirmResponse response = Confirm.Response(responseXml);

            Assert.That(response.TransactionId, Is.EqualTo(598972));
            Assert.That(response.CustomerRefNo, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
            Assert.That(response.ClientOrderNumber, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
            Assert.That(response.StatusCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.ErrorMessage, Is.Empty);
        }

        [Test]
        public void TestConfirmResponseFailure()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>107</statuscode>
                </response>");
            ConfirmResponse response = Confirm.Response(responseXml);

            Assert.That(response.TransactionId, Is.Null);
            Assert.That(response.CustomerRefNo, Is.Null);
            Assert.That(response.ClientOrderNumber, Is.Null);
            Assert.That(response.StatusCode, Is.EqualTo(107));
            Assert.That(response.Accepted, Is.False);
            Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
        }

        [Test]
        public void TestGetPaymentMethods()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .GetPaymentMethods(new GetPaymentMethods(
                    merchantId: 1130
                ));

            var hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/getpaymentmethods/merchantid").InnerText, Is.EqualTo("1130"));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));

            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/paymentmethods").InnerXml, 
                Is.EqualTo("<paymentmethod>BANKAXESS</paymentmethod><paymentmethod>DBNORDEASE</paymentmethod><paymentmethod>DBSEBSE</paymentmethod><paymentmethod>KORTCERT</paymentmethod><paymentmethod>SVEAINVOICEEU_SE</paymentmethod><paymentmethod>SVEASPLITEU_SE</paymentmethod>"));
        }


        [Test]
        public void TestGetPaymentMethodsResponse()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>0</statuscode>
                    <paymentmethods>
                        <paymentmethod>BANKAXESS</paymentmethod>
                        <paymentmethod>DBNORDEASE</paymentmethod>
                    </paymentmethods>
                </response>");
            GetPaymentMethodsResponse response = GetPaymentMethods.Response(responseXml);

            Assert.That(response.StatusCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.ErrorMessage, Is.Empty);
            Assert.That(response.PaymentMethods.Count, Is.EqualTo(2));
            Assert.That(response.PaymentMethods[0], Is.EqualTo("BANKAXESS"));
            Assert.That(response.PaymentMethods[1], Is.EqualTo("DBNORDEASE"));
        }

        [Test]
        public void TestGetPaymentMethodsResponseFailure()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>107</statuscode>
                </response>");
            GetPaymentMethodsResponse response = GetPaymentMethods.Response(responseXml);

            Assert.That(response.StatusCode, Is.EqualTo(107));
            Assert.That(response.Accepted, Is.False);
            Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
            Assert.That(response.PaymentMethods.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestGetReconciliationReport()
        {
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .GetReconciliationReport(new GetReconciliationReport(
                    date: new DateTime(2015, 04, 17)
                ));

            var hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/getreconciliationreport/date").InnerText, Is.EqualTo("2015-04-17"));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));

            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/reconciliation").InnerXml,
                Is.StringStarting("<reconciliationtransaction><transactionid>598268</transactionid><customerrefno>ti-3-183-Nakkilankatu-A3</customerrefno><paymentmethod>KORTCERT</paymentmethod><amount>28420</amount><currency>SEK</currency><time>2015-04-17 00:15:22 CEST</time></reconciliationtransaction>"));

            Assert.That(hostedAdminResponse.To(GetReconciliationReport.Response).ReconciliationTransactions[0].Amount, Is.EqualTo(284.20M));
        }


        [Test]
        public void TestGetReconciliationReportResponse()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>0</statuscode>
                    <reconciliation>
                        <reconciliationtransaction>
                            <transactionid>598268</transactionid>
                            <customerrefno>ti-3-183-Nakkilankatu-A3</customerrefno>
                            <paymentmethod>KORTCERT</paymentmethod>
                            <amount>28420</amount>
                            <currency>SEK</currency>
                            <time>2015-04-17 00:15:22 CEST</time>
                        </reconciliationtransaction>
                    </reconciliation>
                </response>");
            GetReconciliationReportResponse response = GetReconciliationReport.Response(responseXml);

            Assert.That(response.StatusCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.ErrorMessage, Is.Empty);
            Assert.That(response.ReconciliationTransactions.Count, Is.EqualTo(1));
            Assert.That(response.ReconciliationTransactions[0].TransactionId, Is.EqualTo(598268));
            Assert.That(response.ReconciliationTransactions[0].CustomerRefNo, Is.EqualTo("ti-3-183-Nakkilankatu-A3"));
            Assert.That(response.ReconciliationTransactions[0].PaymentMethod, Is.EqualTo("KORTCERT"));
            Assert.That(response.ReconciliationTransactions[0].Amount, Is.EqualTo(284.20M));
            Assert.That(response.ReconciliationTransactions[0].Currency, Is.EqualTo("SEK"));
            Assert.That(response.ReconciliationTransactions[0].Time, Is.EqualTo(new DateTime(2015, 04, 17, 0, 15, 22, DateTimeKind.Unspecified)));
        }

        [Test]
        public void TestGetReconciliationReportResponseFailure()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>107</statuscode>
                </response>");
            GetReconciliationReportResponse response = GetReconciliationReport.Response(responseXml);

            Assert.That(response.StatusCode, Is.EqualTo(107));
            Assert.That(response.Accepted, Is.False);
            Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
            Assert.That(response.ReconciliationTransactions.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestLowerAmount()
        {
            var customerRefNo = CreateCustomerRefNo();
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.KORTCERT, customerRefNo));

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .LowerAmount(new LowerAmount(
                    transactionId: payment.TransactionId,
                    amountToLower: 666
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/loweramount/transactionid").InnerText, Is.EqualTo(payment.TransactionId + ""));
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/loweramount/amounttolower").InnerText, Is.EqualTo("666"));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/customerrefno").InnerText, Is.EqualTo(customerRefNo));
        }


        [Test]
        public void TestLowerAmountResponse()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <transaction id=""598972"">
                        <customerrefno>1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4</customerrefno>
                    </transaction>
                    <statuscode>0</statuscode>
                </response>");
            LowerAmountResponse response = LowerAmount.Response(responseXml);

            Assert.That(response.TransactionId, Is.EqualTo(598972));
            Assert.That(response.CustomerRefNo, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
            Assert.That(response.ClientOrderNumber, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
            Assert.That(response.StatusCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.ErrorMessage, Is.Empty);
        }

        [Test]
        public void TestLowerAmountResponseFailure()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>107</statuscode>
                </response>");
            LowerAmountResponse response = LowerAmount.Response(responseXml);

            Assert.That(response.TransactionId, Is.Null);
            Assert.That(response.CustomerRefNo, Is.Null);
            Assert.That(response.ClientOrderNumber, Is.Null);
            Assert.That(response.StatusCode, Is.EqualTo(107));
            Assert.That(response.Accepted, Is.False);
            Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
        }

        [Test]
        public void TestQueryTransactionIdDirectPayment()
        {
            var customerRefNo = CreateCustomerRefNo();
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.NORDEASE, customerRefNo));
            var now = DateTime.Now;

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Query(new QueryByTransactionId(
                    transactionId: payment.TransactionId
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/query/transactionid").InnerText, Is.EqualTo(payment.TransactionId + ""));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/customerrefno").InnerText, Is.EqualTo(customerRefNo));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/merchantid").InnerText, Is.EqualTo("1130"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/status").InnerText, Is.EqualTo("SUCCESS"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/amount").InnerText, Is.EqualTo("25000"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/currency").InnerText, Is.EqualTo("SEK"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/vat").InnerText, Is.EqualTo("5000"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/capturedamount").InnerText, Is.EqualTo("25000"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/authorizedamount").InnerText, Is.EqualTo("25000"));

            var created = DateTime.Parse(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/created").InnerText);
            Assert.That(created.Year, Is.EqualTo(now.Year));
            Assert.That(created.Month, Is.EqualTo(now.Month));
            Assert.That(created.Day, Is.EqualTo(now.Day));

            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/creditstatus").InnerText, Is.EqualTo("CREDNONE"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/creditedamount").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/merchantresponsecode").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/paymentmethod").InnerText, Is.EqualTo("DBNORDEASE"));

            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/customer/firstname").InnerXml, Is.EqualTo("TestCompagniet"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/customer/ssn").InnerXml, Is.EqualTo("2345234"));

        }

        [Test]
        public void TestQueryCustomerRefNoDirectPayment()
        {
            var customerRefNo = CreateCustomerRefNo();
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.NORDEASE, customerRefNo));
            var now = DateTime.Now;

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Query(new QueryByCustomerRefNo(
                    customerRefNo: customerRefNo
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/query/customerrefno").InnerText, Is.EqualTo(customerRefNo));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/customerrefno").InnerText, Is.EqualTo(customerRefNo));
        }

        [Test]
        public void TestRecur()
        {
            const string customerRefNo = "Customer reference number or client order number";
            const string subscriptionId = "The subscription id";
            const long amount = 66600L;
            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Recur(new Recur(
                    customerRefNo: customerRefNo,
                    subscriptionId: subscriptionId,
                    currency: Currency.SEK,
                    amount: amount 
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.PrepareRequest();
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/currency").InnerText, Is.EqualTo(Currency.SEK.ToString()));
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/amount").InnerText, Is.EqualTo(amount + ""));
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/customerrefno").InnerText, Is.EqualTo(customerRefNo));
            Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/subscriptionid").InnerText, Is.EqualTo(subscriptionId));

            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();

            //Call to non-existing subscription
            Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("322")); 
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