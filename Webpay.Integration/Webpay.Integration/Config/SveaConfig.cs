namespace Webpay.Integration.Config;

public class SveaConfig
{
    private const string HostedTestAdminBaseUrl = "https://webpaypaymentgatewaystage.svea.com/webpay/";
    private const string HostedProdAdminBaseUrl = "https://webpaypaymentgateway.svea.com/webpay/";
    //private const string SwpTestUrl = "https://webpaypaymentgatewaystage.svea.com/webpay/payment";
    //private const string SwpProdUrl = "https://webpaypaymentgateway.svea.com/webpay/payment";
    private const string SwpTestWsUrl = "https://webpaywsstage.svea.com/SveaWebPay.asmx?WSDL";
    private const string SwpProdWsUrl = "https://webpayws.svea.com/SveaWebPay.asmx?WSDL";
    private const string SwpTestAdminWsUrl = "https://webpayadminservicestage.svea.com/AdminService.svc" + "/secure";
    private const string SwpProdAdminWsUrl = "https://webpayadminservice.svea.com/AdminService.svc" + "/secure";

    public static string GetProdWebserviceUrl()
    {
        return SwpProdWsUrl;
    }

    public static string GetProdAdminServiceUrl()
    {
        return SwpProdAdminWsUrl;
    }

    public static string GetProdPayPageUrl()
    {
        return HostedProdAdminBaseUrl + "payment";
    }

    public static string GetTestHostedAdminUrl()
    {
        return HostedTestAdminBaseUrl + "rest";
    }

    public static string GetProdHostedAdminUrl()
    {
        return HostedProdAdminBaseUrl + "rest";
    }

    public static string GetTestWebserviceUrl()
    {
        return SwpTestWsUrl;
    }

    public static string GetTestAdminServiceUrl()
    {
        return SwpTestAdminWsUrl;
    }

    public static string GetTestPayPageUrl()
    {
        return HostedTestAdminBaseUrl + "payment";
    }

    public static IConfigurationProvider GetDefaultConfig()
    {
        return new SveaTestConfigurationProvider();
    }
}