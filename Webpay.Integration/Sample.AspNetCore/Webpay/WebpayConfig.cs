using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Util.Constant;

namespace Sample.AspNetCore.Webpay;

public class WebpayConfig : IConfigurationProvider
{
    public string MyUserName { get; set; }
    public string MyPassword { get; set; }
    public int MyClientNumber { get; set; }
    public string MyMerchantId { get; set; }
    public string MySecretWord { get; set; }

    public string GetUsername(PaymentType type, CountryCode country)
    {
        return MyUserName;
    }

    public string GetPassword(PaymentType type, CountryCode country)
    {
        return MyPassword;
    }

    public int GetClientNumber(PaymentType type, CountryCode country)
    {
        return MyClientNumber;
    }

    public string GetMerchantId(PaymentType type, CountryCode country)
    {
        return MyMerchantId;
    }

    public string GetSecretWord(PaymentType type, CountryCode country)
    {
        return MySecretWord;
    }

    public string GetEndPoint(PaymentType type) => type switch
    {
        PaymentType.HOSTED => SveaConfig.GetTestPayPageUrl(),
        PaymentType.INVOICE => SveaConfig.GetTestWebserviceUrl(),
        PaymentType.PAYMENTPLAN => SveaConfig.GetTestWebserviceUrl(),
        PaymentType.ACCOUNTCREDIT => SveaConfig.GetTestWebserviceUrl(),
        PaymentType.ADMIN_TYPE => SveaConfig.GetTestAdminServiceUrl(),
        _ => throw new SveaWebPayException("Unknown PaymentType")
    };
}
