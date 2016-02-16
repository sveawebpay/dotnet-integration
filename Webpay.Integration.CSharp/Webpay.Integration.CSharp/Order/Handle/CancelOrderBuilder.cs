using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class CancelOrderBuilder : Builder<CancelOrderBuilder>
    {
        private long _orderId;
        public PaymentType OrderType { get; set; }

        public CancelOrderBuilder(IConfigurationProvider config) : base(config)
        {
            // intentionally left blank
        }

        public CancelOrderBuilder SetOrderId(long orderId)
        {
            _orderId = orderId;
            return this;
        }

        public CancelOrderBuilder SetTransactionId(long orderId)
        {
            return this.SetOrderId(orderId);
        }

        public long GetOrderId()
        {
            return _orderId;
        }

        public override CancelOrderBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public AdminService.CancelOrderRequest CancelInvoiceOrder()
        {
            OrderType = PaymentType.INVOICE;
            return new AdminService.CancelOrderRequest(this);
        }
    }
}