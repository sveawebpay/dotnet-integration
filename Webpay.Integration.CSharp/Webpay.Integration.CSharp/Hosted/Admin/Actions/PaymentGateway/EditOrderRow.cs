using System;
using System.Collections.Generic;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Order.Row.Edit;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway
{
    public class EditOrderRow : BasicRequest
    {
        public readonly long TransactionId;
        public readonly List<OrderRow> OrderRows;
        public EditOrderRow(long transactionId, List<OrderRow> orderRows, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            OrderRows = orderRows;
        }
        public string GetXmlForOrderRows()
        {
            var xml = "";
            OrderRows.ForEach(orderRow => { xml += orderRow.GetXmlForOrderRow(); });
            return xml;
        }
        public static EditOrderRowResponse Response(XmlDocument response)
        {
            return new EditOrderRowResponse(response);
        }
    }
}