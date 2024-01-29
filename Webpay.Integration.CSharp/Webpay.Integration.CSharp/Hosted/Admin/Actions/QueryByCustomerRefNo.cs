namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class QueryByCustomerRefNo : Query
    {
        public readonly string CustomerRefNo;
        public readonly string CorrelationId;
        public QueryByCustomerRefNo(string customerRefNo, string correlationId)
        {
            CustomerRefNo = customerRefNo;
            CorrelationId = correlationId;
        }
    }
}