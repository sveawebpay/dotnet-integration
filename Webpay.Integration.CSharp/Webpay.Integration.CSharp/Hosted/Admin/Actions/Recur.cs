using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Recur
    {
        public readonly long Amount;
        public readonly Currency Currency;
        public readonly string CustomerRefNo;
        public readonly string SubscriptionId;

        public Recur(string customerRefNo, string subscriptionId, Currency currency, long amount)
        {
            CustomerRefNo = customerRefNo;
            SubscriptionId = subscriptionId;
            Currency = currency;
            Amount = amount;
        }
    }
}