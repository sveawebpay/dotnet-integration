using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway
{
    public class UpdateMetadataResponse : SpecificHostedAdminResponseBase
    {
        public UpdateMetadataResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}