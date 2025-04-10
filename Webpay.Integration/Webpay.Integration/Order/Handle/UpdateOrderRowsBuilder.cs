using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class UpdateOrderRowsBuilder : Builder<UpdateOrderRowsBuilder>
{
    internal long Id { get; private set; }
    internal PaymentType OrderType { get; set; }
    internal List<NumberedOrderRowBuilder> NumberedOrderRows { get; private set; }
    internal List<InvoiceFeeBuilder> InvoiceFeeRows { get; private set; }

    public UpdateOrderRowsBuilder(IConfigurationProvider config) : base(config)
    {
        NumberedOrderRows = new List<NumberedOrderRowBuilder>();
        InvoiceFeeRows = new List<InvoiceFeeBuilder>();
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

    public UpdateOrderRowsBuilder AddUpdateOrderRows(IList<NumberedOrderRowBuilder> numberedOrderRows)
    {
        NumberedOrderRows.AddRange(numberedOrderRows);
        return this;
    }

    public UpdateOrderRowsBuilder AddInvoiceFee(InvoiceFeeBuilder invoiceFee)
    {
        if (invoiceFee == null)
        {
            throw new ArgumentNullException(nameof(invoiceFee));
        }
        InvoiceFeeRows.Add(invoiceFee);
        return this;
    }
    
    public UpdateOrderRowsBuilder AddFee(IRowBuilder fee)
    {
        if (fee is InvoiceFeeBuilder invoiceFee)
        {
            InvoiceFeeRows.Add(invoiceFee);
        }
        else if (fee is ShippingFeeBuilder)
        {
            throw new NotSupportedException("Shipping fee update is not supported.");
        }
        else
        {
            throw new ArgumentException("Provided fee row must be an InvoiceFeeBuilder", nameof(fee));
        }
        return this;
    }

    public AdminService.UpdateOrderRowsRequest UpdateOrderRowsByPaymentType(PaymentType paymentType)
    {
        OrderType = paymentType;
        return new AdminService.UpdateOrderRowsRequest(this);
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

    public AdminService.UpdateOrderRowsRequest UpdateAccountCreditOrderRows()
    {
        OrderType = PaymentType.ACCOUNTCREDIT;
        return new AdminService.UpdateOrderRowsRequest(this);
    }

    public override UpdateOrderRowsBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}