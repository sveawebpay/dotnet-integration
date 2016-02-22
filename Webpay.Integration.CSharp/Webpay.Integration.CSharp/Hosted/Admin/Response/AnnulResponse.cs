using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class AnnulResponse : CustomerRefNoResponseBase
    {
        public AnnulResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}