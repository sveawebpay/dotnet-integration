using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Webpay.Integration.CSharp.Order.Row.LowerAmount
{
    public class OrderRow
    {
        public int RowId { get; set; }
        public decimal? Quantity { get; set; }

        public string GetXmlForOrderRow()
        {
            return new XElement("orderrow",
                new XElement("rowid", RowId),
                new XElement("quantity", Quantity?.ToString(CultureInfo.InvariantCulture))
            ).ToString(SaveOptions.DisableFormatting);
        }
    }
}
