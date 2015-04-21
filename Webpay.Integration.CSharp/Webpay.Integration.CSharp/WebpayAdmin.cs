using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp
{
    /// <summary>
    /// Admin root. If you are looking for invoice and paymentplan administrative functionality,
    /// please use the SOAP/WSDL integrations.
    /// </summary>
    public class WebpayAdmin
    {
        /// <summary>
        /// Provides the Admin API for the so called Hosted services, meaning the API that integrates
        /// administration for bank, card, and similar payment services.
        /// </summary>
        public static HostedAdmin Hosted(IConfigurationProvider configurationProvider, string merchantId, CountryCode countryCode)
        {
            return new HostedAdmin(configurationProvider, merchantId, countryCode);
        }
    }
}