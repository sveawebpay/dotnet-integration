namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin
{
    public class Query
    {
        public long TransactionId { get; private set; }

        public Query(long transactionId)
        {
            TransactionId = transactionId;
        }
    }
}