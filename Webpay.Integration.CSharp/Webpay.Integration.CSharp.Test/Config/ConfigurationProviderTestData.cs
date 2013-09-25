using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Config
{
    public class ConfigurationProviderTestData : IConfigurationProvider
    {
        public string GetUsername(PaymentType type, CountryCode country)
        {
            return "sverigetest";
        }

        public string GetPassword(PaymentType type, CountryCode country)
        {
            return "sverigetest";
        }

        public int GetClientNumber(PaymentType type, CountryCode country)
        {
            return 79021;
        }

        public string GetMerchantId(PaymentType type, CountryCode country)
        {
            return "1130";
        }

        public string GetSecret(PaymentType type, CountryCode country)
        {
            return
                "8a9cece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3";
        }

        public string GetEndPoint(PaymentType type)
        {
            if (PaymentType.HOSTED == type)
                return SveaConfig.GetTestPayPageUrl();
            return SveaConfig.GetTestWebserviceUrl();
        }
    }
}