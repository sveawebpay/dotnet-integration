using System.Globalization;

namespace Webpay.Integration.CSharp.Order.Row.Add
{
    public class OrderRow
    {
        public string Name { get; set; }
        public long UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal VatPercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public string Unit { get; set; }
        public string ArticleNumber { get; set; }
        public string GetXmlForOrderRow()
        {
            return $"<row>" +
                    $"<name>{Name}</name>" +
                    $"<quantity>{Quantity.ToString(CultureInfo.InvariantCulture)}</quantity>" +
                    $"<unitprice>{UnitPrice}</unitprice>" +
                    $"<vatpercent>{VatPercent}</vatpercent>" +
                    $"<discountpercent>{DiscountPercent}</discountpercent>" +
                    $"<unit>{Unit}</unit>" +
                    $"<articlenumber>{ArticleNumber}</articlenumber>"+
                    $"</row>";
        }
    }
}