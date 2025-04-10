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
    internal List<InvoiceFeeBuilder> InvoiceFeeRows { get; private set; }

    public CreditOrderRowsBuilder(IConfigurationProvider config) : base(config)
    {
        RowIndexesToCredit = new List<long>();
        NewCreditOrderRows = new List<OrderRowBuilder>();
        InvoiceFeeRows = new List<InvoiceFeeBuilder>();
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

    public CreditOrderRowsBuilder AddFee(IRowBuilder fee)
    {
        if (fee is InvoiceFeeBuilder invoiceFee)
        {
            InvoiceFeeRows.Add(invoiceFee);
        }
        else if (fee is ShippingFeeBuilder)
        {
            throw new NotSupportedException("Shipping fee credit is not supported.");
        }
        else
        {
            throw new ArgumentException("Provided fee row must be an InvoiceFeeBuilder", nameof(fee));
        }
    
        return this;
    }
    
    public CreditOrderRowsBuilder AddInvoiceFee(InvoiceFeeBuilder invoiceFee)
    {
        if (invoiceFee == null)
        {
            throw new ArgumentNullException(nameof(invoiceFee));
        }
        InvoiceFeeRows.Add(invoiceFee);
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

    public AdminService.CreditAccountCreditOrderRowsRequest CreditAccountCreditOrderRows()
    {
        OrderType = PaymentType.ACCOUNTCREDIT;
        return new AdminService.CreditAccountCreditOrderRowsRequest(this);
    }

    public override CreditOrderRowsBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}