using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class GetPaymentMethodsResponse : SpecificHostedAdminResponseBase
{
    public readonly IList<string> PaymentMethods;

    public GetPaymentMethodsResponse(XmlDocument response) : base(response)
    {
        PaymentMethods = new List<string>();
        var enumerator = response.SelectNodes("/response/paymentmethods/paymentmethod").GetEnumerator();
        while (enumerator.MoveNext())
        {
            PaymentMethods.Add(((XmlNode) enumerator.Current).InnerText);
        }
    }
}