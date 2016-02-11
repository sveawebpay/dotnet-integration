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
        public PaymentType PayType { get; set; }

        public QueryOrderBuilder(IConfigurationProvider config)
        {
            _config = config;
        }

        public QueryOrderBuilder SetOrderId(long orderId)
        {
            _orderId = orderId;
            return this;
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
            PayType = PaymentType.INVOICE;
            return new AdminService.GetOrdersRequest(this);
        }

        public AdminService.GetOrdersRequest QueryPaymentPlanOrder()
        {
            PayType = PaymentType.PAYMENTPLAN;
            return new AdminService.GetOrdersRequest(this);
        }

        public HostedActionRequest QueryCardOrder()
        {
            // should validate this.GetOrderId() existence here

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Query(new QueryByTransactionId(
                    transactionId: this.GetOrderId()
                    ));

            return preparedHostedAdminRequest;
        }
    }
}
