using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Order.Handle;
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
        public static HostedAdmin Hosted(IConfigurationProvider configurationProvider, CountryCode countryCode)
        {
            return new HostedAdmin(configurationProvider, countryCode);
        }

        /// <summary>
        /// ...
        ///  QueryOrderBuilder request = WebPayAdmin.QueryOrder(config)
        ///   .SetOrderId()              // required
        ///   .SetTransactionId()        // optional, card or direct bank only, alias for setOrderId
        ///   .SetCountryCode()          // required
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.QueryInvoiceOrder().DoRequest();         // returns AdminWS.GetOrdersResponse
        ///  response = request.QueryPaymentPlanOrder().DoRequest();     // returns AdminWS.GetOrdersResponse
        ///  response = request.QueryCardOrder().DoRequest();            // returns Hosted.Admin.HostedAdminResponse
        ///  response = request.QueryDirectBankOrder().DoRequest();      // returns Hosted.Admin.HostedAdminResponse
        ///  ...
        /// </summary>
        public static QueryOrderBuilder QueryOrder(IConfigurationProvider configurationProvider)
        {
            return new QueryOrderBuilder(configurationProvider);
        }
    }
}