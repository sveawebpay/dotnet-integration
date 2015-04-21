namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class Annul
    {
        public long TransactionId { get; private set; }

        public Annul(long transactionId)
        {
            TransactionId = transactionId;
        }
    }
}