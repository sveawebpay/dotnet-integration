namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class LowerAmount
    {
        public readonly long TransactionId;
        public readonly long AmountToLower;

        public LowerAmount(long transactionId, long amountToLower)
        {
            TransactionId = transactionId;
            AmountToLower = amountToLower;
        }
    }
}