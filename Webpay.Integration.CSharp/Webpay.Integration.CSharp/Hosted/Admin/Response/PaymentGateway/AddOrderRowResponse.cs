using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway
{
    public class EditOrderRowResponse : CustomerRefNoResponseBase
    {
        public EditOrderRowResponse(XmlDocument response) : base(response)
        {
            //intentionally left blank
        }
    }
}