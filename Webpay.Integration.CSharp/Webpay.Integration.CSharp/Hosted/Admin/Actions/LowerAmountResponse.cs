using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class LowerAmountResponse : CustomerRefNoResponseBase
    {
        public LowerAmountResponse(XmlDocument response) : base(response)
        {
        }
    }
}