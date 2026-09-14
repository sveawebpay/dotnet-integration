using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Order.Row.Add;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway
{
    public class AddOrderRow : BasicRequest
    {
        public readonly long TransactionId;
        public readonly List<OrderRow> OrderRows;
        public AddOrderRow(long transactionId, List<OrderRow> orderRows, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            OrderRows = orderRows;
        }
        public string GetXmlForOrderRows()
        {
            if (OrderRows == null) return "";
            var elements = OrderRows.Select(orderRow => XElement.Parse(orderRow.GetXmlForOrderRow()));
            return string.Concat(elements.Select(e => e.ToString(SaveOptions.DisableFormatting)));
        }
        public static AddOrderRowResponse Response(XmlDocument response)
        {
            return new AddOrderRowResponse(response);
        }
    }
}