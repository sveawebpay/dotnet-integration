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
        /// Use WebpayAdmin.DeliverOrders to deliver one or more orders of the same type without specifying order rows.
        /// All order rows as currently held by Svea will be delivered when the request is made.
        /// 
        /// When delivered, invoice orders will return a corresponding invoice id, and payment plan orders a contract number. 
        /// These may be found in the returned DeliverOrderResult structure along with the corresponding order id.
        /// 
        /// ...
        ///  DeliverOrdersBuilder request = WebpayAdmin.DeliverOrders(config)
        ///   .SetOrderId()                 // optional, order id to deliver
        ///   .SetOrderIds()                // optional, note that order ids must all be of the same type (invoice, part payment) 
        ///   .SetCountryCode()             // required
        ///   .SetInvoiceDistributionType() // required for invoice only, will apply to all invoice orders
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.DeliverInvoiceOrders().DoRequest();     // returns AdminWS.DeliveryResponse
        ///  response = request.DeliverPaymentPlanOrders().DoRequest();  // returns AdminWS.DeliveryResponse
        ///  ...
        /// </summary>
        public static DeliverOrdersBuilder DeliverOrders(IConfigurationProvider configurationProvider)
        {
            return new DeliverOrdersBuilder(configurationProvider);
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
        ///  CancelOrderRowsBuilder request = WebpayAdmin.CancelOrderRows(config)
        ///   .SetInvoiceId()               // required for invoice only, use invoice number recieved with deliverOrder response
        ///   .SetTransactionId()           // optional, card only, alias for SetOrderId()
        ///   .SetCountryCode()             // required
        ///   .SetRowToCancel()             // optional, index of original order rows you wish to deliver
        ///   .AddNumberedOrderRow()        // required for card orders, should match orginal row indexes
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.CancelInvoiceOrderRows().DoRequest();           // returns AdminWS.DeliveryResponse
        ///  response = request.CancelPaymentPlanOrderRows().DoRequest();       // returns AdminWS.CancelPaymentPlanRowsResponse
        ///  response = request.CancelCardOrderRows().DoRequest();              // returns Hosted.Admin.Response.LowerTransactionResponse
        /// ...
        /// </summary>
        public static CancelOrderRowsBuilder CancelOrderRows(IConfigurationProvider configurationProvider)
        {
            return new CancelOrderRowsBuilder(configurationProvider);
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
        ///  response = request.CreditPaymentPlanOrderRows().DoRequest();       // returns AdminWS.CancelPaymentPlanRowsResponse
        /// ...
        /// </summary>
        public static CreditOrderRowsBuilder CreditOrderRows(IConfigurationProvider configurationProvider)
        {
            return new CreditOrderRowsBuilder(configurationProvider);
        }

        /// <summary>
        /// ...
        ///  UpdateOrderRowsBuilder request = WebpayAdmin.UpdateOrderRows(config)
        ///   .SetOrderId()                 // required, use svea order number recieved in create order response
        ///   .SetCountryCode()             // required
        ///   .AddUpdateOrderRow()          // required, new NumberedOrderRow, replaces original order row with matching row number
        ///  ;
        ///  // then select the corresponding request class and send request
        ///  response = request.UpdateInvoiceOrderRows().DoRequest();           // returns AdminWS.UpdateOrderRowsResponse
        ///  response = request.UpdatePaymentPlanOrderRows().DoRequest();       // returns AdminWS.UpdateOrderRowsResponse
        /// ...
        /// </summary>
        public static UpdateOrderRowsBuilder UpdateOrderRows(IConfigurationProvider configurationProvider)
        {
            return new UpdateOrderRowsBuilder(configurationProvider);
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
 
 