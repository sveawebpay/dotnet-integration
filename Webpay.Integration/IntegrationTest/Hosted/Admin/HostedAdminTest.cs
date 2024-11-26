using System.Web;
using System.Xml;
using Webpay.Integration.Config;
using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Hosted.Admin.Actions;
using Webpay.Integration.Hosted.Admin.Response;
using Webpay.Integration.Hosted.Payment;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Order.Row.credit;
using Webpay.Integration.Order;

namespace Webpay.Integration.IntegrationTest.Hosted.Admin;

[TestFixture]
public class HostedAdminTest
{
    [Test]
    public async Task TestPreparedPaymentRequest()
    {
        var uri = await PrepareRegularPayment(PaymentMethod.SWISH, CreateCustomerRefNo());

        Assert.That(uri.AbsoluteUri, Does.Match(".*\\/preparedpayment\\/[0-9]+"));
    }

    [Test]
    public void TestGetRecurringPaymentUrl()
    {
        var _ = PrepareRecurPayment(PaymentMethod.KORTCERT, SubscriptionType.RECURRING);
    }

    [Test]
    public void TestGetRecurringCapturePaymentUrl()
    {
        var _ = PrepareRecurPayment(PaymentMethod.KORTCERT, SubscriptionType.RECURRINGCAPTURE);
    }

    [Test, Ignore("")]
    public void TestSemiManualCancelRecurSubscription()
    {
        var _ = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .CancelRecurSubscription(new CancelRecurSubscription(
                subscriptionId: "3352",correlationId:null
            ))
            .DoRequest<RecurResponse>();
    }

