using System.Xml;
using Webpay.Integration.Util.Security;

namespace Webpay.Integration.Hosted.Admin;

public class HostedAdminResponse
{
    public readonly string Mac;
    public readonly string MerchantId;
    public readonly string Message;
    public readonly string MessageBase64Encoded;
    public readonly XmlDocument MessageXmlDocument;
    public readonly string WebserviceResponseXml;

    public HostedAdminResponse(string webserviceResponseXml, string originalSecretWord, string expectedMerchantId)
    {
        WebserviceResponseXml = webserviceResponseXml;
        var responseDocument = new XmlDocument();
        responseDocument.LoadXml(webserviceResponseXml);
        MessageBase64Encoded = responseDocument.SelectSingleNode("//message").InnerText;
        Mac = responseDocument.SelectSingleNode("//mac").InnerText;
        MerchantId = responseDocument.SelectSingleNode("//merchantid").InnerText;

        var expectedMac = HashUtil.CreateHash(MessageBase64Encoded + originalSecretWord);

        if (MerchantId != expectedMerchantId)
        {
            throw new System.Exception(
                string.Format(
                    "The merchantId in the response from the server is not the expected. This could mean that someone has tamepered with the message. Expected:{0} Actual:{1}",
                    expectedMerchantId, MerchantId));
        }

        if (Mac != expectedMac)
        {
            throw new System.Exception(
                string.Format(
                    "SEVERE: The mac from the server does not match the expected mac. The message might have been tampered with, or the secret word used is not correct. Merchant:{0} Message:\n{1}",
                    expectedMerchantId, MessageBase64Encoded));
        }

        Message = Base64Util.DecodeBase64String(MessageBase64Encoded);

        MessageXmlDocument = new XmlDocument();
        MessageXmlDocument.LoadXml(Message);
    }

    /// <summary>
    ///     Create a specific object using a function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="funcToObject"></param>
    /// <returns></returns>
    public T To<T>(Func<XmlDocument, T> funcToObject)
    {
        return funcToObject(MessageXmlDocument);
    }
}