using System;
using System.Collections.Generic;
using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class QueryByTransactionId : Query
    {
        public readonly long TransactionId;
        public readonly string CorrelationId;

        public QueryByTransactionId(long transactionId, string correlationId)
        {
            TransactionId = transactionId;
            CorrelationId = correlationId;
        }
    }

}