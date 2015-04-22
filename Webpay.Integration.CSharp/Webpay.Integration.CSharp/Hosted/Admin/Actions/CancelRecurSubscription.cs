namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class CancelRecurSubscription
    {
        public readonly string SubscriptionId;

        public CancelRecurSubscription(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}