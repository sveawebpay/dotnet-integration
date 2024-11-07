using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;

namespace Webpay.Integration.Order;

public abstract class OrderBuilder<T> : Builder<T>
{
    protected List<OrderRowBuilder> OrderRows = new List<OrderRowBuilder>();
    protected List<InvoiceFeeBuilder> InvoiceFeeRows = new List<InvoiceFeeBuilder>();
    protected List<ShippingFeeBuilder> ShippingFeeRows = new List<ShippingFeeBuilder>();
    protected List<FixedDiscountBuilder> FixedDiscountRows = new List<FixedDiscountBuilder>();
    protected List<RelativeDiscountBuilder> RelativeDiscountRows = new List<RelativeDiscountBuilder>();

    protected OrderBuilder(IConfigurationProvider config) : base(config)
    {
    }

    public List<OrderRowBuilder> GetOrderRows()
    {
        return OrderRows;
    }

    public List<InvoiceFeeBuilder> GetInvoiceFeeRows()
    {
        return InvoiceFeeRows;
    }

    public void SetInvoiceFeeRows(List<InvoiceFeeBuilder> invoiceFeeRows)
    {
        InvoiceFeeRows = invoiceFeeRows;
    }

    public List<ShippingFeeBuilder> GetShippingFeeRows()
    {
        return ShippingFeeRows;
    }

    public void SetShippingFeeRows(List<ShippingFeeBuilder> shippingFeeRows)
    {
        ShippingFeeRows = shippingFeeRows;
    }

    public List<FixedDiscountBuilder> GetFixedDiscountRows()
    {
        return FixedDiscountRows;
    }

    public List<RelativeDiscountBuilder> GetRelativeDiscountRows()
    {
        return RelativeDiscountRows;
    }

    public abstract T SetFixedDiscountRows(List<FixedDiscountBuilder> fixedDiscountRows);

    public abstract T SetRelativeDiscountRows(List<RelativeDiscountBuilder> relativeDiscountRows);

    public abstract T AddOrderRow(OrderRowBuilder itemOrderRow);

    public abstract T AddOrderRows(IEnumerable<OrderRowBuilder> itemOrderRow);

    public abstract T AddDiscount(IRowBuilder itemDiscount);

    public abstract T AddFee(IRowBuilder itemFee);
}