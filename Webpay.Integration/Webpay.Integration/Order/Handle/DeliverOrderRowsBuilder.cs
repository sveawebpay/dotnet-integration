using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class DeliverOrderRowsBuilder : Builder<DeliverOrderRowsBuilder>
{
    internal long Id { get; private set; }
    internal PaymentType OrderType { get; set; }
    internal DateTime? CaptureDate { get; private set; }
    internal DistributionType DistributionType { get; private set; }
    internal List<long> RowIndexesToDeliver { get; private set; }
    internal List<NumberedOrderRowBuilder> NumberedOrderRows { get; private set; }
    internal Guid CallerReferenceId { get; private set; }

    public DeliverOrderRowsBuilder(IConfigurationProvider config) : base(config)
    {
        this.CaptureDate = null;
        this.RowIndexesToDeliver = new List<long>();
        this.NumberedOrderRows = new List<NumberedOrderRowBuilder>();
    }

    public DeliverOrderRowsBuilder SetOrderId(long orderId)
    {
        Id = orderId;
        return this;
    }

    public DeliverOrderRowsBuilder SetCallerReferenceId(Guid callerReferenceId)
    {
        CallerReferenceId = callerReferenceId;
        return this;
    }

    public DeliverOrderRowsBuilder SetTransactionId(long orderId)
    {
        return SetOrderId(orderId);
    }

    public override DeliverOrderRowsBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public DeliverOrderRowsBuilder SetInvoiceDistributionType(DistributionType distributionType)
    {
        DistributionType = distributionType;
        return this;
    }

    public DeliverOrderRowsBuilder SetRowToDeliver( long rowIndexToDeliver )
    {
        RowIndexesToDeliver.Add(rowIndexToDeliver);
        return this;
    }

    public DeliverOrderRowsBuilder AddNumberedOrderRows(IList<NumberedOrderRowBuilder> numberedOrderRows)
    {
        NumberedOrderRows.AddRange(numberedOrderRows);
        return this;
    }

    public AdminService.DeliverOrderRowsRequest DeliverInvoiceOrderRows()
    {
        OrderType = PaymentType.INVOICE;
        return new AdminService.DeliverOrderRowsRequest(this);
    }

    public AdminService.ConfirmTransactionRequest DeliverCardOrderRows()
    {
        return new AdminService.ConfirmTransactionRequest(this);
    }

    public override DeliverOrderRowsBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}