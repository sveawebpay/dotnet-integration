using System;
using System.Xml;
using Webpay.Integration.CSharp.Hosted.Admin.Response;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Annul:BasicRequest
    {
        public readonly long TransactionId;

        public Annul(long transactionId, Guid? correlationId) : base(correlationId)
        {
            TransactionId = transactionId;
        }

        public static AnnulResponse Response(XmlDocument response)
        {
            return new AnnulResponse(response);
        }
    }
}