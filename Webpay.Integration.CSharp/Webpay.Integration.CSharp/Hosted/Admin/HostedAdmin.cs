using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class HostedAdmin
    {
        public IConfigurationProvider ConfigurationProvider { get; private set; }
        public string MerchantId { get; private set; }
        public CountryCode CountryCode { get; private set; }

        public HostedAdmin(IConfigurationProvider configurationProvider, string merchantId, CountryCode countryCode)
        {
            ConfigurationProvider = configurationProvider;
            MerchantId = merchantId;
            CountryCode = countryCode;
        }

        public PreparedHostedAdminRequest Annul(Annul annul)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <annul>
                <transactionid>{0}</transactionid>
                </annul>", annul.TransactionId);

            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/annul");
        }

        public PreparedHostedAdminRequest CancelRecurSubscription(CancelRecurSubscription cancelRecurSubscription)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <cancelrecursubscription>
                <subscriptionid>{0}</subscriptionid>
                </cancelrecursubscription>", cancelRecurSubscription.SubscriptionId);

            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/cancelrecursubscription");
        }

        public PreparedHostedAdminRequest Confirm(Confirm confirm)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <confirm>
                <transactionid>{0}</transactionid>
                <capturedate>{1}</capturedate>
                </confirm>", confirm.TransactionId, confirm.CaptureDate.ToString("yyyy-MM-dd") );

            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/confirm");
        }

        public PreparedHostedAdminRequest GetPaymentMethods(GetPaymentMethods getPaymentMethods)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <getpaymentmethods>
                <merchantid>{0}</merchantid>
                </getpaymentmethods>", getPaymentMethods.MerchantId);

            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/getpaymentmethods");
        }

        public PreparedHostedAdminRequest GetReconciliationReport(GetReconciliationReport getReconciliationReport)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <getreconciliationreport>
                <date>{0}</date>
                </getreconciliationreport>", getReconciliationReport.Date.ToString("yyyy-MM-dd"));

            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/getreconciliationreport");
        }

        public PreparedHostedAdminRequest LowerAmount(LowerAmount lowerAmount)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <loweramount>
                <transactionid>{0}</transactionid>
                <amounttolower>{1}</amounttolower>
                </loweramount>", lowerAmount.TransactionId, lowerAmount.AmountToLower);
            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/loweramount");
        }

        public PreparedHostedAdminRequest Query(QueryByTransactionId query)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <query>
                <transactionid>{0}</transactionid>
                </query>", query.TransactionId);

            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/querytransactionid");

        }

        public PreparedHostedAdminRequest Query(QueryByCustomerRefNo query)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <query>
                <customerrefno>{0}</customerrefno>
                </query>", query.CustomerRefNo);

            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/querycustomerrefno");
        }

        public PreparedHostedAdminRequest Recur(Recur recur)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <recur>
                <customerrefno>{0}</customerrefno>
                <subscriptionid>{1}</subscriptionid>
                <currency>{2}</currency>
                <amount>{3}</amount>
                </recur >", recur.CustomerRefNo, recur.SubscriptionId, recur.Currency, recur.Amount);
            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/recur");
        }
    }
}