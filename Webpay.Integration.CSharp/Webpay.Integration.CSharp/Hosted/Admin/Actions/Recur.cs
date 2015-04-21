using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Recur
    {
        public string CustomerRefNo { get; private set ; }
        public string SubscriptionId { get; private set; }
        public Currency Currency { get; private set; }
        public long Amount { get; private set; }

        public Recur(string customerRefNo, string subscriptionId, Currency currency, long amount)
        {
            CustomerRefNo = customerRefNo;
            SubscriptionId = subscriptionId;
            Currency = currency;
            Amount = amount;
        }
    }
}