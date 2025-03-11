using System;
using System.Collections.Generic;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Row.LowerAmount;
namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class LowerOrderRowConfirm : BasicRequest
    {
        public readonly List<OrderRow> OrderRows;
        public readonly long TransactionId;
        public readonly DateTime CaptureDate;

        public LowerOrderRowConfirm(long transactionId, List<OrderRow> orderRows, DateTime captureDate, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            OrderRows = orderRows;
            CaptureDate = captureDate;
        }

        public static LowerOrderRowConfirmResponse Response(XmlDocument responseXml)
        {
            return new LowerOrderRowConfirmResponse(responseXml);
        }
        public string GetXmlForOrderRows()
        {
            var xml = "";
            OrderRows.ForEach(orderRow => { xml += orderRow.GetXmlForOrderRow(); });
            return xml;
        }
    }
}