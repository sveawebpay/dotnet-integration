using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class QueryByTransactionId
    {
        public readonly long TransactionId;

        public QueryByTransactionId(long transactionId)
        {
            TransactionId = transactionId;
        }

        public static QueryResponse Response(XmlDocument responseXml)
        {
            return new QueryResponse(responseXml);
        }
    }

    public class QueryResponse : CustomerRefNoResponseBase
    {
        public QueryResponse(XmlDocument response)
            : base(response)
        {
        }
    }
}