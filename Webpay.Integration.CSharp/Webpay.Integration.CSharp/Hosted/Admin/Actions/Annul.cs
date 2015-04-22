namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Annul
    {
        public readonly long TransactionId;

        public Annul(long transactionId)
        {
            TransactionId = transactionId;
        }
    }
}