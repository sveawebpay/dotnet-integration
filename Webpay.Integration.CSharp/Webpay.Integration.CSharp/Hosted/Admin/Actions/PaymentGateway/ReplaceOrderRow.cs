using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Order.Row.Replace;
using OrderRow = Webpay.Integration.CSharp.Order.Row.Replace.OrderRow;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway
{
    public class ReplaceOrderRow : BasicRequest
    {
        public readonly long TransactionId;
        public readonly List<OrderRow> OrderRows;
        public ReplaceOrderRow(long transactionId, List<OrderRow> orderRows, Guid? correlationId) : base(correlationId)
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
        public static ReplaceOrderRowResponse Response(XmlDocument response)
        {
            return new ReplaceOrderRowResponse(response);
        }
    }
}