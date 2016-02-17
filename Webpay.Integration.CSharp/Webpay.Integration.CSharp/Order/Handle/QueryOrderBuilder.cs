using System;
using Webpay.Integration.CSharp.AdminService;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class QueryOrderBuilder : Builder<QueryOrderBuilder>
    {
        private long _orderId;
        public PaymentType OrderType { get; set; }

        public QueryOrderBuilder(IConfigurationProvider config) : base(config)
        {
            // intentionally left blank
        }

        public QueryOrderBuilder SetOrderId(long orderId)
        {
            _orderId = orderId;
            return this;
        }

        public QueryOrderBuilder SetTransactionId(long orderId)
        {
            return this.SetOrderId(orderId);
        }

        public long GetOrderId()
        {
            return _orderId;
        }

        public override QueryOrderBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public AdminService.GetOrdersRequest QueryInvoiceOrder()
        {
            OrderType = PaymentType.INVOICE;
            return new AdminService.GetOrdersRequest(this);
        }

        public AdminService.GetOrdersRequest QueryPaymentPlanOrder()
        {
            OrderType = PaymentType.PAYMENTPLAN;
            return new AdminService.GetOrdersRequest(this);
        }

        public QueryTransactionRequest QueryCardOrder()
        {
            return new AdminService.QueryTransactionRequest(this);
        }

        public QueryTransactionRequest QueryDirectBankOrder()
        {
            return new AdminService.QueryTransactionRequest(this);
        }
    }
}
