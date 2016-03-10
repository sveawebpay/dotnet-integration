namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class QueryByCustomerRefNo : Query
    {
        public readonly string CustomerRefNo;

        public QueryByCustomerRefNo(string customerRefNo)
        {
            CustomerRefNo = customerRefNo;
        }
    }
}