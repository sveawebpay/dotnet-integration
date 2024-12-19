using System.Text;
using System.Xml;
using System.Security.Cryptography;
using Webpay.Integration.Config;
using Webpay.Integration.Util.Security;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Response.Hosted;

public class SveaResponse : Response
{
    public int StatusCode { get; set; }
    public string TransactionId { get; set; }
    public string ClientOrderNumber { get; set; }
    public string PaymentMethod { get; set; }
    public string MerchantId { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; }
    public string SubscriptionId { get; set; }
    public string SubscriptionType { get; set; }
    public string CardType { get; set; }
    public string MaskedCardNumber { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string AuthCode { get; set; }
    public string Xml { get; set; }
    public int MacValidation { get; set; }

    /// <summary>
    /// SveaResponse
    /// </summary>
    /// <param name="responseXmlBase64"></param>
    /// <exception cref="Exception"></exception>
    public SveaResponse(string responseXmlBase64, string macToValidate = null, CountryCode countryCode = 0, IConfigurationProvider config = null)
    {
        if (responseXmlBase64 != null)
        {
            if (config != null)
            {
                string secret = config.GetSecretWord(PaymentType.HOSTED, countryCode);
                if (validateMac(responseXmlBase64, secret, macToValidate) == true)
                {
                    MacValidation = 1;
                }
                else
                {
                    MacValidation = 2;
                }
            }
            SetValues(responseXmlBase64);
        }
    }

    private bool validateMac(string responseXmlBase64, string secret, string macToValidate)
    {
        var calculatedMac = stringToHashString(responseXmlBase64, secret);
        if (macToValidate == calculatedMac)
        {
            return true;
        }

        return false;
    }

    private static string stringToHashString(string responseXmlBase64, string secret)
    {
        string[] arrayStr = {responseXmlBase64, secret};
        string calculatedMac = String.Join("",arrayStr);
        using (SHA512 hash = SHA512Managed.Create())
        {
            return String.Join("", hash.ComputeHash(Encoding.UTF8.GetBytes(calculatedMac)).Select(item => item.ToString("x2")));
        }
    }

    private void SetValues(string xmlBase64)
    {
        Xml = Base64Util.DecodeBase64String(xmlBase64);

        var d1 = new XmlDocument();
        d1.LoadXml(Xml);
        foreach (XmlElement element in d1.GetElementsByTagName("response"))
        {
            StatusCode = int.Parse(GetTagValue(element, "statuscode"));

            if (StatusCode == 0 && MacValidation == 2 || StatusCode == 150 && MacValidation == 2)
            {
                OrderAccepted = false;
                ErrorMessage = "Mac validation failed.";
            }
            else if (StatusCode == 0 && MacValidation <= 1 || StatusCode == 150 && MacValidation <= 1)
            {
                ResultCode = StatusCode switch
                {
                    0 => "0 (ORDER_ACCEPTED)",
                    150 => "150 (CREDIT_PENDING)",
                    _ => ResultCode
                };

                OrderAccepted = true;
            }

            else
            {
                OrderAccepted = false;
                SetErrorParams(StatusCode);
            }

            TransactionId = GetTagAttribute(element, "transaction", "id");
            PaymentMethod = GetTagValue(element, "paymentmethod");
            MerchantId = GetTagValue(element, "merchantid");
            ClientOrderNumber = GetTagValue(element, "customerrefno");
            Amount = int.Parse(GetTagValue(element, "amount")) * 0.01;
            Currency = GetTagValue(element, "currency");
            SubscriptionId = GetTagValue(element, "subscriptionid");
            SubscriptionType = GetTagValue(element, "subscriptiontype");
            CardType = GetTagValue(element, "cardtype");
            MaskedCardNumber = GetTagValue(element, "maskedcardno");
            ExpiryMonth = GetTagValue(element, "expirymonth");
            ExpiryYear = GetTagValue(element, "expiryyear");
            AuthCode = GetTagValue(element, "authcode");
        }
    }

    private string GetTagAttribute(XmlElement elementNode, string tagName, string attributeName)
    {
        var trans = elementNode.GetElementsByTagName(tagName).Item(0);
        if (trans != null)
        {
            XmlNamedNodeMap attr = trans.Attributes;
            if (attr != null)
            {
                return attr.GetNamedItem(attributeName).Value;
            }
        }
        return null;
    }

    private string GetTagValue(XmlElement elementNode, string tagName)
    {
        var nodeList = elementNode.GetElementsByTagName(tagName);
        var element = (XmlElement) nodeList.Item(0);
        if (element != null && element.HasChildNodes)
        {
            XmlNodeList textList = element.ChildNodes;
            XmlNode xmlNode = textList.Item(0);
            if (xmlNode != null)
            {
                return xmlNode.Value;
            }
        }
        return null;
    }

    private void SetErrorParams(int resultCode)
    {
        var resultTuple = StatusCodeToMessage(resultCode);

        this.ResultCode = resultTuple.Item1;
        this.ErrorMessage = resultTuple.Item2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns>Tuple with Item1: result code message and Item2: errorMessage</returns>
    public static (string resultCodeMessage, string errorMessage) StatusCodeToMessage(int resultCode) =>
        resultCode switch
        {
            0 => ($"{resultCode} (OK)", ""),
            1 => ($"{resultCode} (REQUIRES_MANUAL_REVIEW)", "Request performed successfully but requires manual review from merchant. Applicable PaymentMethod: PAYPAL."),
            100 => ($"{resultCode} (INTERNAL_ERROR)", "Invalid – contact integrator."),
            101 => ($"{resultCode} (XMLPARSEFAIL)", "Invalid XML."),
            102 => ($"{resultCode} (ILLEGAL_ENCODING)", "Invalid encoding."),
            104 => ($"{resultCode} (ILLEGAL_URL)", "Illegal Url."),
            105 => ($"{resultCode} (ILLEGAL_TRANSACTIONSTATUS)", "Invalid transaction status."),
            106 => ($"{resultCode} (EXTERNAL_ERROR)", "Failure at third party e.g. failure at the bank."),
            107 => ($"{resultCode} (DENIED_BY_BANK)", "Transaction rejected by bank."),
            108 => ($"{resultCode} (CANCELLED)", "Transaction cancelled."),
            109 => ($"{resultCode} (NOT_FOUND_AT_BANK)", "Transaction not found at the bank."),
            110 => ($"{resultCode} (ILLEGAL_TRANSACTIONID)", "Invalid transaction ID."),
            111 => ($"{resultCode} (MERCHANT_NOT_CONFIGURED)", "Merchant not configured."),
            112 => ($"{resultCode} (MERCHANT_NOT_CONFIGURED_AT_BANK)", "Merchant not configured at the bank."),
            113 => ($"{resultCode} (PAYMENTMETHOD_NOT_CONFIGURED)", "Payment method not configured for merchant."),
            114 => ($"{resultCode} (TIMEOUT_AT_BANK)", "Timeout at the bank."),
            115 => ($"{resultCode} (MERCHANT_NOT_ACTIVE)", "The merchant is disabled."),
            116 => ($"{resultCode} (PAYMENTMETHOD_NOT_ACTIVE)", "The payment method is disabled."),
            117 => ($"{resultCode} (ILLEGAL_AUTHORIZED_AMOUNT)", "Invalid authorized amount."),
            118 => ($"{resultCode} (ILLEGAL_CAPTURED_AMOUNT)", "Invalid captured amount."),
            119 => ($"{resultCode} (ILLEGAL_CREDITED_AMOUNT)", "Invalid credited amount."),
            120 => ($"{resultCode} (NOT_SUFFICIENT_FUNDS)", "Not enough funds."),
            121 => ($"{resultCode} (EXPIRED_CARD)", "The card has expired."),
            122 => ($"{resultCode} (STOLEN_CARD)", "Stolen card."),
            123 => ($"{resultCode} (LOST_CARD)", "Lost card."),
            124 => ($"{resultCode} (EXCEEDS_AMOUNT_LIMIT)", "Amount exceeds the limit."),
            125 => ($"{resultCode} (EXCEEDS_FREQUENCY_LIMIT)", "Frequency limit exceeded."),
            126 => ($"{resultCode} (TRANSACTION_NOT_BELONGING_TO_MERCHANT)", "Transaction does not belong to merchant."),
            127 => ($"{resultCode} (CUSTOMERREFNO_ALREADY_USED)", "Customer reference number already used in another transaction."),
            128 => ($"{resultCode} (NO_SUCH_TRANS)", "Transaction does not exist."),
            129 => ($"{resultCode} (DUPLICATE_TRANSACTION)", "More than one transaction found for the given customer reference number."),
            130 => ($"{resultCode} (ILLEGAL_OPERATION)", "Operation not allowed for the given payment method."),
            131 => ($"{resultCode} (COMPANY_NOT_ACTIVE)", "Company inactive."),
            132 => ($"{resultCode} (SUBSCRIPTION_NOT_FOUND)", "No subscription exists."),
            133 => ($"{resultCode} (SUBSCRIPTION_NOT_ACTIVE)", "Subscription not active."),
            134 => ($"{resultCode} (SUBSCRIPTION_NOT_SUPPORTED)", "Payment method doesn’t support subscriptions."),
            135 => ($"{resultCode} (ILLEGAL_DATE_FORMAT)", "Illegal date format."),
            136 => ($"{resultCode} (ILLEGAL_RESPONSE_DATA)", "Illegal response data."),
            137 => ($"{resultCode} (IGNORE_CALLBACK)", "Ignore callback."),
            138 => ($"{resultCode} (CURRENCY_NOT_CONFIGURED)", "Currency not configured."),
            139 => ($"{resultCode} (CURRENCY_NOT_ACTIVE)", "Currency not active."),
            140 => ($"{resultCode} (CURRENCY_ALREADY_CONFIGURED)", "Currency is already configured."),
            141 => ($"{resultCode} (ILLEGAL_AMOUNT_OF_RECURS_TODAY)", "Illegal amount of recurs per day."),
            142 => ($"{resultCode} (NO_VALID_PAYMENT_METHODS)", "No valid PaymentMethod."),
            143 => ($"{resultCode} (CREDIT_DENIED_BY_BANK)", "Credit denied by bank."),
            144 => ($"{resultCode} (ILLEGAL_CREDIT_USER)", "User is not allowed to perform credit operation."),
            146 => ($"{resultCode} (CUSTOMER_NOT_FOUND)", "Customer not found."),
            147 => ($"{resultCode} (AGE_LIMIT_EXCEEDED)", "E.g., the transaction is too old."),
            148 => ($"{resultCode} (BROWSER_NOT_SUPPORTED)", "Browser not supported."),
            149 => ($"{resultCode} (PENDING)", "Waiting for final Status code."),
            150 => ($"{resultCode} (CREDIT_PENDING)", "Credit is currently pending."),
            300 => ($"{resultCode} (BAD_CARDHOLDER_NAME)", "Invalid value for cardholder name."),
            301 => ($"{resultCode} (BAD_TRANSACTION_ID)", "Invalid value for transaction id."),
            302 => ($"{resultCode} (BAD_REV)", "Invalid value for rev."),
            303 => ($"{resultCode} (BAD_MERCHANT_ID)", "Invalid value for merchant id."),
            304 => ($"{resultCode} (BAD_LANG)", "Invalid value for lang."),
            305 => ($"{resultCode} (BAD_AMOUNT)", "Invalid value for amount."),
            306 => ($"{resultCode} (BAD_CUSTOMERREFNO)", "Invalid value for customer refno."),
            307 => ($"{resultCode} (BAD_CURRENCY)", "Invalid value for currency."),
            308 => ($"{resultCode} (BAD_PAYMENTMETHOD)", "Invalid value for payment method."),
            309 => ($"{resultCode} (BAD_RETURNURL)", "Invalid value for return url."),
            310 => ($"{resultCode} (BAD_LASTBOOKINGDAY)", "Invalid value for last booking day."),
            311 => ($"{resultCode} (BAD_MAC)", "Invalid value for mac."),
            312 => ($"{resultCode} (BAD_TRNUMBER)", "Invalid value for tr number."),
            313 => ($"{resultCode} (BAD_QUANTITY)", "Invalid value for quantity."),
            314 => ($"{resultCode} (BAD_CC_DESCR)", "Invalid value for cc_descr."),
            315 => ($"{resultCode} (BAD_ERROR_CODE)", "Invalid value for error_code."),
            316 => ($"{resultCode} (BAD_CARDNUMBER_OR_CARDTYPE_NOT_CONFIGURED)", "Card type not configured for merchant."),
            317 => ($"{resultCode} (BAD_SSN)", "Invalid value for ssn."),
            318 => ($"{resultCode} (BAD_VAT)", "Invalid value for vat."),
            319 => ($"{resultCode} (BAD_CAPTURE_DATE)", "Invalid value for capture date."),
            320 => ($"{resultCode} (BAD_CAMPAIGN_CODE_INVALID)", "Invalid value for campaign code. There are no valid matching campaign codes."),
            321 => ($"{resultCode} (BAD_SUBSCRIPTION_TYPE)", "Invalid subscription type."),
            322 => ($"{resultCode} (BAD_SUBSCRIPTION_ID)", "Invalid subscription id."),
            323 => ($"{resultCode} (BAD_BASE64)", "Invalid base64."),
            324 => ($"{resultCode} (BAD_CAMPAIGN_CODE)", "Invalid campaign code. Missing value."),
            325 => ($"{resultCode} (BAD_CALLBACKURL)", "Invalid callbackurl."),
            326 => ($"{resultCode} (THREE_D_CHECK_FAILED)", "3D check failed."),
            327 => ($"{resultCode} (CARD_NOT_ENROLLED)", "Card not enrolled in 3D secure."),
            328 => ($"{resultCode} (BAD_IPADDRESS)", "Provided ip address is incorrect."),
            329 => ($"{resultCode} (BAD_MOBILE)", "Bad mobile phone number."),
            330 => ($"{resultCode} (BAD_COUNTRY)", "Bad country parameter."),
            331 => ($"{resultCode} (THREE_D_CHECK_NOT_AVAILABLE)", "Merchants 3D configuration invalid."),
            332 => ($"{resultCode} (TIMEOUT)", "Timeout at Svea."),
            333 => ($"{resultCode} (BAD_PERIOD)", "The reconciliation period is invalid."),
            334 => ($"{resultCode} (BAD_ADDRESS_ID)", "AddressSelector is not valid for this CountryCode."),
            335 => ($"{resultCode} (BAD_CUSTOMER_DATA)", "The supplied customer data is invalid."),
            336 => ($"{resultCode} (BAD_UNIT)", "Invalid unit."),
            337 => ($"{resultCode} (BAD_EXTERNAL_PAYMENT_REF)", "Invalid external payment reference."),
            338 => ($"{resultCode} (BAD_STOREDCARDALIAS)", "Invalid stored card alias."),
            339 => ($"{resultCode} (STOREDCARDALIS_NOT_ACTIVE)", "Stored card alias inactive."),
            340 => ($"{resultCode} (STORED_CARDS_NOT_ENABLED)", "This merchant does not have stored cards enabled."),
            341 => ($"{resultCode} (ONLY_DEBIT_CARDS_ALLOWED)", "Only debit cards allowed."),
            500 => ($"{resultCode} (ANTIFRAUD_CARDBIN_NOT_ALLOWED)", "Antifraud - cardbin not allowed."),
            501 => ($"{resultCode} (ANTIFRAUD_IPLOCATION_NOT_ALLOWED)", "Antifraud – iplocation not allowed."),
            502 => ($"{resultCode} (ANTIFRAUD_IPLOCATION_AND_BIN_DOESNT_MATCH)", "Antifraud – ip-location and bin do not match."),
            503 => ($"{resultCode} (ANTIFRAUD_MAX_AMOUNT_PER_IP_EXCEEDED)", "Antifraud – max amount per ip exceeded."),
            504 => ($"{resultCode} (ANTIFRAUD_MAX_TRANSACTIONS_PER_IP_EXCEEDED)", "Antifraud – max transactions per ip exceeded."),
            505 => ($"{resultCode} (ANTIFRAUD_MAX_TRANSACTIONS_PER_CARDNO_EXCEEDED)", "Antifraud – max transactions per card number exceeded."),
            506 => ($"{resultCode} (ANTIFRAUD_MAX_AMOUNT_PER_CARDNO_EXCEEDED)", "Antifraud – max amount per card number exceeded."),
            507 => ($"{resultCode} (ANTIFRAUD_IP_ADDRESS_BLOCKED)", "Antifraud – IP address blocked."),
            403 => ($"{resultCode} (BAD_Request)", "Validation failed."),
            _ => ($"{resultCode} (UNKNOWN_ERROR)", "Unknown error.")
        };
}