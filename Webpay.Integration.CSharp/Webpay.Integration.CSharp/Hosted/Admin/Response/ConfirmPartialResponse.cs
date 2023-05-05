using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class ConfirmPartialResponse : CustomerRefNoResponseBase
    {
        public ConfirmPartialResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}