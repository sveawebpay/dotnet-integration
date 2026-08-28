using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Order.Row.Update;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway
{
    public class UpdateOrderRow : BasicRequest
    {
        public readonly long TransactionId;
        public readonly string Reference;
        public readonly List<OrderRow> OrderRows;
        public UpdateOrderRow(long transactionId, List<OrderRow> orderRows,string reference, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            OrderRows = orderRows;
            Reference = reference;
        }
        public string GetXmlForOrderRows()
        {
            if (OrderRows == null) return "";
            var elements = OrderRows.Select(orderRow => XElement.Parse(orderRow.GetXmlForOrderRow()));
            return string.Concat(elements.Select(e => e.ToString(SaveOptions.DisableFormatting)));
        }
        public static UpdateOrderRowResponse Response(XmlDocument response)
        {
            return new UpdateOrderRowResponse(response);
        }
    }
}