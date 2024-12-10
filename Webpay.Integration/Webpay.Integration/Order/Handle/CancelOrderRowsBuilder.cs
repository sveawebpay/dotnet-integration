using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class CancelOrderRowsBuilder : Builder<CancelOrderRowsBuilder>
{
    internal long Id { get; private set; }
    internal PaymentType OrderType { get; set; }
    //internal DateTime? CaptureDate { get; private set; }
    internal List<long> RowIndexesToCancel { get; private set; }
    internal List<NumberedOrderRowBuilder> NumberedOrderRows { get; private set; }

    public CancelOrderRowsBuilder(IConfigurationProvider config) : base(config)
    {
        //this.CaptureDate = null;
        this.RowIndexesToCancel = new List<long>();
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

    public CancelOrderRowsBuilder SetRowToCancel(long rowIndexToCancel)
    {
        RowIndexesToCancel.Add(rowIndexToCancel);
        return this;
    }

    public CancelOrderRowsBuilder SetRowsToCancel(IEnumerable<long> rowIndexesToCancel)
    {
        if (rowIndexesToCancel != null)
        {
            RowIndexesToCancel.AddRange(rowIndexesToCancel);
        }
        return this;
    }

    public CancelOrderRowsBuilder AddNumberedOrderRows(IList<NumberedOrderRowBuilder> numberedOrderRows)
    {
        NumberedOrderRows.AddRange(numberedOrderRows);
        return this;
    }

    public AdminService.CancelOrderRowsRequest CancelPaymentTypeOrderRows(PaymentType paymentType)
    {
        OrderType = paymentType;
        return new AdminService.CancelOrderRowsRequest(this);
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

    public AdminService.CancelOrderRowsRequest CancelAccountCreditOrderRows()
    {
        OrderType = PaymentType.ACCOUNTCREDIT;
        return new AdminService.CancelOrderRowsRequest(this);
    }

    public AdminService.LowerTransactionRequest CancelCardOrderRows()
    {
        return new AdminService.LowerTransactionRequest(this);
    }

    public override CancelOrderRowsBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}