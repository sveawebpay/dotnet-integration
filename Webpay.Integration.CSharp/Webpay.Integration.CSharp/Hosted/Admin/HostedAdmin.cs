using Webpay.Integration.CSharp.Config;
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
            throw new System.NotImplementedException();
        }
    }
}