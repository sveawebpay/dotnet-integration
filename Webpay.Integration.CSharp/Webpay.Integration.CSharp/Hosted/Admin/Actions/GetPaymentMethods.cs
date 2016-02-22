using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class GetPaymentMethods
    {
        public readonly int MerchantId;

        public GetPaymentMethods(int merchantId)
        {
            MerchantId = merchantId;
        }

        public static GetPaymentMethodsResponse Response(XmlDocument responseXml)
        {
            return new GetPaymentMethodsResponse(responseXml);
        }
    }
}