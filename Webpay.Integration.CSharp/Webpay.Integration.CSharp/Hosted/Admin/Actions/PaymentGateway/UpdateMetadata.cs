using System;
using System.Collections.Generic;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Util;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway
{
    public class UpdateMetadata : BasicRequest
    {
        public readonly long TransactionId;
        internal Dictionary<string, string> Metadata = new Dictionary<string, string>();
        public UpdateMetadata(long transactionId, Dictionary<string, string> metadata, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
            Metadata = metadata;
        }
        public string GetXmlForMetadata()
        {
            var xml = "";
            foreach (var item in Metadata)
            {
                xml += item.GetXml();
            };
            return xml;
        }
        public static UpdateMetadataResponse Response(XmlDocument response)
        {
            return new UpdateMetadataResponse(response);
        }
       
    }
}