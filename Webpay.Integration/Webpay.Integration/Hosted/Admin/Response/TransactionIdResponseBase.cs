using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class TransactionIdResponseBase : SpecificHostedAdminResponseBase
{
    public long? TransactionId;

    public TransactionIdResponseBase(XmlDocument response) : base(response)
    {
        TransactionId = AttributeLong(response, "/response/transaction", "id");
    }
}