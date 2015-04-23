using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class CancelRecurSubscriptionResponse : SpecificHostedAdminResponseBase
    {
        public CancelRecurSubscriptionResponse(XmlDocument response) : base(response)
        {
        }
    }
}