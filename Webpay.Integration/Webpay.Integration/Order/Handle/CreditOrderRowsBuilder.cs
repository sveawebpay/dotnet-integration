using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class CreditOrderRowsBuilder : Builder<CreditOrderRowsBuilder>
{
    internal long Id { get; private set; }
    internal PaymentType OrderType { get; set; }
    internal DistributionType DistributionType { get; private set; }
    internal List<long> RowIndexesToCredit { get; private set; }
    internal List<OrderRowBuilder> NewCreditOrderRows { get; private set; }

    public CreditOrderRowsBuilder(IConfigurationProvider config) : base(config)
    {
        RowIndexesToCredit = new List<long>();
        NewCreditOrderRows = new List<OrderRowBuilder>();
    }

    public CreditOrderRowsBuilder SetInvoiceId(long invoiceId)
    {
        Id = invoiceId;
        return this;
    }

    public CreditOrderRowsBuilder SetContractNumber(long contractNumber)
    {
        Id = contractNumber;
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

    public CreditOrderRowsBuilder SetRowToCredit(long rowIndexToCredit)
    {
        RowIndexesToCredit.Add(rowIndexToCredit);
        return this;
    }

    public CreditOrderRowsBuilder SetRowsToCredit(IEnumerable<long> rowIndexesToCredit)
    {
        if (rowIndexesToCredit != null)
        {
            RowIndexesToCredit.AddRange(rowIndexesToCredit);
        }

        return this;
    }

    public CreditOrderRowsBuilder AddCreditOrderRows(IList<OrderRowBuilder> newCreditOrderRows)
    {
        NewCreditOrderRows.AddRange(newCreditOrderRows);
        return this;
    }

    public AdminService.CreditInvoiceOrderRowsRequest CreditInvoiceOrderRows()
    {
        OrderType = PaymentType.INVOICE;
        return new AdminService.CreditInvoiceOrderRowsRequest(this);
    }

    public AdminService.CreditPaymentPlanOrderRowsRequest CreditPaymentPlanOrderRows()
    {
        OrderType = PaymentType.PAYMENTPLAN;
        return new AdminService.CreditPaymentPlanOrderRowsRequest(this);
    }

    public override CreditOrderRowsBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}