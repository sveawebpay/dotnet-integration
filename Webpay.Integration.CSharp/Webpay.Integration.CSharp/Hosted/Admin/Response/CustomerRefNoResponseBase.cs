using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class CustomerRefNoResponseBase : TransactionIdResponseBase
    {
        public readonly string ClientOrderNumber;
        public readonly string CustomerRefNo;

        public CustomerRefNoResponseBase(XmlDocument response) : base(response)
        {
            CustomerRefNo = TextString(response, "/response/transaction/customerrefno");
            ClientOrderNumber = CustomerRefNo;
        }
    }
}