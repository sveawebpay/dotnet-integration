using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webpay.Integration.CSharp.Hosted.Admin.Response;

namespace Webpay.Integration.CSharp.Order.Row.LowerAmount
{
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
}
