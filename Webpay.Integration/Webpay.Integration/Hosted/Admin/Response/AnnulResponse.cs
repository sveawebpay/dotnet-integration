using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class AnnulResponse : CustomerRefNoResponseBase
{
    public AnnulResponse(XmlDocument response) : base(response)
    {
    }
}