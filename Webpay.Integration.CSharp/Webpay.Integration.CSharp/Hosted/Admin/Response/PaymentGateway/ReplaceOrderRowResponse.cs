using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway
{

    public class ReplaceOrderRowResponse : SpecificHostedAdminResponseBase
    {
        public long? TransactionId { get; private set; }
        public string CustomerRefNo { get; private set; }
        public IList<ResponseOrderRow> OrderRows { get; private set; }

        public ReplaceOrderRowResponse(XmlNode response)
            : base(response)
        {
            OrderRows = new List<ResponseOrderRow>();

            if (!Accepted)
                return;

            TransactionId = AttributeLong(response, "/response/transaction", "id");
            CustomerRefNo = TextString(response, "/response/transaction/customerrefno");

            var rowNodes = response.SelectNodes("/response/transaction/orderrows/row");
            if (rowNodes != null)
            {
                foreach (XmlNode row in rowNodes)
                {
                    OrderRows.Add(new ResponseOrderRow
                    {
                        Id = TextInt(row, "id"),
                        RowId = TextInt(row, "rowid"),
                        Name = TextString(row, "name"),
                        Amount = TextDecimal(row, "amount"),
                        Vat = TextDecimal(row, "vat"),
                        Description = TextString(row, "description"),
                        Quantity = TextDecimal(row, "quantity"),
                        Sku = TextString(row, "sku"),
                        Unit = TextString(row, "unit")
                    });
                }
            }
        }
    }
    public class ResponseOrderRow
    {
        public int? Id { get; set; }
        public int? RowId { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Vat { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public string Sku { get; set; }
        public string Unit { get; set; }
    }
}