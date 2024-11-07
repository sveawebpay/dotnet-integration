using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Row;

public class NumberedOrderRowBuilder : OrderRowBuilder, IPriced<NumberedOrderRowBuilder>
{
    private long? _creditInvoiceId;
    private long? _invoiceId;
    private int _rowNumber;
    private OrderRowStatus? _status;

    public NumberedOrderRowBuilder() : base()
    {
    }

    public long? GetCreditInvoiceId()
    {
        return _creditInvoiceId;

    }
    public NumberedOrderRowBuilder SetCreditInvoiceId(long? creditInvoiceId)
    {
        this._creditInvoiceId = creditInvoiceId;
        return this;
    }
    public long? GetInvoiceId()
    {
        return _invoiceId;
    }
    public NumberedOrderRowBuilder SetInvoiceId(long? invoiceId)
    {
        this._invoiceId = invoiceId;
        return this;
    }
    public int GetRowNumber()
    {
        return _rowNumber;
    }
    public NumberedOrderRowBuilder SetRowNumber(int rowNumber)
    {
        this._rowNumber = rowNumber;
        return this;
    }      
    public OrderRowStatus? GetStatus()
    {
        return _status;
    }
    public NumberedOrderRowBuilder SetStatus(OrderRowStatus? status)
    {
        this._status = status;
        return this;
    }
    // shadow OrderRowBuilder setter methods to ensure correct return type

    /// <summary>
    /// Optional
    /// </summary>
    /// <param name="articleNumber"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetArticleNumber(string articleNumber)
    {
        base.SetArticleNumber(articleNumber);
        return this;
    }

    /// <summary>
    /// Optional
    /// </summary>
    /// <param name="name"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetName(string name)
    {
        base.SetName(name);
        return this;
    }

    /// <summary>
    /// Optional
    /// </summary>
    /// <param name="description"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetDescription(string description)
    {
        base.SetDescription(description);
        return this;
    }

    /// <summary>
    /// Optional
    /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
    /// </summary>
    /// <param name="dExVatAmount"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetAmountExVat(decimal dExVatAmount)
    {
        base.SetAmountExVat(dExVatAmount);
        return this;
    }

    /// <summary>
    /// Optional
    /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
    /// </summary>
    /// <param name="vatPercent"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetVatPercent(decimal vatPercent)
    {
        base.SetVatPercent(vatPercent);
        return this;
    }
    
    /// <summary>
    /// Required
    /// </summary>
    /// <param name="quantity"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetQuantity(decimal quantity)
    {
        base.SetQuantity(quantity);
        return this;
    }

    /// <summary>
    /// SetUnit
    /// </summary>
    /// <param name="unit">i.e. "pcs", "st" etc</param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetUnit(string unit)
    {
        base.SetUnit(unit);
        return this;
    }
    
    /// <summary>
    /// Optional
    /// </summary>
    /// <param name="vatDiscount"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetVatDiscount(int vatDiscount)
    {
        base.SetVatDiscount(vatDiscount);
        return this;
    }

    /// <summary>
    /// Optional
    /// </summary>
    /// <param name="discountPercent"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetDiscountPercent(int discountPercent)
    {
        base.SetDiscountPercent(discountPercent);
        return this;
    }

    /// <summary>
    /// Optional
    /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
    /// </summary>
    /// <param name="amountIncVat"></param>
    /// <returns>NumberedOrderRowBuilder</returns>
    public new NumberedOrderRowBuilder SetAmountIncVat(decimal amountIncVat)
    {
        base.SetAmountIncVat(amountIncVat);
        return this;
    }
}