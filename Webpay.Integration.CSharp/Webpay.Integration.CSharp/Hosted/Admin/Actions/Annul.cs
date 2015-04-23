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

    public class AnnulResponse : CustomerRefNoResponseBase
    {
        public AnnulResponse(XmlDocument response) : base(response)
        {
        }
    }
}