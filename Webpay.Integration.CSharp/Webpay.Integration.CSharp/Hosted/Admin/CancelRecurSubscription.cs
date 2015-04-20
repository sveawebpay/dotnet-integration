namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin
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