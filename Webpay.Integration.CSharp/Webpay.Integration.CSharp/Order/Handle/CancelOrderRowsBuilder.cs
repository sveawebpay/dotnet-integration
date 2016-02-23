using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class CancelOrderRowsBuilder : Builder<CancelOrderRowsBuilder>
    {
        internal long Id { get; private set; }
        internal PaymentType OrderType { get; set; }
        //internal DateTime? CaptureDate { get; private set; }
        //internal DistributionType DistributionType { get; private set; }
        internal List<long> RowIndexesToDeliver { get; private set; }
        internal List<NumberedOrderRowBuilder> NumberedOrderRows { get; private set; }

        public CancelOrderRowsBuilder(IConfigurationProvider config) : base(config)
        {
            //this.CaptureDate = null;
            this.RowIndexesToDeliver = new List<long>();
            this.NumberedOrderRows = new List<NumberedOrderRowBuilder>();
        }

        public CancelOrderRowsBuilder SetOrderId(long orderId)
        {
            Id = orderId;
            return this;
        }

        public CancelOrderRowsBuilder SetTransactionId(long orderId)
        {
            return SetOrderId(orderId);
        }

        public override CancelOrderRowsBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        //public CancelOrderRowsBuilder SetInvoiceDistributionType(DistributionType distributionType)
        //{
        //    DistributionType = distributionType;
        //    return this;
        //}

        public CancelOrderRowsBuilder SetRowToDeliver(long rowIndexToDeliver)
        {
            RowIndexesToDeliver.Add(rowIndexToDeliver);
            return this;
        }

        public CancelOrderRowsBuilder AddNumberedOrderRows(IList<NumberedOrderRowBuilder> numberedOrderRows)
        {
            NumberedOrderRows.AddRange(numberedOrderRows);
            return this;
        }

        public AdminService.CancelOrderRowsRequest CancelInvoiceOrderRows()
        {
            OrderType = PaymentType.INVOICE;
            return new AdminService.CancelOrderRowsRequest(this);
        }

        public AdminService.CancelOrderRowsRequest CancelPaymentPlanOrderRows()
        {
            OrderType = PaymentType.PAYMENTPLAN;
            return new AdminService.CancelOrderRowsRequest(this);
        }

        //public AdminService.LowerTransactionRequest DeliverCardOrderRows()
        //{
        //    return new AdminService.LowerTransactionRequest(this);
        //}
    }
}