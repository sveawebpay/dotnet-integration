using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class CustomerRefNoResponseBase : SpecificHostedAdminResponseBase
    {
        public readonly string ClientOrderNumber;
        public readonly string CustomerRefNo;
        public readonly int? TransactionId;

        public CustomerRefNoResponseBase(XmlDocument response)
            : base(response)
        {
            TransactionId = AttributeInt(response, "/response/transaction", "id");
            CustomerRefNo = TextString(response, "/response/transaction/customerrefno");
            ClientOrderNumber = CustomerRefNo;
        }
    }
}