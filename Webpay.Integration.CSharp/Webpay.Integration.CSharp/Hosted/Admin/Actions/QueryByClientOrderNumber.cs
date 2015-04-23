namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    /// <summary>
    ///     Convenience class if you use ClientOrderNumber instead of CustomerRefNo in your code
    /// </summary>
    public class QueryByClientOrderNumber : QueryByCustomerRefNo
    {
        public QueryByClientOrderNumber(string clientOrderNumber) : base(clientOrderNumber)
        {
        }
    }
}