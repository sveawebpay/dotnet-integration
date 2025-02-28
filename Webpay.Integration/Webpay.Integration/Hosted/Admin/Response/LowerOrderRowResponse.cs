using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class LowerOrderRowResponse : CustomerRefNoResponseBase
{
    public LowerOrderRowResponse(XmlDocument response) : base(response)
    {
        //intentionally left blank
    }
}
