namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin
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