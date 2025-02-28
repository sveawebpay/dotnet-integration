namespace Webpay.Integration.Order.Row.LowerAmount;

public class OrderRow
{
    public int RowId { get; set; }
    public decimal? Quantity { get; set; }

    public string GetXmlForOrderRow()
    {
        return $"<orderrow>" +
                $"<rowid>{RowId}</rowid>" +
                $"<quantity>{Quantity}</quantity>" +
                $"</orderrow>";
    }
}
