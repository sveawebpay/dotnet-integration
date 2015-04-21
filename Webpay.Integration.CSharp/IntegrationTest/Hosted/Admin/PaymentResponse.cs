using System.Xml;
using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class PaymentResponse
    {
        public string MerchantId { get; private set; }
        public string Mac { get; private set; }
        public string MessageBase64 { get; private set; }
        public string Message { get; private set; }
        public XmlDocument MessageXmlDoc { get; private set; }
        public long TransactionId { get; private set; }

        public string SubscriptionId
        {
            get { return MessageXmlDoc.SelectSingleNode("//subscriptionid").InnerText; }
        }

        public PaymentResponse(string messageBase64, string mac, string merchantId)
        {
            Mac = mac;
            MessageBase64 = messageBase64;
            MerchantId = merchantId;

            Message = Base64Util.DecodeBase64String(messageBase64);
            MessageXmlDoc = new XmlDocument();
            MessageXmlDoc.LoadXml(Message);

            TransactionId = long.Parse(MessageXmlDoc.SelectSingleNode("//transaction").Attributes["id"].Value);
        }

    }
}