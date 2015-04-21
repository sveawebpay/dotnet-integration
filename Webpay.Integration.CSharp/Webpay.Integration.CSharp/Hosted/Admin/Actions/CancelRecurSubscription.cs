namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class CancelRecurSubscription
    {
        public string SubscriptionId { get; private set; }

        public CancelRecurSubscription(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}