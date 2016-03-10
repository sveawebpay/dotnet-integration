using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class ConfirmResponse : CustomerRefNoResponseBase
    {
        public ConfirmResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}