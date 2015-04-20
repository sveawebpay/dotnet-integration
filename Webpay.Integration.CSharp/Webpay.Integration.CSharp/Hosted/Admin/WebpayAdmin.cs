using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class WebpayAdmin
    {
        public static HostedAdmin Hosted(IConfigurationProvider configurationProvider, string merchantId, CountryCode countryCode)
        {
            return new HostedAdmin(configurationProvider, merchantId, countryCode);
        }
    }
}