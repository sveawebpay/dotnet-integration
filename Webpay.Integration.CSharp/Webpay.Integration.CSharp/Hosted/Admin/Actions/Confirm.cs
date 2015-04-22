using System;
using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class Confirm
    {
        public readonly DateTime CaptureDate;
        public readonly int TransactionId;

        public Confirm(int transactionId, DateTime captureDate)
        {
            TransactionId = transactionId;
            CaptureDate = captureDate;
        }

        public static ConfirmResponse Response(XmlDocument responseXml)
        {
            return new ConfirmResponse(responseXml);
        }
    }

    public class ConfirmResponse : SpecificHostedAdminResponseBase
    {
        public readonly int? TransactionId;
        public readonly string CustomerRefNo;
        public readonly string ClientOrderNumber;

        public ConfirmResponse(XmlDocument response)
            : base(response)
        {
            TransactionId = AttributeInt(response, "/response/transaction", "id");
            CustomerRefNo = TextString(response, "/response/transaction/customerrefno");
            ClientOrderNumber = CustomerRefNo;
        }
    }
}