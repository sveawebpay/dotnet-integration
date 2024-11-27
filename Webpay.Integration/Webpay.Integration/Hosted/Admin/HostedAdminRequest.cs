using System.Xml;
using Webpay.Integration.Util;
using Webpay.Integration.Util.Security;

namespace Webpay.Integration.Hosted.Admin;

public class HostedAdminRequest
{
    public readonly string EndPointBase;
    public readonly string Mac;
    public readonly string MerchantId;
    public readonly string Message;
    public readonly string MessageBase64Encoded;
    public readonly XmlDocument MessageXmlDocument;
    public readonly string SecretWord;
    public readonly List<AdminRequestHeader> Headers;

    public HostedAdminRequest(string message, string secretWord, string merchantId, string endPointBase, List<AdminRequestHeader> headers)
    {
        EndPointBase = endPointBase;
        Message = message;
        SecretWord = secretWord;
        MerchantId = merchantId;
        Headers = headers;

        MessageBase64Encoded = Base64Util.EncodeBase64String(Message);
        Mac = HashUtil.CreateHash(MessageBase64Encoded + secretWord);

        MessageXmlDocument = new XmlDocument();
        MessageXmlDocument.LoadXml(message);
    }

    public async Task<HostedAdminResponse> DoRequestAsync()
    {
        return await HostedAdminCall(EndPointBase, this);
    }

    //public static HostedAdminResponse HostedAdminCall(string targetAddress, HostedAdminRequest hostedRequest)
    //{
    //    // TODO
    //    using (var client = new WebClient())
    //    {
    //        foreach (var item in hostedRequest.Headers)
    //        {
    //            client.Headers.Add(item.Header.Key, item.Header.Value?.ToString() ?? "");
    //        }

    //        var response =
    //            client.UploadValues(targetAddress, new NameValueCollection
    //            {
    //                { "message", hostedRequest.MessageBase64Encoded },
    //                { "mac", hostedRequest.Mac },
    //                { "merchantid", hostedRequest.MerchantId }
    //            });

    //        var result = Encoding.UTF8.GetString(response);
    //        var hostedResponse = new HostedAdminResponse(result, hostedRequest.SecretWord, hostedRequest.MerchantId);

    //        return hostedResponse;
    //    }
    //}

    public static async Task<HostedAdminResponse> HostedAdminCall(string targetAddress, HostedAdminRequest hostedRequest)
    {
        using (var client = new HttpClient())
        {
            foreach (var item in hostedRequest.Headers)
            {
                client.DefaultRequestHeaders.Add(item.Header.Key, item.Header.Value?.ToString() ?? "");
            }

            var formData = new Dictionary<string, string>
            {
                { "message", hostedRequest.MessageBase64Encoded },
                { "mac", hostedRequest.Mac },
                { "merchantid", hostedRequest.MerchantId }
            };

            var content = new FormUrlEncodedContent(formData);

            HttpResponseMessage response = await client.PostAsync(targetAddress, content);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var hostedResponse = new HostedAdminResponse(result, hostedRequest.SecretWord, hostedRequest.MerchantId);

            return hostedResponse;
        }
    }
}
