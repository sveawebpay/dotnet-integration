namespace Webpay.Integration.CSharp.Hosted.Admin
{
    public class QueryByCustomerRefNo
    {
        public string CustomerRefNo { get; private set; }

        public QueryByCustomerRefNo(string customerRefNo)
        {
            CustomerRefNo = customerRefNo;
        }
    }

    /// <summary>
    /// Convenience class if you use ClientOrderNumber instead of CustomerRefNo in your code
    /// </summary>
    public class QueryByClientOrderNumber : QueryByCustomerRefNo
    {
        public QueryByClientOrderNumber(string clientOrderNumber) : base(clientOrderNumber)
        {
        }
    }
}