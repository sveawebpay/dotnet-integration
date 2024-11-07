//using Microsoft.SqlServer.Server;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Configuration;
//using System.Net;
//using System.Runtime.Remoting.Messaging;
//using System.Text;
//using System.Xml;
//using Webpay.Integration.Util;
//using Webpay.Integration.Util.Security;
//
//namespace Webpay.Integration.Hosted.Admin
//{
//    public class HostedAdminRequest
//    {
//        public readonly string EndPointBase;
//        public readonly string Mac;
//        public readonly string MerchantId;
//        public readonly string Message;
//        public readonly string MessageBase64Encoded;
//        public readonly XmlDocument MessageXmlDocument;
//        public readonly string SecretWord;
//        public readonly List<AdminRequestHeader> Headers;
//
//        public HostedAdminRequest(string message, string secretWord, string merchantId, string endPointBase, List<AdminRequestHeader> headers)
//        {
//            EndPointBase = endPointBase;
//            Message = message;
//            SecretWord = secretWord;
//            MerchantId = merchantId;
//            Headers = headers;
//
//            MessageBase64Encoded = Base64Util.EncodeBase64String(Message);
//            Mac = HashUtil.CreateHash(MessageBase64Encoded + secretWord);
//
//            MessageXmlDocument = new XmlDocument();
//            MessageXmlDocument.LoadXml(message);
//        }
//
//        public HostedAdminResponse DoRequest()
//        {
//            return HostedAdminCall(EndPointBase, this);
//        }
//
//        public static HostedAdminResponse HostedAdminCall(string targetAddress, HostedAdminRequest hostedRequest)
//        {
//            using (var client = new WebClient())
//            {
//                foreach (var item in hostedRequest.Headers)
//                {
//                    client.Headers.Add(item.Header.Key,item.Header.Value?.ToString() ?? "");
//                }
//                             
//                var response =
//                    client.UploadValues(targetAddress, new NameValueCollection
//                    {
//                        {"message", hostedRequest.MessageBase64Encoded},
//                        {"mac", hostedRequest.Mac},
//                        {"merchantid", hostedRequest.MerchantId}
//                    });
//
//                var result = Encoding.UTF8.GetString(response);
//
//                var hostedResponse = new HostedAdminResponse(result, hostedRequest.SecretWord, hostedRequest.MerchantId);
//
//                return hostedResponse;
//            }
//        }
//    }
//}

using System.Collections.Specialized;
using System.Net;
using System.Text;
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

    public HostedAdminResponse DoRequest()
    {
        return HostedAdminCall(EndPointBase, this);
    }

    public static HostedAdminResponse HostedAdminCall(string targetAddress, HostedAdminRequest hostedRequest)
    {
        using (var client = new WebClient())
        {
            foreach (var item in hostedRequest.Headers)
            {
                client.Headers.Add(item.Header.Key, item.Header.Value?.ToString() ?? "");
            }

            var response =
                client.UploadValues(targetAddress, new NameValueCollection
                {
                    { "message", hostedRequest.MessageBase64Encoded },
                    { "mac", hostedRequest.Mac },
                    { "merchantid", hostedRequest.MerchantId }
                });

            var result = Encoding.UTF8.GetString(response);
            var hostedResponse = new HostedAdminResponse(result, hostedRequest.SecretWord, hostedRequest.MerchantId);

            return hostedResponse;
        }
    }
}
