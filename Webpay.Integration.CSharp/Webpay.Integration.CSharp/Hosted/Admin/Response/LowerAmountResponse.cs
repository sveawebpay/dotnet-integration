using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class LowerAmountResponse : CustomerRefNoResponseBase
    {
        public LowerAmountResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}