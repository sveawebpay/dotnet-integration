using System.Collections.Specialized;
using System.Net;
using System.Xml;
using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Hosted.Helper
{
    public class HostedAdminRequest
    {
        public string XmlMessage { get; private set; }
        public string SecretWord { get; private set; }
        public string MerchantId { get; private set; }
        public string Base64Message { get; private set; }
        public string Mac { get; private set; }
        public XmlDocument XmlDoc { get; private set; }

        public HostedAdminRequest(string xmlMessage, string secretWord, string merchantId)
        {
            XmlMessage = xmlMessage;
            SecretWord = secretWord;
            MerchantId = merchantId;

            Base64Message = Base64Util.EncodeBase64String(XmlMessage);
            Mac = HashUtil.CreateHash(Base64Message + secretWord);

            XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(xmlMessage);
        }

        public static HostedAdminResponse HostedAdminCall(string targetAddress, HostedAdminRequest hostedRequest)
        {
            using (var client = new WebClient())
            {
                var response =
                    client.UploadValues(targetAddress, new NameValueCollection()
                    {
                        {"message", hostedRequest.Base64Message},
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