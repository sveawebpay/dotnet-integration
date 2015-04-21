namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class LowerAmount
    {
        public long TransactionId { get; private set; }
        public long AmountToLower { get; private set; }

        public LowerAmount(long transactionId, long amountToLower)
        {
            TransactionId = transactionId;
            AmountToLower = amountToLower;
        }
    }
}