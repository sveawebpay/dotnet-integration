using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class ConfirmPartialResponse : CustomerRefNoResponseBase
{
    public ConfirmPartialResponse(XmlDocument response) : base(response)
    {
    }
}