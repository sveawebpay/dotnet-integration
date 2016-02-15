using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webpay.Integration.CSharp;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
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

        [Obsolete("Use SetOrderId() instead")]
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

        public HostedActionRequest QueryCardOrder()
        {
            // should validate this.GetOrderId() existence here

            var hostedActionRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Query(new QueryByTransactionId(
                    transactionId: this.GetOrderId()
                    ));

            return hostedActionRequest;
        }

        public HostedActionRequest QueryDirectBankOrder()
        {
            // should validate this.GetOrderId() existence here

            var hostedActionRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Query(new QueryByTransactionId(
                    transactionId: this.GetOrderId()
                    ));

            return hostedActionRequest;
        }
    }
}
