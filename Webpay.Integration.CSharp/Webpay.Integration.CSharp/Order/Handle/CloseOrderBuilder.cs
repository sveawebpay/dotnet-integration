using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Webservice.Handleorder;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class CloseOrderBuilder
    {
        private CountryCode _countryCode;
        private long _orderId;
        private string _orderType;
        private readonly IConfigurationProvider _config;

        public CloseOrderBuilder(IConfigurationProvider config)
        {
            _config = config;
        }

        public IConfigurationProvider GetConfig()
        {
            return _config;
        }

        public long GetOrderId()
        {
            return _orderId;
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>CloseOrderBuilder</returns>
        public CloseOrderBuilder SetOrderId(long orderId)
        {
            _orderId = orderId;
            return this;
        }

        public string GetOrderType()
        {
            return _orderType;
        }

        public CloseOrderBuilder SetCountrycode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public CountryCode GetCountrycode()
        {
            return _countryCode;
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns>CloseOrderBuilder</returns>
        public CloseOrderBuilder SetOrderType(string orderType)
        {
            _orderType = orderType;
            return this;
        }

        public CloseOrder CloseInvoiceOrder()
        {
            _orderType = "Invoice";
            return new CloseOrder(this);
        }

        public CloseOrder ClosePaymentPlanOrder()
        {
            _orderType = "PaymentPlan";
            return new CloseOrder(this);
        }
    }
}