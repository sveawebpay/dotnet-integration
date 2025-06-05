using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway
{
    public class UpdateOrderRowResponse : CustomerRefNoResponseBase
    {
        public UpdateOrderRowResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}