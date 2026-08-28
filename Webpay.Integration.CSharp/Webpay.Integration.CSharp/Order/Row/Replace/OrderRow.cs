using System.Globalization;
using System.Xml.Linq;

namespace Webpay.Integration.CSharp.Order.Row.Replace
{
    public class OrderRow
    {
        public string Name { get; set; }
        public long UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal VatPercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public long DiscountAmount { get; set; }
        public string Unit { get; set; }
        public string ArticleNumber { get; set; }
        public string Reference { get; set; }
        public string GetXmlForOrderRow()
        {
            return new XElement("row",
                new XElement("name", Name),
                new XElement("quantity", Quantity.ToString(CultureInfo.InvariantCulture)),
                new XElement("unitprice", UnitPrice),
                new XElement("vatpercent", VatPercent.ToString(CultureInfo.InvariantCulture)),
                new XElement("discountpercent", DiscountPercent.ToString(CultureInfo.InvariantCulture)),
                new XElement("discountamount", DiscountAmount),
                new XElement("unit", Unit),
                new XElement("articlenumber", ArticleNumber),
                new XElement("reference", Reference)
            ).ToString(SaveOptions.DisableFormatting);
        }
    }
}