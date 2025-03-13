using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class LowerOrderRowConfirmResponse : CustomerRefNoResponseBase
    {
        public LowerOrderRowConfirmResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}