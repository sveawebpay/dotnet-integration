using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Hosted.Payment
{
    public class HostedAdminRequest
    {
        public string XmlMessage { get; private set; }
        public string SecretWord { get; private set; }
        public string MerchantId { get; private set; }
        public string Base64Message { get; private set; }
        public string Mac { get; private set; }

        public HostedAdminRequest(string xmlMessage, string secretWord, string merchantId)
        {
            XmlMessage = xmlMessage;
            SecretWord = secretWord;
            MerchantId = merchantId;

            Base64Message = Base64Util.EncodeBase64String(XmlMessage);
            Mac = HashUtil.CreateHash(Base64Message + secretWord);
        }
    }
}