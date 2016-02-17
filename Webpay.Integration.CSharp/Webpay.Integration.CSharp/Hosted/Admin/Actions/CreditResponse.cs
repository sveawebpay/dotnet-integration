using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class CreditResponse : CustomerRefNoResponseBase
    {
        public CreditResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}