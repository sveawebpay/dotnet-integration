namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class QueryByTransactionId
    {
        public long TransactionId { get; private set; }

        public QueryByTransactionId(long transactionId)
        {
            TransactionId = transactionId;
        }
    }
}