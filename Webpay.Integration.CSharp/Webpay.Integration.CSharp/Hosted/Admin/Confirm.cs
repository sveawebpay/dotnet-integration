using System;

namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class Confirm
    {
        public DateTime CaptureDate { get; private set; }
        public int TransactionId { get; private set; }

        public Confirm(int transactionId, DateTime captureDate)
        {
            TransactionId = transactionId;
            CaptureDate = captureDate;
        }
    }
}