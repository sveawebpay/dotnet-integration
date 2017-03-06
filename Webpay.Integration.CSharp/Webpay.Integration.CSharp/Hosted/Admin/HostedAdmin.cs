using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class HostedAdmin
    {
        public readonly IConfigurationProvider ConfigurationProvider;
        public readonly CountryCode CountryCode;
        public readonly string MerchantId;

        public HostedAdmin(IConfigurationProvider configurationProvider, CountryCode countryCode)
        {
            ConfigurationProvider = configurationProvider;
            MerchantId = configurationProvider.GetMerchantId(PaymentType.HOSTED, countryCode);
            CountryCode = countryCode;
        }

        public HostedActionRequest Annul(Annul annul)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <annul>
                <transactionid>{0}</transactionid>
                </annul>", annul.TransactionId);

            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/annul");
        }

        public HostedActionRequest CancelRecurSubscription(CancelRecurSubscription cancelRecurSubscription)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <cancelrecursubscription>
                <subscriptionid>{0}</subscriptionid>
                </cancelrecursubscription>", cancelRecurSubscription.SubscriptionId);

            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider,
                "/cancelrecursubscription");
        }

        public HostedActionRequest Confirm(Confirm confirm)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <confirm>
                <transactionid>{0}</transactionid>
                <capturedate>{1}</capturedate>
                </confirm>", confirm.TransactionId, confirm.CaptureDate.ToString("yyyy-MM-dd"));

            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/confirm");
        }

        public HostedActionRequest Credit(Credit credit)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <credit>
                <transactionid>{0}</transactionid>
                <amounttocredit>{1}</amounttocredit>
                </credit>", credit.TransactionId, credit.AmountToCredit);

            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/credit");
        }

        public HostedActionRequest GetPaymentMethods(GetPaymentMethods getPaymentMethods)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <getpaymentmethods>
                <merchantid>{0}</merchantid>
                </getpaymentmethods>", getPaymentMethods.MerchantId);

            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider,
                "/getpaymentmethods");
        }

        public HostedActionRequest GetReconciliationReport(GetReconciliationReport getReconciliationReport)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <getreconciliationreport>
                <date>{0}</date>
                </getreconciliationreport>", getReconciliationReport.Date.ToString("yyyy-MM-dd"));

            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider,
                "/getreconciliationreport");
        }

        public HostedActionRequest LowerAmount(LowerAmount lowerAmount)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <loweramount>
                <transactionid>{0}</transactionid>
                <amounttolower>{1}</amounttolower>
                </loweramount>", lowerAmount.TransactionId, lowerAmount.AmountToLower);
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/loweramount");
        }

        public HostedActionRequest Query(QueryByTransactionId query)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <query>
                <transactionid>{0}</transactionid>
                </query>", query.TransactionId);

            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider,
                "/querytransactionid");
        }

        public HostedActionRequest Query(QueryByCustomerRefNo query)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <query>
                <customerrefno>{0}</customerrefno>
                </query>", query.CustomerRefNo);

            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider,
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
            return new HostedActionRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/recur");
        }
    }
}