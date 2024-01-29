using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class UpdateOrderRowsBuilder : Builder<UpdateOrderRowsBuilder>
    {
        internal long Id { get; private set; }
        internal PaymentType OrderType { get; set; }
        internal List<NumberedOrderRowBuilder> NumberedOrderRows { get; private set; }

        public UpdateOrderRowsBuilder(IConfigurationProvider config) : base(config)
        {
            this.NumberedOrderRows = new List<NumberedOrderRowBuilder>();
        }

        public UpdateOrderRowsBuilder SetOrderId(long orderId)
        {
            Id = orderId;
            return this;
        }

        public override UpdateOrderRowsBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }


        public UpdateOrderRowsBuilder AddUpdateOrderRow(NumberedOrderRowBuilder numberedOrderRow)
        {
            NumberedOrderRows.Add(numberedOrderRow);
            return this;
        }
        public UpdateOrderRowsBuilder AddUpdateOrderRows( IList<NumberedOrderRowBuilder> numberedOrderRows)
        {
            NumberedOrderRows.AddRange(numberedOrderRows);
            return this;
        }

        public AdminService.UpdateOrderRowsRequest UpdateInvoiceOrderRows()
        {
            OrderType = PaymentType.INVOICE;
            return new AdminService.UpdateOrderRowsRequest(this);
        }
        public AdminService.UpdateOrderRowsRequest UpdatePaymentPlanOrderRows()
        {
            OrderType = PaymentType.PAYMENTPLAN;
            return new AdminService.UpdateOrderRowsRequest(this);
        }

        public override UpdateOrderRowsBuilder SetCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}