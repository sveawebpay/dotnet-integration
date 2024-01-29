using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Recur:BasicRequest
    {
        public readonly long Amount;
        public readonly Currency Currency;
        public readonly string CustomerRefNo;
        public readonly string SubscriptionId;
        public readonly long Vat;

        public Recur(string customerRefNo, string subscriptionId, Currency currency, long amount, string correlationId, long vat = 0) : base(correlationId)
        {
            CustomerRefNo = customerRefNo;
            SubscriptionId = subscriptionId;
            Currency = currency;
            Amount = amount;
            Vat = vat;
        }

        public static RecurResponse Response(XmlDocument responseXml)
        {
            return new RecurResponse(responseXml);
        }
    }
}