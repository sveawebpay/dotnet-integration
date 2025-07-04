using System.Globalization;

namespace Webpay.Integration.CSharp.Order.Row.Update
{
    public class OrderRow
    {
        public int RowId { get; set; }
        public string Name { get; set; }
        public long UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal VatPercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public long DiscountAmount { get; set; }
        public string Unit { get; set; }
        public string ArticleNumber { get; set; }
        public string GetXmlForOrderRow()
        {
            return $"<orderrow>" +
                    $"<rowid>{RowId}</rowid>" +
                    $"<name>{Name}</name>" +
                    $"<quantity>{Quantity.ToString(CultureInfo.InvariantCulture)}</quantity>" +
                    $"<unitprice>{UnitPrice}</unitprice>" +
                    $"<vatpercent>{VatPercent.ToString(CultureInfo.InvariantCulture)}</vatpercent>" +
                    $"<discountpercent>{DiscountPercent.ToString(CultureInfo.InvariantCulture)}</discountpercent>" +
                    $"<discountamount>{DiscountAmount}</discountamount>" +
                    $"<unit>{Unit}</unit>" +
                    $"<articlenumber>{ArticleNumber}</articlenumber>"+
                    $"</orderrow>";
        }
    }
}