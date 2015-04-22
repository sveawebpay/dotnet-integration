using System.Collections.Specialized;
using System.Net;
using System.Xml;
using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class HostedAdminRequest
    {
        public string Message { get; private set; }
        public string SecretWord { get; private set; }
        public string MerchantId { get; private set; }
        public string MessageBase64Encoded { get; private set; }
        public string Mac { get; private set; }
        public XmlDocument MessageXmlDocument { get; private set; }

        public HostedAdminRequest(string message, string secretWord, string merchantId)
        {
            Message = message;
            SecretWord = secretWord;
            MerchantId = merchantId;

            MessageBase64Encoded = Base64Util.EncodeBase64String(Message);
            Mac = HashUtil.CreateHash(MessageBase64Encoded + secretWord);

            MessageXmlDocument = new XmlDocument();
            MessageXmlDocument.LoadXml(message);
        }

        public static HostedAdminResponse HostedAdminCall(string targetAddress, HostedAdminRequest hostedRequest)
        {
            using (var client = new WebClient())
            {
                var response =
                    client.UploadValues(targetAddress, new NameValueCollection()
                    {
                        {"message", hostedRequest.MessageBase64Encoded},
                        {"mac", hostedRequest.Mac},
                        {"merchantid", hostedRequest.MerchantId}
                    });

                var result = System.Text.Encoding.UTF8.GetString(response);

                var hostedResponse = new HostedAdminResponse(result, hostedRequest.SecretWord, hostedRequest.MerchantId);

                return hostedResponse;
            }
        }
    }
}