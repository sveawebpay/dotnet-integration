using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class LowerAmountResponse : CustomerRefNoResponseBase
{
    public LowerAmountResponse(XmlDocument response) : base(response)
    {
    }
}