using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Row.LowerAmount;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class LowerOrderRow : BasicRequest
    {
        public readonly long TransactionId;
        public readonly List<OrderRow> OrderRows;
        public LowerOrderRow(long transactionId, List<OrderRow> orderRows, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            OrderRows = orderRows;
        }

        public static LowerOrderRowResponse Response(XmlDocument responseXml)
        {
            return new LowerOrderRowResponse(responseXml);
        }
        public string GetXmlForOrderRows()
        {
            var xml = "";
            OrderRows.ForEach(orderRow => { xml += orderRow.GetXmlForOrderRow(); });
            return xml;
        }
    }
}