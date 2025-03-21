using System.Xml;
using Webpay.Integration.Hosted.Admin.Response;
using Webpay.Integration.Order.Row;

namespace Webpay.Integration.Hosted.Admin.Actions;

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