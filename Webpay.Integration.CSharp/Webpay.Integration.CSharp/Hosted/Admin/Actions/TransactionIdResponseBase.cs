using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class TransactionIdResponseBase : SpecificHostedAdminResponseBase
    {
        public int? TransactionId;

        public TransactionIdResponseBase(XmlDocument response)
            : base(response)
        {
            TransactionId = AttributeInt(response, "/response/transaction", "id");
        }
    }
}