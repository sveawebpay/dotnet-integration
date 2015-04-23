namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class LowerAmount
    {
        public readonly long AmountToLower;
        public readonly long TransactionId;

        public LowerAmount(long transactionId, long amountToLower)
        {
            TransactionId = transactionId;
            AmountToLower = amountToLower;
        }
    }
}