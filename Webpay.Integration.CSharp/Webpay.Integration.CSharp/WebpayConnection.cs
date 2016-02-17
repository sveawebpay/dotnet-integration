using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Getaddresses;
using Webpay.Integration.CSharp.Webservice.Getpaymentplanparams;

namespace Webpay.Integration.CSharp
{
    /// <summary>
    /// Start build request object by choosing the right method.
    /// </summary>
    public static class WebpayConnection
    {
        /// <summary>
        /// Start build order request to create an order for all payments.
        /// </summary>
        /// <param name="config"></param>
        /// <returns>CreateOrderBuilder</returns>
        public static CreateOrderBuilder CreateOrder(IConfigurationProvider config = null)
        {
            if (config == null)
            {
                throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
            }

            return new CreateOrderBuilder(config);
        }

        /// <summary>
        /// Start building request to close order.
        /// </summary>
        /// <param name="config"></param>
        /// <remarks>Deprecated</remarks>
        /// <returns>CloseOrderBuilder</returns>
        [Obsolete("Use WebpayAdmin.CancelOrder() instead")]
        public static CloseOrderBuilder CloseOrder(IConfigurationProvider config = null)
        {
            if (config == null)
            {
                throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
            }

            return new CloseOrderBuilder(config);
        }

        /// <summary>
        ///     DeliverOrderBuilder request = WebpayConnection.DeliverOrder(config)
        ///         .SetOrderId()                  // invoice or payment plan only, required
        ///???         .SetTransactionId()            // card only, optional, alias for setOrderId
        ///         .SetCountryCode()              // required
        ///         .SetInvoiceDistributionType()  // invoice only, required
        ///         .SetNumberOfCreditDays()       // invoice only, optional
        ///         .SetCaptureDate()              // card only, optional
        ///         .AddOrderRow()                 // deprecated, optional -- use WebPayAdmin.DeliverOrderRows instead
        ///         .SetCreditInvoice()            // deprecated, optional -- use WebPayAdmin.CreditOrderRows instead
        ///     ;
        ///     // then select the corresponding request class and send request
        ///     response = request.DeliverInvoiceOrder().doRequest();       // returns DeliverOrderResponse
        ///     response = request.DeliverPaymentPlanOrder().doRequest();   // returns DeliverOrderResponse
        ///     response = request.DeliverCardOrder().doRequest();          // returns ConfirmTransactionResponse
        /// </summary>
        /// <param name="config"></param>
        /// <returns>DeliverOrderBuilder</returns>
        public static DeliverOrderBuilder DeliverOrder(IConfigurationProvider config = null)
        {
            if (config == null)
            {
                throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
            }

            return new DeliverOrderBuilder(config);
        }

        /// <summary>
        /// Get payment plan parameters to present to customer before creating a payment plan payment request
        /// </summary>
        /// <param name="config"></param>
        /// <returns>GetPaymentPlanParams</returns>
        public static GetPaymentPlanParams GetPaymentPlanParams(IConfigurationProvider config = null)
        {
            if (config == null)
            {
                throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
            }

            return new GetPaymentPlanParams(config);
        }

        /// <summary>
        /// Start building request for getting addresses.
        /// </summary>
        /// <param name="config"></param>
        /// <returns>GetAddresses</returns>
        public static GetAddresses GetAddresses(IConfigurationProvider config = null)
        {
            if (config == null)
            {
                throw new SveaWebPayException("A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()");
            }

            return new GetAddresses(config);
        }

        /// <summary>
        /// Calculate the prices per month for the payment plan campaigns
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="paymentPlanParams"></param>
        /// <returns>PaymentPlanPricePerMonth</returns>
        public static List<Dictionary<string, long>> PaymentPlanPricePerMonth(decimal amount, GetPaymentPlanParamsEuResponse paymentPlanParams)
        {
            return new PaymentPlanPricePerMonth().Calculate(amount, paymentPlanParams);
        }
    }
}