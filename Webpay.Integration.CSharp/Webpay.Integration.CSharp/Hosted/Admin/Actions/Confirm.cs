using System;
using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Confirm
    {
        public readonly DateTime CaptureDate;
        public readonly int TransactionId;

        public Confirm(int transactionId, DateTime captureDate)
        {
            TransactionId = transactionId;
            CaptureDate = captureDate;
        }

        public static ConfirmResponse Response(XmlDocument responseXml)
        {
            return new ConfirmResponse(responseXml);
        }
    }
}