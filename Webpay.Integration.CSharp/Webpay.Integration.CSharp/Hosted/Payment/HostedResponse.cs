using System.Xml;
using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Hosted.Payment
{
    public class HostedResponse
    {
        public string Xml { get; private set; }
        public string MessageBase64 { get; private set; }
        public string Mac { get; private set; }
        public string ReceivedMerchantId { get; private set; }
        public string Message { get; private set; }

        public HostedResponse(string xml, string originalSecretWord, string expectedMerchantId)
        {
            Xml = xml;
            var responseDocument = new XmlDocument();
            responseDocument.LoadXml(xml);
            MessageBase64 = responseDocument.SelectSingleNode("//message").InnerText;
            Mac = responseDocument.SelectSingleNode("//mac").InnerText;
            ReceivedMerchantId = responseDocument.SelectSingleNode("//merchantid").InnerText;

            var expectedMac = HashUtil.CreateHash(MessageBase64 + originalSecretWord);

            if (ReceivedMerchantId != expectedMerchantId)
            {
                throw new System.Exception(string.Format("The merchantId in the response from the server is not the expected. This could mean that someone has tamepered with the message. Expected:{0} Actual:{1}", expectedMerchantId, ReceivedMerchantId));
            }

            if (expectedMac != Mac)
            {
                throw new System.Exception(string.Format("SEVERE: The mac from the server does not match the expected mac. The message might have been tampered with, or the secret word used is not correct. Merchant:{0} Message:\n{1}", expectedMerchantId, MessageBase64));
            }

            Message = Base64Util.DecodeBase64String(MessageBase64);
        }
    }
}