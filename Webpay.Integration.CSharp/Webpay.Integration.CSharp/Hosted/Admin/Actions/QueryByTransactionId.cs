namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class QueryByTransactionId
    {
        public readonly long TransactionId;

        public QueryByTransactionId(long transactionId)
        {
            TransactionId = transactionId;
        }
    }
}