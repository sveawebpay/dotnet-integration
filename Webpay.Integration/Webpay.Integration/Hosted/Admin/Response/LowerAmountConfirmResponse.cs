using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class LowerAmountConfirmResponse: LowerAmountResponse
{
    public LowerAmountConfirmResponse(XmlDocument response) : base(response)
    {
    }
}
