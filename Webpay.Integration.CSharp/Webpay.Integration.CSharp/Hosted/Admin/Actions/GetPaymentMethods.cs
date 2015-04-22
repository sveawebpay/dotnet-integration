using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Webpay.Integration.CSharp.Util.Constant;

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

    public class GetPaymentMethodsResponse : SpecificHostedAdminResponseBase
    {
        public readonly IList<string> PaymentMethods;
        public GetPaymentMethodsResponse(XmlDocument response)
            : base(response)
        {
            PaymentMethods = new List<string>();
            var enumerator = response.SelectNodes("/response/paymentmethods/paymentmethod").GetEnumerator();
            while (enumerator.MoveNext())
            {
                PaymentMethods.Add(((XmlNode)enumerator.Current).InnerText);
            }
        }

    }

}