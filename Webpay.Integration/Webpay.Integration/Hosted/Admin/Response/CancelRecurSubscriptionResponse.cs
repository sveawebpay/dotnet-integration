using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class CancelRecurSubscriptionResponse : SpecificHostedAdminResponseBase
{
    public CancelRecurSubscriptionResponse(XmlDocument response) : base(response)
    {
    }
}