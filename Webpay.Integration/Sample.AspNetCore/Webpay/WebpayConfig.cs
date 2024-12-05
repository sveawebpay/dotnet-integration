using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Util.Constant;

namespace Sample.AspNetCore.Webpay;

public class WebpayConfig : IConfigurationProvider
{
    public string GetUsername(PaymentType type, CountryCode country) =>
        GetCredentials(type, country);

    public string GetPassword(PaymentType type, CountryCode country) =>
        GetCredentials(type, country);

    private string GetCredentials(PaymentType type, CountryCode country) =>
        (type, country) switch
        {
            (PaymentType.INVOICE or PaymentType.PAYMENTPLAN, CountryCode.SE) => "sverigetest",
            (PaymentType.INVOICE or PaymentType.PAYMENTPLAN, CountryCode.NO) => "norgetest2",
            (PaymentType.INVOICE or PaymentType.PAYMENTPLAN, CountryCode.FI) => "finlandtest2",
            (PaymentType.INVOICE or PaymentType.PAYMENTPLAN, CountryCode.DK) => "danmarktest2",
            (PaymentType.INVOICE or PaymentType.PAYMENTPLAN, CountryCode.NL) => "hollandtest",
            (PaymentType.INVOICE or PaymentType.PAYMENTPLAN, CountryCode.DE) => "germanytest",
            _ => ""
        };

    public int GetClientNumber(PaymentType type, CountryCode country) => (country, type) switch
    {
        (CountryCode.SE, PaymentType.INVOICE) => 79021,
        (CountryCode.SE, PaymentType.PAYMENTPLAN) => 59999,
        (CountryCode.NO, PaymentType.INVOICE) => 33308,
        (CountryCode.NO, PaymentType.PAYMENTPLAN) => 32503,
        (CountryCode.FI, PaymentType.INVOICE) => 26136,
        (CountryCode.FI, PaymentType.PAYMENTPLAN) => 27136,
        (CountryCode.DK, PaymentType.INVOICE) => 62008,
        (CountryCode.DK, PaymentType.PAYMENTPLAN) => 64008,
        (CountryCode.NL, PaymentType.INVOICE) => 85997,
        (CountryCode.NL, PaymentType.PAYMENTPLAN) => 86997,
        (CountryCode.DE, PaymentType.INVOICE) => 14997,
        (CountryCode.DE, PaymentType.PAYMENTPLAN) => 16997,
        _ => 0
    };

    public string GetMerchantId(PaymentType type, CountryCode country)
    {
        return type == PaymentType.HOSTED ? (country == CountryCode.NO ? "1109" : "1110") : "";
    }

    public string GetSecretWord(PaymentType type, CountryCode country)
    {
        return type == PaymentType.HOSTED
        ? (country == CountryCode.NO ? "9d8d83fe18ed0fe3cf6de8bfd29d82bdaf228f3f8b292b087ec48736b083ce4699c3493a406f02f1300e24490e0c7d0d6d55bfc38dd8e9c1390ac17ce56bdb1a" : "1f8bcd8a564073f7156efd2522d5998f5487a1dcd19e1e120276fb1fb7e233a6059c45d6eb44a8d7342a4989bbb95acd4708051bbc145bda43ae0dd3503928db")
        : "";
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
