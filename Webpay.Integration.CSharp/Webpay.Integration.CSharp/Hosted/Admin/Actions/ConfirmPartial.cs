using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Row;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class ConfirmPartial:BasicRequest
    {
        public readonly long TransactionId;
        public readonly Guid CallerReferenceId;
        public readonly long? Amount;
        public readonly List<NumberedOrderRowBuilder> OrderRows;

        public ConfirmPartial(long transactionId, Guid callerReferenceId ,long? amount ,List<NumberedOrderRowBuilder> orderRows, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            CallerReferenceId = callerReferenceId;
            Amount = amount;
            OrderRows = orderRows;
        }

        public string GetXmlForOrderRows()
        {
            var elements = OrderRows?
                .Where(row => row.GetQuantity() > -1 && row.GetRowNumber() >= 0)
                .Select(row => new XElement("orderrow",
                    new XElement("rowId", row.GetRowNumber()),
                    new XElement("quantity", row.GetQuantity())
                ));
            return string.Concat(elements?.Select(e => e.ToString(SaveOptions.DisableFormatting)) ?? Enumerable.Empty<string>());
        }
        public static ConfirmPartialResponse Response(XmlDocument responseXml)
        {
            return new ConfirmPartialResponse(responseXml);
        }
    }
}