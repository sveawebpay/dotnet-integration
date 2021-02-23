using System;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class LowerAmountConfirm
    {
        public readonly long AmountToLower;
        public readonly long TransactionId;
        public readonly DateTime CaptureDate;

        public LowerAmountConfirm(long transactionId, long amountToLower, DateTime captureDate)
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
}