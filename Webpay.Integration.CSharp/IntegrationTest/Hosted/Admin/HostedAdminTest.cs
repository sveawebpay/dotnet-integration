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
        public void TestLowerAmountCompleteFlow()
        {
            var customerRefNo = CreateCustomerRefNo();
            var payment = MakePreparedPayment(PrepareRegularPayment(PaymentMethod.KORTCERT, customerRefNo));

            LowerAmountResponse lowerAmountResponse = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .LowerAmount(new LowerAmount(
                    transactionId: payment.TransactionId,
                    amountToLower: 666
                    ))
                .DoRequest()
                .To(LowerAmount.Response);
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
        public void TestQueryResponse()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <transaction id=""599086"">
                        <customerrefno>19aa8f62d9cb44eb6851ff3650873b2ac</customerrefno>
                        <merchantid>1130</merchantid>
                        <status>SUCCESS</status>
                        <amount>25000</amount>
                        <currency>SEK</currency>
                        <vat>5000</vat>
                        <capturedamount>25000</capturedamount>
                        <authorizedamount>25000</authorizedamount>
                        <created>2015-04-23 10:51:06.977</created>
                        <creditstatus>CREDNONE</creditstatus>
                        <creditedamount>0</creditedamount>
                        <merchantresponsecode>0</merchantresponsecode>
                        <paymentmethod>DBNORDEASE</paymentmethod>
                        <callbackurl>https://hej.hopp</callbackurl>
                        <subscriptionid>subid</subscriptionid>
                        <subscriptiontype>subtype</subscriptiontype>
                        <capturedate>2015-04-23 10:51:06.977</capturedate>
                        <eci>eci</eci>
                        <mdstatus>mdstatus</mdstatus>
                        <expiryyear>2015</expiryyear>
                        <expirymonth>09</expirymonth>
                        <ch_name>ch_name</ch_name>
                        <authcode>authcode</authcode>
                        <customer id=""22513"">
                            <firstname>Testa</firstname>
                            <lastname>Testson</lastname>
                            <initials>TT</initials>
                            <fullname>Testa T Testson</fullname>
                            <email>testson@sveaekonomi.se</email>
                            <ssn>460509-2222</ssn>
                            <address>Testgatan 666</address>
                            <address2>c/o Quality Man</address2>
                            <city>Testholm</city>
                            <country>SE</country>
                            <zip>123 45</zip>
                            <phone>012345566789</phone>
                            <vatnumber>123456-7890</vatnumber>
                            <housenumber>69</housenumber>
                            <companyname>TestCompagniet</companyname>
                        </customer>
                        <orderrows>
                            <row>
                                <id>72750</id>
                                <name>Prod</name>
                                <amount>12500</amount>
                                <vat>2500</vat>
                                <description>Specification</description>
                                <quantity>2.0</quantity>
                                <sku>1</sku>
                                <unit>st</unit>
                            </row>
                        </orderrows>
                    </transaction>
                    <statuscode>0</statuscode>
                </response>");
            QueryResponse response = Query.Response(responseXml);

            Assert.That(response.TransactionId, Is.EqualTo(599086));
            Assert.That(response.CustomerRefNo, Is.EqualTo("19aa8f62d9cb44eb6851ff3650873b2ac"));
            Assert.That(response.ClientOrderNumber, Is.EqualTo("19aa8f62d9cb44eb6851ff3650873b2ac"));
            Assert.That(response.Transaction.CustomerRefNo, Is.EqualTo("19aa8f62d9cb44eb6851ff3650873b2ac"));
            Assert.That(response.Transaction.ClientOrderNumber, Is.EqualTo("19aa8f62d9cb44eb6851ff3650873b2ac"));
            Assert.That(response.Transaction.MerchantId, Is.EqualTo(1130));
            Assert.That(response.Transaction.Status, Is.EqualTo("SUCCESS"));
            Assert.That(response.Transaction.Amount, Is.EqualTo(250.00M));
            Assert.That(response.Transaction.Currency, Is.EqualTo("SEK"));
            Assert.That(response.Transaction.Vat, Is.EqualTo(50.00M));
            Assert.That(response.Transaction.CapturedAmount, Is.EqualTo(250.00M));
            Assert.That(response.Transaction.AuthorizedAmount, Is.EqualTo(250.00M));
            Assert.That(response.Transaction.Created, Is.EqualTo(new DateTime(2015, 04, 23, 10, 51, 06, 977)));
            Assert.That(response.Transaction.CreditStatus, Is.EqualTo("CREDNONE"));
            Assert.That(response.Transaction.CreditedAmount, Is.EqualTo(0.00M));
            Assert.That(response.Transaction.MerchantResponseCode, Is.EqualTo(0));
            Assert.That(response.Transaction.PaymentMethod, Is.EqualTo("DBNORDEASE"));
            Assert.That(response.Transaction.CallbackUrl, Is.EqualTo("https://hej.hopp"));
            Assert.That(response.Transaction.SubscriptionId, Is.EqualTo("subid"));
            Assert.That(response.Transaction.SubscriptionType, Is.EqualTo("subtype"));
            Assert.That(response.Transaction.Eci, Is.EqualTo("eci"));
            Assert.That(response.Transaction.MdStatus, Is.EqualTo("mdstatus"));
            Assert.That(response.Transaction.ExpiryYear, Is.EqualTo("2015"));
            Assert.That(response.Transaction.ExpiryMonth, Is.EqualTo("09"));
            Assert.That(response.Transaction.ChName, Is.EqualTo("ch_name"));
            Assert.That(response.Transaction.AuthCode, Is.EqualTo("authcode"));
            Assert.That(response.Transaction.Customer.Id, Is.EqualTo("22513"));
            Assert.That(response.Transaction.Customer.FirstName, Is.EqualTo("Testa"));
            Assert.That(response.Transaction.Customer.LastName, Is.EqualTo("Testson"));
            Assert.That(response.Transaction.Customer.Initials, Is.EqualTo("TT"));
            Assert.That(response.Transaction.Customer.FullName, Is.EqualTo("Testa T Testson"));
            Assert.That(response.Transaction.Customer.Email, Is.EqualTo("testson@sveaekonomi.se"));
            Assert.That(response.Transaction.Customer.Ssn, Is.EqualTo("460509-2222"));
            Assert.That(response.Transaction.Customer.Address, Is.EqualTo("Testgatan 666"));
            Assert.That(response.Transaction.Customer.Address2, Is.EqualTo("c/o Quality Man"));
            Assert.That(response.Transaction.Customer.City, Is.EqualTo("Testholm"));
            Assert.That(response.Transaction.Customer.Country, Is.EqualTo("SE"));
            Assert.That(response.Transaction.Customer.Zip, Is.EqualTo("123 45"));
            Assert.That(response.Transaction.Customer.Phone, Is.EqualTo("012345566789"));
            Assert.That(response.Transaction.Customer.VatNumber, Is.EqualTo("123456-7890"));
            Assert.That(response.Transaction.Customer.HouseNumber, Is.EqualTo("69"));
            Assert.That(response.Transaction.Customer.CompanyName, Is.EqualTo("TestCompagniet"));
            Assert.That(response.Transaction.OrderRows[0].Id, Is.EqualTo("72750"));
            Assert.That(response.Transaction.OrderRows[0].Name, Is.EqualTo("Prod"));
            Assert.That(response.Transaction.OrderRows[0].Amount, Is.EqualTo(125.00M));
            Assert.That(response.Transaction.OrderRows[0].Vat, Is.EqualTo(25.00M));
            Assert.That(response.Transaction.OrderRows[0].Description, Is.EqualTo("Specification"));
            Assert.That(response.Transaction.OrderRows[0].Quantity, Is.EqualTo(2));
            Assert.That(response.Transaction.OrderRows[0].Sku, Is.EqualTo("1"));
            Assert.That(response.Transaction.OrderRows[0].Unit, Is.EqualTo("st"));

            Assert.That(response.StatusCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.ErrorMessage, Is.Empty);
        }

        [Test]
        public void TestQueryResponseFailure()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>107</statuscode>
                </response>");
            QueryResponse response = Query.Response(responseXml);

            Assert.That(response.TransactionId, Is.Null);
            Assert.That(response.CustomerRefNo, Is.Null);
            Assert.That(response.ClientOrderNumber, Is.Null);
            Assert.That(response.StatusCode, Is.EqualTo(107));
            Assert.That(response.Accepted, Is.False);
            Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
            Assert.That(response.Transaction, Is.Null);
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


        [Test]
        public void TestRecurResponse()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <transaction id=""598972"">
                        <customerrefno>1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4</customerrefno>
                        <paymentmethod>KORTCERT</paymentmethod>
                        <merchantid>1130</merchantid>
                        <amount>66600</amount>
                        <currency>SEK</currency>
                        <cardtype>VISA</cardtype>
                        <maskedcardno>12345***********1234</maskedcardno>
                        <expirymonth>07</expirymonth>
                        <expiryyear>2099</expiryyear>
                        <authcode>authcode</authcode>
                        <subscriptionid>subscriptionid</subscriptionid>
                    </transaction>
                    <statuscode>0</statuscode>
                </response>");
            RecurResponse response = Recur.Response(responseXml);

            Assert.That(response.TransactionId, Is.EqualTo(598972));
            Assert.That(response.CustomerRefNo, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
            Assert.That(response.ClientOrderNumber, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
            Assert.That(response.PaymentMethod, Is.EqualTo("KORTCERT"));
            Assert.That(response.MerchantId, Is.EqualTo(1130));
            Assert.That(response.Amount, Is.EqualTo(666.00M));
            Assert.That(response.Currency, Is.EqualTo("SEK"));
            Assert.That(response.CardType, Is.EqualTo("VISA"));
            Assert.That(response.MaskedCardNo, Is.EqualTo("12345***********1234"));
            Assert.That(response.ExpiryMonth, Is.EqualTo("07"));
            Assert.That(response.ExpiryYear, Is.EqualTo("2099"));
            Assert.That(response.AuthCode, Is.EqualTo("authcode"));
            Assert.That(response.SubscriptionId, Is.EqualTo("subscriptionid"));
            Assert.That(response.StatusCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
            Assert.That(response.ErrorMessage, Is.Empty);
        }


        [Test]
        public void TestRecurResponseFailure()
        {
            var responseXml = new XmlDocument();
            responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                <response>
                    <statuscode>107</statuscode>
                </response>");
            RecurResponse response = Recur.Response(responseXml);

            Assert.That(response.TransactionId, Is.Null);
            Assert.That(response.CustomerRefNo, Is.Null);
            Assert.That(response.ClientOrderNumber, Is.Null);
            Assert.That(response.StatusCode, Is.EqualTo(107));
            Assert.That(response.Accepted, Is.False);
            Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
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