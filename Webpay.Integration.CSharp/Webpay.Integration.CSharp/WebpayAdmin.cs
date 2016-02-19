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
        /// ...
        ///  CreditOrderRowsBuilder request = WebpayAdmin.CreditOrderRows(config)
        ///   .SetInvoiceId()               // required for invoice only, use invoice number recieved with deliverOrder response
        ///   .SetContractNumber()          // required for payment plan only, use contract number recieved with deliverOrder response
        ///   .SetInvoiceDistributionType() // required for invoice only
        ///   .SetCountryCode()             // required
        ///   .AddCreditOrderRow()          // optional, use to specify a new credit row, i.e. for amounts not present in the original order
        ///   .SetRowToCredit()             // optional, index of original order rows you wish to deliver
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.CreditInvoiceOrderRows().DoRequest();           // returns AdminWS.DeliveryResponse
        ///  ???response = request.CreditPaymentPlanOrderRows().DoRequest();    // returns AdminWS.xxx
        /// ...
        /// </summary>
        public static CreditOrderRowsBuilder CreditOrderRows(IConfigurationProvider configurationProvider)
        {
            return new CreditOrderRowsBuilder(configurationProvider);
        }

        /// <summary>
        ///  ...
        ///  CancelOrderBuilder request = WebpayAdmin.CancelOrder(config)
        ///   .SetOrderId()                 // required, use SveaOrderId recieved with createOrder response
        ///   .SetTransactionId()           // optional, card or direct bank only, alias for SetOrderId
        ///   .SetCountryCode()             // required
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.CancelInvoiceOrder().DoRequest();       // returns AdminWS.CancelOrderResponse
        ///  response = request.CancelPaymentPlanOrder().DoRequest();   // returns AdminWS.CancelOrderResponse
        ///  response = request.CancelCardOrder().DoRequest();          // returns Hosted.Admin.Actions.AnnulResponse
        ///  ...
        /// </summary>
        public static CancelOrderBuilder CancelOrder(IConfigurationProvider configurationProvider)
        {
            return new CancelOrderBuilder(configurationProvider);
        }


        /// <summary>
        ///  ...
        ///  CreditAmountBuilder request = WebpayAdmin.CreditAmount(config)
        ///   .SetContractNumber()          // required for payment plan only, use contract number recieved with deliverOrder response
        ///   .SetTransactionId()           // required for card or direct bank only
        ///   .SetCountryCode()             // required for payment plan only
        ///   .SetDescription()             // optional for payment plan only, description to print on resulting cancellation rows
        ///   .SetAmountIncVat()            // required, amount to credit
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.CreditPaymentPlanAmount().DoRequest();  // returns AdminWS.CancelPaymentPlanAmountResponse
        ///  response = request.CreditCardAmount().DoRequest();         // returns Hosted.Admin.Actions.CreditResponse
        ///  response = request.CreditDirectBankAmount().DoRequest();   // returns Hosted.Admin.Actions.CreditResponse
        ///  ...
        /// </summary>
        public static CreditAmountBuilder CreditAmount(IConfigurationProvider configurationProvider)
        {
            return new CreditAmountBuilder(configurationProvider);
        }
    }
}
 
 