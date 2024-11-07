using System.Xml;
using Webpay.Integration.Hosted.Admin.Response;

namespace Webpay.Integration.Hosted.Admin.Actions;

public class GetPaymentMethods:BasicRequest
{
    public readonly int MerchantId;

    public GetPaymentMethods(int merchantId, Guid? correlationId):base(correlationId)
    {
        MerchantId = merchantId;
    }

    public static GetPaymentMethodsResponse Response(XmlDocument responseXml)
    {
        return new GetPaymentMethodsResponse(responseXml);
    }
}