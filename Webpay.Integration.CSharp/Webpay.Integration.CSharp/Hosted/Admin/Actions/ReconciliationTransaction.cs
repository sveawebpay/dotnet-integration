using System;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class ReconciliationTransaction
    {
        public readonly decimal Amount;
        public readonly string Currency;
        public readonly string CustomerRefNo;
        public readonly string PaymentMethod;
        public readonly DateTime Time;
        public readonly int TransactionId;

        public ReconciliationTransaction(int transactionId, string customerRefNo, string paymentMethod, decimal amount,
            string currency, DateTime time)
        {
            TransactionId = transactionId;
            CustomerRefNo = customerRefNo;
            PaymentMethod = paymentMethod;
            Amount = amount;
            Currency = currency;
            Time = time;
        }
    }
}