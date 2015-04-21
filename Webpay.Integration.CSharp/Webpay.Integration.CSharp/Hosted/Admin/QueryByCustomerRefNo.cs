namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin
{
    public class QueryByCustomerRefNo
    {
        public string CustomerRefNo { get; private set; }

        public QueryByCustomerRefNo(string customerRefNo)
        {
            CustomerRefNo = customerRefNo;
        }
    }
}