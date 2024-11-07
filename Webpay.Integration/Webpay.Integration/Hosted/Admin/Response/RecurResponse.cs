using System.Xml;

namespace Webpay.Integration.Hosted.Admin.Response;

public class RecurResponse : CustomerRefNoResponseBase
{
    public readonly string PaymentMethod;
    public readonly int? MerchantId;
    public readonly decimal? Amount;
    public readonly string Currency;
    public readonly string CardType;
    public readonly string MaskedCardNo;
    public readonly string ExpiryMonth;
    public readonly string ExpiryYear;
    public readonly string AuthCode;
    public readonly string SubscriptionId;

    public RecurResponse(XmlDocument response) : base(response)
    {
        PaymentMethod = TextString(response, "/response/transaction/paymentmethod");
        MerchantId = TextInt(response, "/response/transaction/merchantid");
        Amount = MinorCurrencyToDecimalAmount(TextInt(response, "/response/transaction/amount"));
        Currency = TextString(response, "/response/transaction/currency");
        CardType = TextString(response, "/response/transaction/cardtype");
        MaskedCardNo = TextString(response, "/response/transaction/maskedcardno");
        ExpiryMonth = TextString(response, "/response/transaction/expirymonth");
        ExpiryYear = TextString(response, "/response/transaction/expiryyear");
        AuthCode = TextString(response, "/response/transaction/authcode");
        SubscriptionId = TextString(response, "/response/transaction/subscriptionid");
    }
}