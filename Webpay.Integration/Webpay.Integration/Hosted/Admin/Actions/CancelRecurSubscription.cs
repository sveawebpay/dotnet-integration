using System.Xml;
using Webpay.Integration.Hosted.Admin.Response;

namespace Webpay.Integration.Hosted.Admin.Actions;

public class CancelRecurSubscription:BasicRequest
{
    public readonly string SubscriptionId;

    public CancelRecurSubscription(string subscriptionId, Guid? correlationId) : base(correlationId)
    {
        SubscriptionId = subscriptionId;
    }

    public static CancelRecurSubscriptionResponse Response(XmlDocument responseXml)
    {
        return new CancelRecurSubscriptionResponse(responseXml);
    }
}