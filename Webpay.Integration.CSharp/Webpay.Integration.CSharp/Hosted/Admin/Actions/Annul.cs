using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Annul
    {
        public readonly long TransactionId;

        public Annul(long transactionId)
        {
            TransactionId = transactionId;
        }

        public static AnnulResponse Response(XmlDocument response)
        {
            return new AnnulResponse(response);
        }
    }

    public class AnnulResponse : SpecificHostedAdminResponseBase
    {
        public int? TransactionId { get; private set; }
        public string CustomerRefNo { get; private set; }
        public string ClientOrderNumber { get; private set; }

        public AnnulResponse(XmlDocument response) : base(response)
        {
            TransactionId = AttributeInt(response, "/response/transaction", "id");
            CustomerRefNo = TextString(response, "/response/transaction/customerrefno");
            ClientOrderNumber = CustomerRefNo;
        }
    }
}