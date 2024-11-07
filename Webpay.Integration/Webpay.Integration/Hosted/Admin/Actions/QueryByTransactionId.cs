namespace Webpay.Integration.Hosted.Admin.Actions;

public class QueryByTransactionId : Query
{
    public readonly long TransactionId;
    public readonly Guid? CorrelationId;

    public QueryByTransactionId(long transactionId, Guid? correlationId)
    {
        TransactionId = transactionId;
        CorrelationId = correlationId;
    }
}