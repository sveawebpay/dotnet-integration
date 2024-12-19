using System.Xml;
using Webpay.Integration.Hosted.Admin.Response;

namespace Webpay.Integration.Hosted.Admin.Actions;

public class Confirm:BasicRequest
{
    public readonly DateTime CaptureDate;
    public readonly long TransactionId;
    
    public Confirm(long transactionId, DateTime captureDate, Guid? correlationId):base(correlationId) 
    {
        TransactionId = transactionId;
        CaptureDate = captureDate;
    }

    public static ConfirmResponse Response(XmlDocument responseXml)
    {
        return new ConfirmResponse(responseXml);
    }
}