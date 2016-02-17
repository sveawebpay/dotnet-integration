using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class DeliverOrderRowsBuilder : Builder<DeliverOrderRowsBuilder>
    {
        private long _orderId;
        internal PaymentType OrderType { get; set; }

        internal DistributionType _distributionType { get; set; }
        internal List<long> _rowIndexesToDeliver { get; set; }
        internal List<NumberedOrderRowBuilder> _numberedOrderRows { get; set; }

        internal DateTime? _captureDate;

        public DeliverOrderRowsBuilder(IConfigurationProvider config) : base(config)
        {
            this._captureDate = null;
            this._rowIndexesToDeliver = new List<long>();
            this._numberedOrderRows = new List<NumberedOrderRowBuilder>();
        }


        public DeliverOrderRowsBuilder SetOrderId(long orderId)
        {
            _orderId = orderId;
            return this;
        }

        public DeliverOrderRowsBuilder SetTransactionId(long orderId)
        {
            return this.SetOrderId(orderId);
        }

        public long GetOrderId()
        {
            return _orderId;
        }

        public override DeliverOrderRowsBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public DeliverOrderRowsBuilder SetInvoiceDistributionType(DistributionType distributionType)
        {
            _distributionType = distributionType;
            return this;
        }

        public DeliverOrderRowsBuilder SetRowToDeliver( long rowIndexToDeliver )
        {
                this._rowIndexesToDeliver.Add(rowIndexToDeliver);
                return this;
        }

        public AdminService.DeliverOrderRowsRequest DeliverInvoiceOrderRows()
        {
            OrderType = PaymentType.INVOICE;
            return new AdminService.DeliverOrderRowsRequest(this);
        }

        public DeliverOrderRowsBuilder AddNumberedOrderRows(IList<NumberedOrderRowBuilder> numberedOrderRows)
        {
            this._numberedOrderRows.AddRange(numberedOrderRows);
            return this;
        }

        public AdminService.ConfirmTransactionRequest DeliverCardOrderRows()
        {
            return new AdminService.ConfirmTransactionRequest(this);
        }
    }
}