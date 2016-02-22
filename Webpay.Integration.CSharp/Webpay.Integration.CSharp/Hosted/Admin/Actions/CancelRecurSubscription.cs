using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class CancelRecurSubscription
    {
        public readonly string SubscriptionId;

        public CancelRecurSubscription(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        public static CancelRecurSubscriptionResponse Response(XmlDocument responseXml)
        {
            return new CancelRecurSubscriptionResponse(responseXml);
        }
    }
}