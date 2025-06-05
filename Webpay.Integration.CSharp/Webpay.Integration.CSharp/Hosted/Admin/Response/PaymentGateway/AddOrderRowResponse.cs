using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway
{
    public class AddOrderRowResponse : CustomerRefNoResponseBase
    {
        public AddOrderRowResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}