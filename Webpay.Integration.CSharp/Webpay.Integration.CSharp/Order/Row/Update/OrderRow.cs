using System.Globalization;
using System.Xml.Linq;

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
            return new XElement("orderrow",
                new XElement("rowid", RowId),
                new XElement("name", Name),
                new XElement("quantity", Quantity.ToString(CultureInfo.InvariantCulture)),
                new XElement("unitprice", UnitPrice),
                new XElement("vatpercent", VatPercent.ToString(CultureInfo.InvariantCulture)),
                new XElement("discountpercent", DiscountPercent.ToString(CultureInfo.InvariantCulture)),
                new XElement("discountamount", DiscountAmount),
                new XElement("unit", Unit),
                new XElement("articlenumber", ArticleNumber)
            ).ToString(SaveOptions.DisableFormatting);
        }
    }
}