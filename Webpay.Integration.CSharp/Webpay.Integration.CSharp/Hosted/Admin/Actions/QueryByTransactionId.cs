using System;
using System.Collections.Generic;
using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class QueryByTransactionId : Query
    {
        public readonly long TransactionId;
        public readonly Guid? CorrelationId;

        public QueryByTransactionId(long transactionId, Guid? correlationId)
        {
            TransactionId = transactionId;
            CorrelationId = correlationId;
        }
    }

}