    [Test, Ignore("")]
    public void TestSemiManualConfirm()
    {
        var _ = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Confirm(new Confirm(
                transactionId: 598683,
                captureDate: DateTime.Now, correlationId: null
            ))
            .DoRequest<ConfirmResponse>();
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
        var response = Annul.Response(responseXml);

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
        var response = Annul.Response(responseXml);

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
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .CancelRecurSubscription(new CancelRecurSubscription(
                subscriptionId: "12341234", correlationId: null
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
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
        var response = CancelRecurSubscription.Response(responseXml);

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
        var response = CancelRecurSubscription.Response(responseXml);

        Assert.That(response.StatusCode, Is.EqualTo(101));
        Assert.That(response.Accepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Invalid XML."));
    }

    [Test]
    public void TestConfirm()
    {
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Confirm(new Confirm(
                transactionId: 12341234,
                captureDate: new DateTime(2015, 05, 22), correlationId: null
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/confirm/transactionid").InnerText, Is.EqualTo("12341234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/confirm/capturedate").InnerText, Is.EqualTo("2015-05-22"));
    }

    [Test]
    public void TestCreditOrderRows()
    {     
        var delivery = new Delivery { Id = 1234, OrderRows = new List<CreditOrderRowBuilder> { new CreditOrderRowBuilder {  RowId = 1 } } };
        var deliveries = new List<Delivery> { delivery };
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Credit(new Credit(
                transactionId: 12341234,
                amountToCredit: 100, 
                deliveries: deliveries,
                correlationId: null
            ));
      
        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/transactionid").InnerText, Is.EqualTo("12341234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("id").InnerText, Is.EqualTo("1234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("orderrows").FirstChild.FirstChild.InnerText, Is.EqualTo("1"));
    }

    [Test]
    public void TestCreditOrderRowsWithoutDeliveryId()
    {
        var delivery = new Delivery {NewOrderRows = new List<NewCreditOrderRowBuilder> { }, OrderRows = new List<CreditOrderRowBuilder> { new CreditOrderRowBuilder { RowId = 1 } } };
        var deliveries = new List<Delivery> { delivery };
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Credit(new Credit(
                transactionId: 12341234,
                amountToCredit: 100,
                deliveries: deliveries,
                correlationId: null
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/transactionid").InnerText, Is.EqualTo("12341234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("id").InnerText, Is.EqualTo(""));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("orderrows").FirstChild.FirstChild.InnerText, Is.EqualTo("1"));
    }

    [Test]
    public void TestCreditNewOrderRows()
    {
        var delivery = new Delivery { 
            Id = 1234, 
            NewOrderRows = new List<NewCreditOrderRowBuilder> { new NewCreditOrderRowBuilder {
                        Quantity = 1,
                        DiscountAmount = 10,
                        DiscountPercent=2 ,
                        Name = "test",
                        VatPercent=1,
                        UnitPrice = 1
                    }
                }, 
            OrderRows = new List<CreditOrderRowBuilder> { new CreditOrderRowBuilder { Quantity = 1, RowId = 1 } } 
        };
        var deliveries = new List<Delivery> { delivery };
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Credit(new Credit(
                transactionId: 12341234,
                amountToCredit: 100,
                deliveries: deliveries,
                correlationId: null
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/transactionid").InnerText, Is.EqualTo("12341234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("id").InnerText, Is.EqualTo("1234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("orderrows").ChildNodes[1].SelectSingleNode("quantity").InnerText, Is.EqualTo("1"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("orderrows").ChildNodes[1].SelectSingleNode("name").InnerText, Is.EqualTo("test"));
    }

    [Test]
    public void TestCreditNewOrderRowsWithZeroUnitPrice()
    {
        var delivery = new Delivery
        {
            Id = 1234,
            NewOrderRows = new List<NewCreditOrderRowBuilder> { new NewCreditOrderRowBuilder {
                        Quantity = 1,
                        DiscountAmount = 10,
                        DiscountPercent=2 ,
                        Name = "test",
                        VatPercent=1,
                        UnitPrice = 0
                    },
                    new NewCreditOrderRowBuilder {
                        Quantity = 1,
                        DiscountAmount = 10,
                        DiscountPercent=2 ,
                        Name = "test",
                        VatPercent=1,
                        UnitPrice = 1
                    }
                },
            OrderRows = new List<CreditOrderRowBuilder> { }
        };
        var deliveries = new List<Delivery> { delivery };
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Credit(new Credit(
                transactionId: 12341234,
                amountToCredit: 100,
                deliveries: deliveries,
                correlationId: null
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/transactionid").InnerText, Is.EqualTo("12341234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("id").InnerText, Is.EqualTo("1234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("orderrows").ChildNodes[1].SelectSingleNode("quantity").InnerText, Is.EqualTo("1"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("orderrows").ChildNodes[1].SelectSingleNode("name").InnerText, Is.EqualTo("test"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/credit/deliveries").FirstChild.SelectSingleNode("orderrows").ChildNodes[0].SelectSingleNode("unitprice").InnerText, Is.EqualTo("0"));
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
        var response = Confirm.Response(responseXml);

        Assert.That(response.TransactionId, Is.EqualTo(598972));
        Assert.That(response.CustomerRefNo, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
        Assert.That(response.ClientOrderNumber, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
        Assert.That(response.StatusCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
        Assert.That(response.ErrorMessage, Is.Empty);
    }

    [Test]
    public void TestConfirmPartial()
    {
        var orderRows = new List<NumberedOrderRowBuilder>
        {
            new NumberedOrderRowBuilder() { }.SetRowNumber(1).SetQuantity(1),
            new NumberedOrderRowBuilder() { }.SetRowNumber(2).SetQuantity(1),
        };
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .ConfirmPartial(new ConfirmPartial(
                transactionId: 12341234,
                callerReferenceId: new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"),
                amount: 1000,
                orderRows: orderRows, correlationId: new Guid()
            ));
       
        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/confirmPartial/transactionid").InnerText, Is.EqualTo("12341234"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/confirmPartial/captureRequestId").InnerText, Is.EqualTo("9d2b0228-4d0d-4c23-8b49-01a698857709"));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/confirmPartial/orderrows").FirstChild.SelectSingleNode("rowId").InnerText, Is.EqualTo("1"));
    }

    [Test]
    public void TestConfirmPartialResponse()
    {
        var responseXml = new XmlDocument();
        responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                        <response>
                            <transaction id=""598972"">
                                <customerrefno>1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4</customerrefno>
                            </transaction>
                            <statuscode>0</statuscode>
                        </response>");
        var response = ConfirmPartial.Response(responseXml);

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
        var response = Confirm.Response(responseXml);

        Assert.That(response.TransactionId, Is.Null);
        Assert.That(response.CustomerRefNo, Is.Null);
        Assert.That(response.ClientOrderNumber, Is.Null);
        Assert.That(response.StatusCode, Is.EqualTo(107));
        Assert.That(response.Accepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
    }

    [Test]
    public async Task TestGetPaymentMethods()
    {
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .GetPaymentMethods(new GetPaymentMethods(
                merchantId: 1130, correlationId: null
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/getpaymentmethods/merchantid").InnerText, Is.EqualTo("1130"));

        var hostedAdminResponse = await hostedActionRequest.DoRequest<HostedAdminResponse>();
        Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));

        var actualPaymentmethodsXml = hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/paymentmethods").InnerXml;
        var expectedPaymentmethodsXml =
            "<paymentmethod>MOBILEPAY</paymentmethod><paymentmethod>SVEACARDPAY</paymentmethod><paymentmethod>SVEACARDPAY_PF</paymentmethod><paymentmethod>SWISH</paymentmethod><paymentmethod>SWISH_PF</paymentmethod><paymentmethod>TRUSTLY</paymentmethod><paymentmethod>VIPPS_DIRECT</paymentmethod>";

        Assert.That(actualPaymentmethodsXml, Is.EqualTo(expectedPaymentmethodsXml));
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
        var response = GetPaymentMethods.Response(responseXml);

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
        var response = GetPaymentMethods.Response(responseXml);

        Assert.That(response.StatusCode, Is.EqualTo(107));
        Assert.That(response.Accepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
        Assert.That(response.PaymentMethods.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task TestGetReconciliationReport()
    {
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .GetReconciliationReport(new GetReconciliationReport(
                date: new DateTime(2023, 04, 27), correlationId: null
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/getreconciliationreport/date").InnerText, Is.EqualTo("2023-04-27"));

        var hostedAdminResponse = await hostedActionRequest.DoRequest<HostedAdminResponse>();
        Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));

        var reconciliationXml = hostedAdminResponse.MessageXmlDocument
            .SelectSingleNode("/response/reconciliation/reconciliationtransaction").InnerXml;

        var matchPattern = @"<transactionid>\d+</transactionid><customerrefno>.*</customerrefno><paymentmethod>SVEACARDPAY</paymentmethod><amount>500</amount><currency>SEK</currency><time>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2} CEST</time>";
        Assert.That(reconciliationXml, Does.Match(matchPattern));
        Assert.That(hostedAdminResponse.To(GetReconciliationReport.Response).ReconciliationTransactions[0].Amount, Is.EqualTo(5m));
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
        var response = GetReconciliationReport.Response(responseXml);

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
        var response = GetReconciliationReport.Response(responseXml);

        Assert.That(response.StatusCode, Is.EqualTo(107));
        Assert.That(response.Accepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
        Assert.That(response.ReconciliationTransactions.Count, Is.EqualTo(0));
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
        var response = LowerAmount.Response(responseXml);

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
        var response = LowerAmount.Response(responseXml);

        Assert.That(response.TransactionId, Is.Null);
        Assert.That(response.CustomerRefNo, Is.Null);
        Assert.That(response.ClientOrderNumber, Is.Null);
        Assert.That(response.StatusCode, Is.EqualTo(107));
        Assert.That(response.Accepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
    }

    [Test]
    public void TestLowerAmountConfirmResponse()
    {
        var responseXml = new XmlDocument();
        responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                        <response>
                            <transaction id=""598972"">
                                <customerrefno>1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4</customerrefno>
                            </transaction>
                            <statuscode>0</statuscode>
                        </response>");
        var response = LowerAmountConfirm.Response(responseXml);

        Assert.That(response.TransactionId, Is.EqualTo(598972));
        Assert.That(response.CustomerRefNo, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
        Assert.That(response.ClientOrderNumber, Is.EqualTo("1ba66a0d653ca4cf3a5bc3eeb9ed1a2b4"));
        Assert.That(response.StatusCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
        Assert.That(response.ErrorMessage, Is.Empty);
    }

    [Test]
    public void TestLowerAmountConfirmResponseFailure()
    {
        var responseXml = new XmlDocument();
        responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                        <response>
                            <statuscode>107</statuscode>
                        </response>");
        var response = LowerAmountConfirm.Response(responseXml);

        Assert.That(response.TransactionId, Is.Null);
        Assert.That(response.CustomerRefNo, Is.Null);
        Assert.That(response.ClientOrderNumber, Is.Null);
        Assert.That(response.StatusCode, Is.EqualTo(107));
        Assert.That(response.Accepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
    }

    [Test]
    public async Task TestQueryTransactionIdDirectPayment()
    {
        var customerRefNo = CreateCustomerRefNo();
        var preparedPaymentUri = await PrepareRegularPayment(PaymentMethod.SWISH, customerRefNo);
        var payment = await MakePreparedPayment(preparedPaymentUri);

        var now = DateTime.Now;

        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Query(new QueryByTransactionId(
                transactionId: payment.TransactionId, 
                correlationId: new Guid()
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/query/transactionid").InnerText, Is.EqualTo(payment.TransactionId + ""));

        var hostedAdminResponse = await hostedActionRequest.DoRequest<HostedAdminResponse>();
        Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
        Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/customerrefno").InnerText, Is.EqualTo(customerRefNo));
        Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/merchantid").InnerText, Is.EqualTo("1110"));
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
        Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/paymentmethod").InnerText, Is.EqualTo("SWISH"));
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
                                <chname>ch_name</chname>
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

        var response = Query.Response(responseXml);

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
        Assert.That(response.Transaction.NumberedOrderRows[0].GetName(), Is.EqualTo("Prod"));
        Assert.That(response.Transaction.NumberedOrderRows[0].GetAmountExVat(), Is.EqualTo(100.00M));
        Assert.That(response.Transaction.NumberedOrderRows[0].GetAmountIncVat(), Is.EqualTo(125.00M));
        Assert.That(response.Transaction.NumberedOrderRows[0].GetVatPercent(), Is.EqualTo(25.00M));
        Assert.That(response.Transaction.NumberedOrderRows[0].GetDescription(), Is.EqualTo("Specification"));
        Assert.That(response.Transaction.NumberedOrderRows[0].GetQuantity(), Is.EqualTo(2));
        Assert.That(response.Transaction.NumberedOrderRows[0].GetArticleNumber(), Is.EqualTo("1"));
        Assert.That(response.Transaction.NumberedOrderRows[0].GetUnit(), Is.EqualTo("st"));
    }

    [Test]
    public void TestQueryResponseFailure()
    {
        var responseXml = new XmlDocument();
        responseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                        <response>
                            <statuscode>107</statuscode>
                        </response>");
        var response = Query.Response(responseXml);

        Assert.That(response.TransactionId, Is.Null);
        Assert.That(response.CustomerRefNo, Is.Null);
        Assert.That(response.ClientOrderNumber, Is.Null);
        Assert.That(response.StatusCode, Is.EqualTo(107));
        Assert.That(response.Accepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
        Assert.That(response.Transaction, Is.Null);
    }

    [Test]
    public async Task TestQueryCustomerRefNoDirectPayment()
    {
        var customerRefNo = CreateCustomerRefNo();
        var preparedPaymentUri = await PrepareRegularPayment(PaymentMethod.SWISH, customerRefNo);
        var _ = await MakePreparedPayment(preparedPaymentUri);

        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Query(new QueryByCustomerRefNo(
                customerRefNo: customerRefNo, correlationId: new Guid()
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/query/customerrefno").InnerText, Is.EqualTo(customerRefNo));

        var hostedAdminResponse = await hostedActionRequest.DoRequest<HostedAdminResponse>();
        Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/statuscode").InnerText, Is.EqualTo("0"));
        Assert.That(hostedAdminResponse.MessageXmlDocument.SelectSingleNode("/response/transaction/customerrefno").InnerText, Is.EqualTo(customerRefNo));
    }

    [Test]
    public async Task TestRecur()
    {
        const string customerRefNo = "Customer reference number or client order number";
        const string subscriptionId = "The subscription id";
        const long amount = 66600L;
        const long vat = 2500;
        var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
            .Recur(new Recur(
                customerRefNo: customerRefNo,
                subscriptionId: subscriptionId,
                currency: Currency.SEK,
                amount: amount,
                vat: vat, correlationId: new Guid()
            ));

        var hostedAdminRequest = hostedActionRequest.PrepareRequest();

        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/currency").InnerText, Is.EqualTo(Currency.SEK.ToString()));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/amount").InnerText, Is.EqualTo(amount + ""));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/customerrefno").InnerText, Is.EqualTo(customerRefNo));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/subscriptionid").InnerText, Is.EqualTo(subscriptionId));
        Assert.That(hostedAdminRequest.MessageXmlDocument.SelectSingleNode("/recur/vat").InnerText, Is.EqualTo(vat + ""));

        var hostedAdminResponse = await hostedActionRequest.DoRequest<HostedAdminResponse>();

        // Call to non-existing subscription
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
        var response = Recur.Response(responseXml);

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
        var response = Recur.Response(responseXml);

        Assert.That(response.TransactionId, Is.Null);
        Assert.That(response.CustomerRefNo, Is.Null);
        Assert.That(response.ClientOrderNumber, Is.Null);
        Assert.That(response.StatusCode, Is.EqualTo(107));
        Assert.That(response.Accepted, Is.False);
        Assert.That(response.ErrorMessage, Is.EqualTo("Transaction rejected by bank."));
    }

    [Test]
    public void TestHostedAdminResponseBadMerchant()
    {
        var ex = Assert.Throws<System.Exception>(() => new HostedAdminResponse(
                @"<?xml version='1.0' encoding='UTF-8'?><response><message>PD94bWwgdmVyc2lvbj0nMS4wJyBlbmNvZGluZz0nVVRGLTgnPz48cmVzcG9uc2U+PHN0YXR1c2NvZGU+MDwvc3RhdHVzY29kZT48cHJlcGFyZWRwYXltZW50PjxpZD4yNjYwOTwvaWQ+PGNyZWF0ZWQ+MjAxNS0wNC0yMyAxNzo0MjoxMSBDRVNUPC9jcmVhdGVkPjwvcHJlcGFyZWRwYXltZW50PjwvcmVzcG9uc2U+</message><merchantid>1130</merchantid><mac>db277097f6c30f08ae1cc463d755bce42b89564ee6bec8ba0fdf9ec25036bd68d9f92b9db4ff348ced0b2a530e3ecee913941301236098477fc1b87843ba44fc</mac></response>",
                "8a9cece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3",
                "1131"
            ));
        Assert.That(ex.Message, Is.EqualTo("The merchantId in the response from the server is not the expected. This could mean that someone has tamepered with the message. Expected:1131 Actual:1130"));
    }

    [Test]
    public void TestHostedAdminResponseBadMacInMessage()
    {
        var ex = Assert.Throws<System.Exception>(() => new HostedAdminResponse(
                @"<?xml version='1.0' encoding='UTF-8'?><response><message>PD94bWwgdmVyc2lvbj0nMS4wJyBlbmNvZGluZz0nVVRGLTgnPz48cmVzcG9uc2U+PHN0YXR1c2NvZGU+MDwvc3RhdHVzY29kZT48cHJlcGFyZWRwYXltZW50PjxpZD4yNjYwOTwvaWQ+PGNyZWF0ZWQ+MjAxNS0wNC0yMyAxNzo0MjoxMSBDRVNUPC9jcmVhdGVkPjwvcHJlcGFyZWRwYXltZW50PjwvcmVzcG9uc2U+</message><merchantid>1130</merchantid><mac>aba77097f6c30f08ae1cc463d755bce42b89564ee6bec8ba0fdf9ec25036bd68d9f92b9db4ff348ced0b2a530e3ecee913941301236098477fc1b87843ba44fc</mac></response>",
                "8a9cece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3",
                "1130"
            ));
        Assert.That(ex.Message,
            Is.EqualTo(
                "SEVERE: The mac from the server does not match the expected mac. The message might have been tampered with, or the secret word used is not correct. Merchant:1130 Message:\nPD94bWwgdmVyc2lvbj0nMS4wJyBlbmNvZGluZz0nVVRGLTgnPz48cmVzcG9uc2U+PHN0YXR1c2NvZGU+MDwvc3RhdHVzY29kZT48cHJlcGFyZWRwYXltZW50PjxpZD4yNjYwOTwvaWQ+PGNyZWF0ZWQ+MjAxNS0wNC0yMyAxNzo0MjoxMSBDRVNUPC9jcmVhdGVkPjwvcHJlcGFyZWRwYXltZW50PjwvcmVzcG9uc2U+"));
    }

    [Test]
    public void TestHostedAdminResponseCorrectMacInMessageButWrongSecretWord()
    {
        var ex = Assert.Throws<System.Exception>(() => new HostedAdminResponse(
                @"<?xml version='1.0' encoding='UTF-8'?><response><message>PD94bWwgdmVyc2lvbj0nMS4wJyBlbmNvZGluZz0nVVRGLTgnPz48cmVzcG9uc2U+PHN0YXR1c2NvZGU+MDwvc3RhdHVzY29kZT48cHJlcGFyZWRwYXltZW50PjxpZD4yNjYwOTwvaWQ+PGNyZWF0ZWQ+MjAxNS0wNC0yMyAxNzo0MjoxMSBDRVNUPC9jcmVhdGVkPjwvcHJlcGFyZWRwYXltZW50PjwvcmVzcG9uc2U+</message><merchantid>1130</merchantid><mac>db277097f6c30f08ae1cc463d755bce42b89564ee6bec8ba0fdf9ec25036bd68d9f92b9db4ff348ced0b2a530e3ecee913941301236098477fc1b87843ba44fc</mac></response>",
                "abacece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3",
                "1130"
            ));
        Assert.That(ex.Message,
            Is.EqualTo("SEVERE: The mac from the server does not match the expected mac. The message might have been tampered with, or the secret word used is not correct. Merchant:1130 Message:\nPD94bWwgdmVyc2lvbj0nMS4wJyBlbmNvZGluZz0nVVRGLTgnPz48cmVzcG9uc2U+PHN0YXR1c2NvZGU+MDwvc3RhdHVzY29kZT48cHJlcGFyZWRwYXltZW50PjxpZD4yNjYwOTwvaWQ+PGNyZWF0ZWQ+MjAxNS0wNC0yMyAxNzo0MjoxMSBDRVNUPC9jcmVhdGVkPjwvcHJlcGFyZWRwYXltZW50PjwvcmVzcG9uc2U+"));
    }

    [Test, Ignore("Create a directbank transaction with Trustly to test this")]
    public void TestHostedAdminResponseStatus150()
    {
        var request = WebpayAdmin.CreditPayment(new SveaTestConfigurationProvider())
            .SetTransactionId(697022)
            .SetAmountIncVat(100);
        var response = request.CreditDirectBankPayment().DoRequest();

        Assert.That(response.StatusCode, Is.EqualTo(150));
        Assert.That(response.Accepted, Is.EqualTo(true));        
    }

    internal static Task<Uri> PrepareRegularPayment(PaymentMethod paymentMethod, string createCustomerRefNo)
    {           
        return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
            .SetPayerAlias(TestingTool.DefaultTestPayerAlias)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(createCustomerRefNo)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePaymentMethod(paymentMethod)
            .___SetSimulatorCode_ForTestingOnly("0")
            .SetReturnUrl(
                "https://webpaypaymentgatewaystage.svea.com/webpay/public/static/testlandingpage.html")
            .PreparePayment("127.0.0.1");
    }

    internal static Task<Uri> PrepareRegularPaymentWithTwoRowsSpecifiedExVatAndVatPercent(PaymentMethod paymentMethod, string createCustomerRefNo)
    {
        return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(createCustomerRefNo)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePaymentMethod(paymentMethod)
            .___SetSimulatorCode_ForTestingOnly("0")
            .SetReturnUrl(
                "https://webpaypaymentgatewaystage.svea.com/webpay/public/static/testlandingpage.html")
            .PreparePayment("127.0.0.1");
    }

    internal static Task<Uri> PrepareRegularPaymentWithTwoRowsSpecifiedIncVatAndVatPercent(PaymentMethod paymentMethod, string createCustomerRefNo)
    {
        return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(createCustomerRefNo)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePaymentMethod(paymentMethod)
            .___SetSimulatorCode_ForTestingOnly("0")
            .SetReturnUrl(
                "https://webpaypaymentgatewaystage.svea.com/webpay/public/static/testlandingpage.html")
            .PreparePayment("127.0.0.1");
    }

    internal static Task<Uri> PrepareRegularPaymentWithTwoRowsSpecifiedIncVatAndExVat(PaymentMethod paymentMethod, string createCustomerRefNo)
    {
        return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(createCustomerRefNo)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePaymentMethod(paymentMethod)
            .___SetSimulatorCode_ForTestingOnly("0")
            .SetReturnUrl(
                "https://webpaypaymentgatewaystage.svea.com/webpay/public/static/testlandingpage.html")
            .PreparePayment("127.0.0.1");
    }

    internal static string CreateCustomerRefNo()
    {
        return "1" + Guid.NewGuid().ToString().Replace("-", "");
    }

    private static Task<Uri> PrepareRecurPayment(PaymentMethod paymentMethod, SubscriptionType subscriptionType)
    {
        return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber("1" + Guid.NewGuid().ToString().Replace("-", ""))
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UsePaymentMethod(paymentMethod)
            .SetSubscriptionType(subscriptionType)
            .___SetSimulatorCode_ForTestingOnly("0")
            .SetReturnUrl(
                "https://webpaypaymentgatewaystage.svea.com/webpay/public/static/testlandingpage.html")
            .PreparePayment("127.0.0.1");
    }

    internal static async Task<PaymentResponse> MakePreparedPayment(Uri preparePayment)
    {
        using (var handler = new HttpClientHandler { AllowAutoRedirect = false })
        using (var httpClient = new HttpClient(handler))
        {
            var response = await httpClient.GetAsync(preparePayment, HttpCompletionOption.ResponseHeadersRead);

            if (response.Headers.Location == null)
            {
                throw new InvalidOperationException("Response does not contain a 'Location' header.");
            }

            var location = response.Headers.Location;
            var nameValueCollection = HttpUtility.ParseQueryString(location.Query);

            var messageBase64 = nameValueCollection["response"];
            var merchantId = nameValueCollection["merchantid"];
            var mac = nameValueCollection["mac"];

            return new PaymentResponse(messageBase64, mac, merchantId);
        }
    }
}