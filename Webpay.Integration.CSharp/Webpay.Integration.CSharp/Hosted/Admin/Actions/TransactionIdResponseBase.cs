using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class TransactionIdResponseBase : SpecificHostedAdminResponseBase
    {
        public long? TransactionId;

        public TransactionIdResponseBase(XmlDocument response)
            : base(response)
        {
            TransactionId = AttributeLong(response, "/response/transaction", "id");
        }
    }
}