using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.IntegrationTest.Config;

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

    public string GetSecretWord(PaymentType type, CountryCode country)
    {
        return
            "8a9cece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3";
    }

    public string GetEndPoint(PaymentType type)
    {
        return PaymentType.HOSTED == type ? SveaConfig.GetTestPayPageUrl() : SveaConfig.GetTestWebserviceUrl();
    }
}