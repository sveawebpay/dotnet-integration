using System.Xml;
using Webpay.Integration.Hosted.Admin.Response;

namespace Webpay.Integration.Hosted.Admin.Actions;

public class LowerAmountConfirm:BasicRequest
{
    public readonly long AmountToLower;
    public readonly long TransactionId;
    public readonly DateTime CaptureDate;

    public LowerAmountConfirm(long transactionId, long amountToLower, DateTime captureDate, Guid? correlationId) : base(correlationId)
    {
        TransactionId = transactionId;
        AmountToLower = amountToLower;
        CaptureDate = captureDate;
    }

    public static LowerAmountConfirmResponse Response(XmlDocument responseXml)
    {
        return new LowerAmountConfirmResponse(responseXml);
    }
}