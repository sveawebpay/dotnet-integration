using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class CreditResponse : CustomerRefNoResponseBase
{
    public CreditResponse(XmlDocument response) : base(response)
    {
    }
}