using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class ConfirmResponse : CustomerRefNoResponseBase
{
    public ConfirmResponse(XmlDocument response) : base(response)
    {
    }
}