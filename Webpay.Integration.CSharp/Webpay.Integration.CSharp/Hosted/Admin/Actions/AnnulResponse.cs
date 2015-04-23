using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class AnnulResponse : CustomerRefNoResponseBase
    {
        public AnnulResponse(XmlDocument response) : base(response)
        {
        }
    }
}