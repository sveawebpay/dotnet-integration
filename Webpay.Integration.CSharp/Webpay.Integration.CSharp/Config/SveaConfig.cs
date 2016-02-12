using System.Reflection.Emit;

namespace Webpay.Integration.CSharp.Config
{
    public class SveaConfig
    {
        private const string HostedTestAdminBaseUrl = "https://test.sveaekonomi.se/webpay/rest";
        private const string HostedProdAdminBaseUrl = "https://webpay.sveaekonomi.se/webpay/rest";
        private const string SwpTestUrl = "https://test.sveaekonomi.se/webpay/payment";
        private const string SwpProdUrl = "https://webpay.sveaekonomi.se/webpay/payment";
        private const string SwpTestWsUrl = "https://webservices.sveaekonomi.se/webpay_test/SveaWebPay.asmx?WSDL";
        private const string SwpProdWsUrl = "https://webservices.sveaekonomi.se/webpay/SveaWebPay.asmx?WSDL";
        private const string SwpTestAdminWsUrl = "https://partnerweb.sveaekonomi.se/WebPayAdminService_test/AdminService.svc" + "/secure"; // make sure to include "/secure" part
        private const string SwpProdAdminWsUrl = "https://partnerweb.sveaekonomi.se/WebPayAdminService/AdminService.svc" + "/secure"; // make sure to include "/secure" part

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
            return SwpProdUrl;
        }

        public static string GetTestHostedAdminUrl()
        {
            return HostedTestAdminBaseUrl;
        }

        public static string GetProdHostedAdminUrl()
        {
            return HostedProdAdminBaseUrl;
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
            return SwpTestUrl;
        }

        public static IConfigurationProvider GetDefaultConfig()
        {
            return new SveaTestConfigurationProvider();
        }
    }
}