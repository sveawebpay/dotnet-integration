using System;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class QueryByCustomerRefNo : Query
    {
        public readonly string CustomerRefNo;
        public readonly Guid? CorrelationId;
        public QueryByCustomerRefNo(string customerRefNo, Guid? correlationId)
        {
            CustomerRefNo = customerRefNo;
            CorrelationId = correlationId;
        }
    }
}