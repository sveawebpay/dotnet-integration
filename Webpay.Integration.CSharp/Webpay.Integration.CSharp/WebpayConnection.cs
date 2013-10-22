using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Handle;
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
        public static CreateOrderBuilder CreateOrder(IConfigurationProvider config)
        {
            return new CreateOrderBuilder(config);
        }

        /// <summary>
        /// Start build order request to create an order for all payments.
        /// </summary>
        /// <returns>CreateOrderBuilder</returns>
        public static CreateOrderBuilder CreateOrder()
        {
            return CreateOrder(SveaConfig.GetDefaultConfig());
        }

        /// <summary>
        /// Start building request to close order.
        /// </summary>
        /// <param name="config"></param>
        /// <returns>CloseOrderBuilder</returns>
        public static CloseOrderBuilder CloseOrder(IConfigurationProvider config)
        {
            return new CloseOrderBuilder(config);
        }

        /// <summary>
        /// Start building request to close order.
        /// </summary>
        /// <returns>CloseOrderBuilder</returns>
        public static CloseOrderBuilder CloseOrder()
        {
            return CloseOrder(SveaConfig.GetDefaultConfig());
        }

        /// <summary>
        /// Starts building request for deliver order.
        /// </summary>
        /// <param name="config"></param>
        /// <returns>DeliverOrderBuilder</returns>
        public static DeliverOrderBuilder DeliverOrder(IConfigurationProvider config)
        {
            return new DeliverOrderBuilder(config);
        }

        /// <summary>
        /// Starts building request for deliver order.
        /// </summary>
        /// <returns>DeliverOrderBuilder</returns>
        public static DeliverOrderBuilder DeliverOrder()
        {
            return DeliverOrder(SveaConfig.GetDefaultConfig());
        }

        /// <summary>
        /// Get payment plan parameters to present to customer before creating a payment plan payment request
        /// </summary>
        /// <param name="config"></param>
        /// <returns>GetPaymentPlanParams</returns>
        public static GetPaymentPlanParams GetPaymentPlanParams(IConfigurationProvider config)
        {
            return new GetPaymentPlanParams(config);
        }

        /// <summary>
        /// Get payment plan parameters to present to customer before creating a payment plan payment request
        /// </summary>
        /// <returns>GetPaymentPlanParams</returns>
        public static GetPaymentPlanParams GetPaymentPlanParams()
        {
            return GetPaymentPlanParams(SveaConfig.GetDefaultConfig());
        }

        /// <summary>
        /// Start building request for getting addresses.
        /// </summary>
        /// <param name="config"></param>
        /// <returns>GetAddresses</returns>
        public static GetAddresses GetAddresses(IConfigurationProvider config)
        {
            return new GetAddresses(config);
        }

        /// <summary>
        /// Start building request for getting addresses.
        /// </summary>
        /// <returns>GetAddresses</returns>
        public static GetAddresses GetAddresses()
        {
            return GetAddresses(SveaConfig.GetDefaultConfig());
        }
    }
}