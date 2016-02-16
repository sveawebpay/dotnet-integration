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
        ///  QueryOrderBuilder request = WebpayAdmin.QueryOrder(config)
        ///   .SetOrderId()                 // required
        ///   .SetTransactionId()           // optional, card or direct bank only, alias for SetOrderId
        ///   .SetCountryCode()             // required
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.QueryInvoiceOrder().DoRequest();         // returns AdminWS.GetOrdersResponse
        ///  response = request.QueryPaymentPlanOrder().DoRequest();     // returns AdminWS.GetOrdersResponse
        ///  response = request.QueryCardOrder().DoRequest();            // returns Hosted.Admin.Actions.QueryResponse
        ///  response = request.QueryDirectBankOrder().DoRequest();      // returns Hosted.Admin.Actions.QueryResponse
        ///  ...
        /// </summary>
        public static QueryOrderBuilder QueryOrder(IConfigurationProvider configurationProvider)
        {
            return new QueryOrderBuilder(configurationProvider);
        }

        /// <summary>
        /// ...
        ///  DeliverOrderRowsBuilder request = WebpayAdmin.DeliverOrderRows(config)
        ///   .SetOrderId()                 // required
        ///   .SetTransactionId()           // optional, card or direct bank only, alias for SetOrderId
        ///   .SetCountryCode()             // required
        ///   .SetInvoiceDistributionType() // required for invoice only
        ///   .SetRowToDeliver()            // required, index of original order rows you wish to deliver
        ///   .AddNumberedOrderRow()        // required for card orders, should match original row indexes
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.DeliverInvoiceOrderRows().DoRequest();   // returns AdminWS.DeliverOrderRowsResponse
        ///  response = request.DeliverCardOrder().DoRequest();          // returns Hosted.Admin.Actions.ConfirmResponse
        ///  ...
        /// </summary>
        public static DeliverOrderRowsBuilder DeliverOrderRows(IConfigurationProvider configurationProvider)
        {
            return new DeliverOrderRowsBuilder(configurationProvider);
        }

        /// <summary>
        ///  ...
        ///  CancelOrderBuilder request = WebpayAdmin.CancelOrder(config)
        ///   .SetOrderId()                 // required, use SveaOrderId recieved with createOrder response
        ///   .SetTransactionId()           // optional, card or direct bank only, alias for SetOrderId
        ///   .SetCountryCode()             // required
        ///  ;
        ///  // then select the corresponding request class and send request
        ///???  response = request.CancelInvoiceOrder().DoRequest();           // returns AdminWS.CancelOrderResponse
        ///???  response = request.CancelPaymentPlanOrder().DoRequest();	   // returns AdminWS.CancelOrderResponse
        ///  response = request.CancelCardOrder().DoRequest();              // returns Hosted.Admin.Actions.AnnulResponse
        ///  ...
        /// </summary>
        public static CancelOrderBuilder CancelOrder(IConfigurationProvider configurationProvider)
        {
            return new CancelOrderBuilder(configurationProvider);
        }
    }
}
 
 