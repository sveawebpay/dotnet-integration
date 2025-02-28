using Webpay.Integration.Config;
using Webpay.Integration.Hosted.Admin.Actions;
using Webpay.Integration.Util;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Hosted.Admin;

public class HostedAdmin
{
    public readonly IConfigurationProvider ConfigurationProvider;
    public readonly CountryCode CountryCode;
    public readonly string MerchantId;
    public readonly List<AdminRequestHeader> Headers;

    public HostedAdmin(IConfigurationProvider configurationProvider, CountryCode countryCode)
    {
        ConfigurationProvider = configurationProvider;
        MerchantId = configurationProvider.GetMerchantId(PaymentType.HOSTED, countryCode);
        CountryCode = countryCode;
        Headers = new List<AdminRequestHeader>();
    }

    public HostedActionRequest Annul(Annul annul)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <annul>
                <transactionid>{0}</transactionid>
                </annul>", annul.TransactionId);
        AddCorrelationIdHeader(annul.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/annul");
    }

    public HostedActionRequest CancelRecurSubscription(CancelRecurSubscription cancelRecurSubscription)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <cancelrecursubscription>
                <subscriptionid>{0}</subscriptionid>
                </cancelrecursubscription>", cancelRecurSubscription.SubscriptionId);
        AddCorrelationIdHeader(cancelRecurSubscription.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
            "/cancelrecursubscription");
    }

    public HostedActionRequest Confirm(Confirm confirm)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <confirm>
                <transactionid>{0}</transactionid>
                <capturedate>{1}</capturedate>
                </confirm>", confirm.TransactionId, confirm.CaptureDate.ToString("yyyy-MM-dd"));
        AddCorrelationIdHeader(confirm.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/confirm");
    }

    public HostedActionRequest ConfirmPartial(ConfirmPartial confirmPartial)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <confirmPartial>
                <captureRequestId>{0}</captureRequestId>
                <transactionid>{1}</transactionid>
                <amount>{2}</amount>
                <orderrows>{3}
                </orderrows>
                </confirmPartial>", confirmPartial.CallerReferenceId.ToString(), confirmPartial.TransactionId, confirmPartial.Amount, confirmPartial.GetXmlForOrderRows());
        AddCorrelationIdHeader(confirmPartial.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/confirmpartial");
    }

    public HostedActionRequest Credit(Credit credit)
    {
        var creditByAmount = $"<amounttocredit>{credit.AmountToCredit}</amounttocredit>";
        if (credit.Deliveries.Count>0)
        {
            creditByAmount = "";
        }
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <credit>
                <transactionid>{0}</transactionid>
                {1}
                {2}
                </credit>", credit.TransactionId, creditByAmount, credit.GetXmlForDeliveries());
        AddCorrelationIdHeader(credit.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/credit");
    }

    public HostedActionRequest GetPaymentMethods(GetPaymentMethods getPaymentMethods)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <getpaymentmethods>
                <merchantid>{0}</merchantid>
                </getpaymentmethods>", getPaymentMethods.MerchantId);
        AddCorrelationIdHeader(getPaymentMethods.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
            "/getpaymentmethods");
    }

    public HostedActionRequest GetReconciliationReport(GetReconciliationReport getReconciliationReport)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <getreconciliationreport>
                <date>{0}</date>
                </getreconciliationreport>", getReconciliationReport.Date.ToString("yyyy-MM-dd"));
        AddCorrelationIdHeader(getReconciliationReport.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
            "/getreconciliationreport");
    }

    public HostedActionRequest LowerAmount(LowerAmount lowerAmount)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <loweramount>
                <transactionid>{0}</transactionid>
                <amounttolower>{1}</amounttolower>
                </loweramount>", lowerAmount.TransactionId, lowerAmount.AmountToLower);
        AddCorrelationIdHeader(lowerAmount.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/loweramount");
    }

    public HostedActionRequest LowerOrderRow(LowerOrderRow lowerOrderRow)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <lowerorderrow>
                <transactionid>{0}</transactionid>
                <orderrows>{1}
                </orderrows>
                </lowerorderrow>", lowerOrderRow.TransactionId, lowerOrderRow.GetXmlForOrderRows());
        AddCorrelationIdHeader(lowerOrderRow.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/lowerorderrow");
    }

    public HostedActionRequest LowerAmountConfirm(LowerAmountConfirm lowerAmount)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <loweramountconfirm>
                <transactionid>{0}</transactionid>
                <amounttolower>{1}</amounttolower>
                <capturedate>{2}</capturedate>
                </loweramountconfirm>", lowerAmount.TransactionId, 
                                    lowerAmount.AmountToLower,
                                    lowerAmount.CaptureDate.ToString("yyyy-MM-dd"));
        AddCorrelationIdHeader(lowerAmount.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/loweramountconfirm");
    }

    public HostedActionRequest Query(QueryByTransactionId query)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <query>
                <transactionid>{0}</transactionid>
                </query>", query.TransactionId);
        AddCorrelationIdHeader(query.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
            "/querytransactionid");
    }

    public HostedActionRequest Query(QueryByCustomerRefNo query)
    {
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <query>
                <customerrefno>{0}</customerrefno>
                </query>", query.CustomerRefNo);
        AddCorrelationIdHeader(query.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers,
            "/querycustomerrefno");
    }

    public HostedActionRequest Recur(Recur recur)
    {
        var vat = recur.Vat != 0 ? "<vat>" + recur.Vat + "</vat>" : "";
        var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <recur>
                <customerrefno>{0}</customerrefno>
                <subscriptionid>{1}</subscriptionid>
                <currency>{2}</currency>
                <amount>{3}</amount>
                {4}
                </recur >", recur.CustomerRefNo, recur.SubscriptionId, recur.Currency, recur.Amount, vat);
        AddCorrelationIdHeader(recur.CorrelationId);
        return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, Headers, "/recur");
    }

    private void AddCorrelationIdHeader(Guid? correlationId)
    {
        Headers.Add(new AdminRequestHeader("X-Svea-CorrelationId", correlationId));
    }
}