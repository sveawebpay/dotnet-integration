using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway
{
    public class AddOrderRowResponse : SpecificHostedAdminResponseBase
    {
        public AddOrderRowResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}