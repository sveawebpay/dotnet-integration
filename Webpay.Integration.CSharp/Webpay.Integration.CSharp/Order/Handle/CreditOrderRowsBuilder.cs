using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class CreditOrderRowsBuilder : Builder<CreditOrderRowsBuilder>
    {
        internal long InvoiceId { get; private set; }
        internal PaymentType OrderType { get; set; }
        internal DistributionType DistributionType { get; private set; }
        internal List<long> RowIndexesToCredit { get; }
        internal List<OrderRowBuilder> NewCreditInvoiceRows { get; }

        public CreditOrderRowsBuilder(IConfigurationProvider config) : base(config)
        {
            RowIndexesToCredit = new List<long>();
            NewCreditInvoiceRows = new List<OrderRowBuilder>();
        }

        public CreditOrderRowsBuilder SetInvoiceId(long invoiceId)
        {
            InvoiceId = invoiceId;
            return this;
        }

        public override CreditOrderRowsBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public CreditOrderRowsBuilder SetInvoiceDistributionType(DistributionType distributionType)
        {
            DistributionType = distributionType;
            return this;
        }

        public CreditOrderRowsBuilder SetRowToCredit( long rowIndexToCredit )
        {
            RowIndexesToCredit.Add(rowIndexToCredit);
            return this;
        }

        public AdminService.CreditOrderRowsRequest CreditInvoiceOrderRows()
        {
            OrderType = PaymentType.INVOICE;
            return new AdminService.CreditOrderRowsRequest(this);
        }

        public CreditOrderRowsBuilder AddNumberedOrderRows(IList<OrderRowBuilder> newCreditInvoiceRows)
        {
            NewCreditInvoiceRows.AddRange(newCreditInvoiceRows);
            return this;
        }
    }
}