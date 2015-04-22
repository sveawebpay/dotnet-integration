using System.Xml;

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

    public class CancelRecurSubscriptionResponse : SpecificHostedAdminResponseBase
    {
        public CancelRecurSubscriptionResponse(XmlDocument response) : base(response)
        {
        }
    }
}