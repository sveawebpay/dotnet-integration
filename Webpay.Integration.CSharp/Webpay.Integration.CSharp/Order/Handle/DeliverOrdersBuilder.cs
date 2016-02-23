using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class DeliverOrdersBuilder : Builder<DeliverOrdersBuilder>
    {
        internal List<long> OrderIds { get; private set; }
        internal DistributionType DistributionType { get; private set; }
        public PaymentType OrderType { get; set; }


        public DeliverOrdersBuilder(IConfigurationProvider config) : base(config)
        {
            OrderIds = new List<long>();
        }

        public DeliverOrdersBuilder SetOrderId(long orderId)
        {
            OrderIds.Add(orderId);
            return this;
        }
        public DeliverOrdersBuilder SetOrderIds(IList<long> orderIds)
        {
            OrderIds.AddRange(orderIds);
            return this;
        }
        public override DeliverOrdersBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public DeliverOrdersBuilder SetInvoiceDistributionType(DistributionType distributionType)
        {
            DistributionType = distributionType;
            return this;
        }

        public AdminService.DeliverOrdersRequest DeliverInvoiceOrders()
        {
            OrderType = PaymentType.INVOICE;
            return new AdminService.DeliverOrdersRequest(this);
        }

        public AdminService.DeliverOrdersRequest DeliverPaymentPlanOrders()
        {
            OrderType = PaymentType.PAYMENTPLAN;
            DistributionType = DistributionType.POST;   // always use Post for payment plan orders
            return new AdminService.DeliverOrdersRequest(this);
        }
    }
}
