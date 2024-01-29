using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class QueryOrderBuilder : Builder<QueryOrderBuilder>
    {
        internal long Id { get; private set; }
        internal PaymentType OrderType { get; private set; }

        public QueryOrderBuilder(IConfigurationProvider config) : base(config)
        {
            // intentionally left blank
        }

        public QueryOrderBuilder SetOrderId(long orderId)
        {
            Id = orderId;
            return this;
        }

        public QueryOrderBuilder SetTransactionId(long orderId)
        {
            return this.SetOrderId(orderId);
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

        public AdminService.QueryTransactionRequest QueryCardOrder()
        {
            return new AdminService.QueryTransactionRequest(this);
        }

        public AdminService.QueryTransactionRequest QueryDirectBankOrder()
        {
            return new AdminService.QueryTransactionRequest(this);
        }

        public override QueryOrderBuilder SetCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}
