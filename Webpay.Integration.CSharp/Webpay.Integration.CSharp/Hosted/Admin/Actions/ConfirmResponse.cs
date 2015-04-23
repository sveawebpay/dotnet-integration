using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class ConfirmResponse : CustomerRefNoResponseBase
    {
        public ConfirmResponse(XmlDocument response)
            : base(response)
        {
        }
    }
}