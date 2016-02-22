using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class CancelRecurSubscriptionResponse : SpecificHostedAdminResponseBase
    {
        public CancelRecurSubscriptionResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}