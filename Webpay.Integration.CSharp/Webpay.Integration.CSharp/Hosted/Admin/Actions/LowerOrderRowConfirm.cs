using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Row.LowerAmount;
namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class LowerOrderRowConfirm : BasicRequest
    {
        public readonly List<OrderRow> OrderRows;
        public readonly long TransactionId;
        public readonly string CaptureRequestId;
        public LowerOrderRowConfirm(long transactionId, List<OrderRow> orderRows, string captureRequestId, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            OrderRows = orderRows;
            CaptureRequestId = captureRequestId;
        }

        public static LowerOrderRowConfirmResponse Response(XmlDocument responseXml)
        {
            return new LowerOrderRowConfirmResponse(responseXml);
        }
        public string GetXmlForOrderRows()
        {
            if (OrderRows == null) return "";
            var elements = OrderRows.Select(orderRow => XElement.Parse(orderRow.GetXmlForOrderRow()));
            return string.Concat(elements.Select(e => e.ToString(SaveOptions.DisableFormatting)));
        }
    }
}