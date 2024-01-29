using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Row;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class ConfirmPartial:BasicRequest
    {
        public readonly long TransactionId;
        public readonly string CallerReferenceId;
        public readonly long Amount;
        public readonly List<NumberedOrderRowBuilder> OrderRows;

        public ConfirmPartial(long transactionId,  string callerReferenceId ,long amount ,List<NumberedOrderRowBuilder> orderRows, string correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            CallerReferenceId = callerReferenceId;
            Amount = amount;
            OrderRows = orderRows;
        }

        public string GetXmlForOrderRows()
        {
            var xml = "";
            foreach (var row in OrderRows)
            {
                if(row.GetQuantity()>0 && row.GetRowNumber()>=0)
                {
                   xml += string.Format(@"
                <orderrow>
                <rowId>{0}</rowId>
                <quantity>{1}</quantity>
                </orderrow>", row.GetRowNumber(), row.GetQuantity());
                }
               
            }
            return xml;
        }
        public static ConfirmPartialResponse Response(XmlDocument responseXml)
        {
            return new ConfirmPartialResponse(responseXml);
        }
    }
}