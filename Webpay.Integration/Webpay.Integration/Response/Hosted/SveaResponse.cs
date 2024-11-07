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
                // TODO:
                //switch(StatusCode)
                //{
                //    case 0:
                //        ResultCode = "0 (ORDER_ACCEPTED)";
                //        break;
                //    case 150:
                //        ResultCode = "150 (CREDIT_PENDING)";
                //        break;
                //}
                ResultCode = StatusCode switch
                {
                    0 => "0 (ORDER_ACCEPTED)",
                    150 => "150 (CREDIT_PENDING)",
                    _ => ResultCode // Keep existing value
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
        XmlNode trans = elementNode.GetElementsByTagName(tagName).Item(0);
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
        XmlNodeList nodeList = elementNode.GetElementsByTagName(tagName);
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
    // TODO
    //public static Tuple<string, string> StatusCodeToMessage(int resultCode)
    //{
    //    var resultCodeMessage = "";
    //    var errorMessage = "";

    //    switch (resultCode)
    //    {
    //        case 0:
    //            resultCodeMessage = resultCode + " (OK)";
    //            errorMessage =
    //                "";
    //            break;
    //        case 1:
    //            resultCodeMessage = resultCode + " (REQUIRES_MANUAL_REVIEW)";
    //            errorMessage =
    //                "Request performed successfully but requires manual review from merchant. Applicable PaymentMethod: PAYPAL.";
    //            break;
    //        case 100:
    //            resultCodeMessage = resultCode + " (INTERNAL_ERROR)";
    //            errorMessage = "Invalid – contact integrator.";
    //            break;
    //        case 101:
    //            resultCodeMessage = resultCode + " (XMLPARSEFAIL)";
    //            errorMessage = "Invalid XML.";
    //            break;
    //        case 102:
    //            resultCodeMessage = resultCode + " (ILLEGAL_ENCODING)";
    //            errorMessage = "Invalid encoding.";
    //            break;
    //        case 104:
    //            resultCodeMessage = resultCode + " (ILLEGAL_URL)";
    //            errorMessage = "Illegal Url.";
    //            break;
    //        case 105:
    //            resultCodeMessage = resultCode + " (ILLEGAL_TRANSACTIONSTATUS)";
    //            errorMessage = "Invalid transaction status.";
    //            break;
    //        case 106:
    //            resultCodeMessage = resultCode + " (EXTERNAL_ERROR)";
    //            errorMessage = "Failure at third party e.g. failure at the bank.";
    //            break;
    //        case 107:
    //            resultCodeMessage = resultCode + " (DENIED_BY_BANK)";
    //            errorMessage = "Transaction rejected by bank.";
    //            break;
    //        case 108:
    //            resultCodeMessage = resultCode + " (CANCELLED)";
    //            errorMessage = "Transaction cancelled.";
    //            break;
    //        case 109:
    //            resultCodeMessage = resultCode + " (NOT_FOUND_AT_BANK)";
    //            errorMessage = "Transaction not found at the bank.";
    //            break;
    //        case 110:
    //            resultCodeMessage = resultCode + " (ILLEGAL_TRANSACTIONID)";
    //            errorMessage = "Invalid transaction ID.";
    //            break;
    //        case 111:
    //            resultCodeMessage = resultCode + " (MERCHANT_NOT_CONFIGURED)";
    //            errorMessage = "Merchant not configured.";
    //            break;
    //        case 112:
    //            resultCodeMessage = resultCode + " (MERCHANT_NOT_CONFIGURED_AT_BANK)";
    //            errorMessage = "Merchant not configured at the bank.";
    //            break;
    //        case 113:
    //            resultCodeMessage = resultCode + " (PAYMENTMETHOD_NOT_CONFIGURED)";
    //            errorMessage = "Payment method not configured for merchant.";
    //            break;
    //        case 114:
    //            resultCodeMessage = resultCode + " (TIMEOUT_AT_BANK)";
    //            errorMessage = "Timeout at the bank.";
    //            break;
    //        case 115:
    //            resultCodeMessage = resultCode + " (MERCHANT_NOT_ACTIVE)";
    //            errorMessage = "The merchant is disabled.";
    //            break;
    //        case 116:
    //            resultCodeMessage = resultCode + " (PAYMENTMETHOD_NOT_ACTIVE)";
    //            errorMessage = "The payment method is disabled.";
    //            break;
    //        case 117:
    //            resultCodeMessage = resultCode + " (ILLEGAL_AUTHORIZED_AMOUNT)";
    //            errorMessage = "Invalid authorized amount.";
    //            break;
    //        case 118:
    //            resultCodeMessage = resultCode + " (ILLEGAL_CAPTURED_AMOUNT)";
    //            errorMessage = "Invalid captured amount.";
    //            break;
    //        case 119:
    //            resultCodeMessage = resultCode + " (ILLEGAL_CREDITED_AMOUNT)";
    //            errorMessage = "Invalid credited amount.";
    //            break;
    //        case 120:
    //            resultCodeMessage = resultCode + " (NOT_SUFFICIENT_FUNDS)";
    //            errorMessage = "Not enough founds.";
    //            break;
    //        case 121:
    //            resultCodeMessage = resultCode + " (EXPIRED_CARD)";
    //            errorMessage = "The card has expired.";
    //            break;
    //        case 122:
    //            resultCodeMessage = resultCode + " (STOLEN_CARD)";
    //            errorMessage = "Stolen card.";
    //            break;
    //        case 123:
    //            resultCodeMessage = resultCode + " (LOST_CARD)";
    //            errorMessage = "Lost card.";
    //            break;
    //        case 124:
    //            resultCodeMessage = resultCode + " (EXCEEDS_AMOUNT_LIMIT)";
    //            errorMessage = "Amount exceeds the limit.";
    //            break;
    //        case 125:
    //            resultCodeMessage = resultCode + " (EXCEEDS_FREQUENCY_LIMIT)";
    //            errorMessage = "Frequency limit exceeded.";
    //            break;
    //        case 126:
    //            resultCodeMessage = resultCode + " (TRANSACTION_NOT_BELONGING_TO_MERCHANT)";
    //            errorMessage = "Transaction does not belong to merchant.";
    //            break;
    //        case 127:
    //            resultCodeMessage = resultCode + " (CUSTOMERREFNO_ALREADY_USED)";
    //            errorMessage = "Customer reference number already used in another transaction.";
    //            break;
    //        case 128:
    //            resultCodeMessage = resultCode + " (NO_SUCH_TRANS)";
    //            errorMessage = "Transaction does not exist.";
    //            break;
    //        case 129:
    //            resultCodeMessage = resultCode + " (DUPLICATE_TRANSACTION)";
    //            errorMessage = "More than one transaction found for the given customer reference number.";
    //            break;
    //        case 130:
    //            resultCodeMessage = resultCode + " (ILLEGAL_OPERATION)";
    //            errorMessage = "Operation not allowed for the given payment method.";
    //            break;
    //        case 131:
    //            resultCodeMessage = resultCode + " (COMPANY_NOT_ACTIVE)";
    //            errorMessage = "Company inactive.";
    //            break;
    //        case 132:
    //            resultCodeMessage = resultCode + " (SUBSCRIPTION_NOT_FOUND)";
    //            errorMessage = "No subscription exist.";
    //            break;
    //        case 133:
    //            resultCodeMessage = resultCode + " (SUBSCRIPTION_NOT_ACTIVE)";
    //            errorMessage = "Subscription not active.";
    //            break;
    //        case 134:
    //            resultCodeMessage = resultCode + " (SUBSCRIPTION_NOT_SUPPORTED)";
    //            errorMessage = "Payment method doesn’t support subscriptions.";
    //            break;
    //        case 135:
    //            resultCodeMessage = resultCode + " (ILLEGAL_DATE_FORMAT)";
    //            errorMessage = "Illegal date format.";
    //            break;
    //        case 136:
    //            resultCodeMessage = resultCode + " (ILLEGAL_RESPONSE_DATA)";
    //            errorMessage = "Illegal response data.";
    //            break;
    //        case 137:
    //            resultCodeMessage = resultCode + " (IGNORE_CALLBACK)";
    //            errorMessage = "Ignore callback.";
    //            break;
    //        case 138:
    //            resultCodeMessage = resultCode + " (CURRENCY_NOT_CONFIGURED)";
    //            errorMessage = "Currency not configured.";
    //            break;
    //        case 139:
    //            resultCodeMessage = resultCode + " (CURRENCY_NOT_ACTIVE)";
    //            errorMessage = "Currency not active.";
    //            break;
    //        case 140:
    //            resultCodeMessage = resultCode + " (CURRENCY_ALREADY_CONFIGURED)";
    //            errorMessage = "Currency is already configured.";
    //            break;
    //        case 141:
    //            resultCodeMessage = resultCode + " (ILLEGAL_AMOUNT_OF_RECURS_TODAY)";
    //            errorMessage = "Ilegal amount of recurs per day.";
    //            break;
    //        case 142:
    //            resultCodeMessage = resultCode + " (NO_VALID_PAYMENT_METHODS)";
    //            errorMessage = "No valid PaymentMethod.";
    //            break;
    //        case 143:
    //            resultCodeMessage = resultCode + " (CREDIT_DENIED_BY_BANK)";
    //            errorMessage = "Credit denied by bank.";
    //            break;
    //        case 144:
    //            resultCodeMessage = resultCode + " (ILLEGAL_CREDIT_USER)";
    //            errorMessage = "User is not allowed to perform credit operation.";
    //            break;
    //        case 146:
    //            resultCodeMessage = resultCode + " (CUSTOMER_NOT_FOUND)";
    //            errorMessage = "Customer not found.";
    //            break;
    //        case 147:
    //            resultCodeMessage = resultCode + " (AGE_LIMIT_EXCEEDED)";
    //            errorMessage = "E.g. the transaction is too old.";
    //            break;
    //        case 148:
    //            resultCodeMessage = resultCode + " (BROWSER_NOT_SUPPORTED)";
    //            errorMessage = "Browser not supported.";
    //            break;
    //        case 149:
    //            resultCodeMessage = resultCode + " (PENDING)";
    //            errorMessage = "Waiting for final Status code.";
    //            break;
    //        case 150:
    //            resultCodeMessage = resultCode + " (CREDIT_PENDING)";
    //            errorMessage = "Credit is currently pending.";
    //            break;
    //        case 300:
    //            resultCodeMessage = resultCode + " (BAD_CARDHOLDER_NAME)";
    //            errorMessage = "Invalid value for cardholder name.";
    //            break;
    //        case 301:
    //            resultCodeMessage = resultCode + " (BAD_TRANSACTION_ID)";
    //            errorMessage = "Invalid value for transaction id.";
    //            break;
    //        case 302:
    //            resultCodeMessage = resultCode + " (BAD_REV)";
    //            errorMessage = "Invalid value for rev.";
    //            break;
    //        case 303:
    //            resultCodeMessage = resultCode + " (BAD_MERCHANT_ID)";
    //            errorMessage = "nvalid value for merchant id.";
    //            break;
    //        case 304:
    //            resultCodeMessage = resultCode + " (BAD_LANG)";
    //            errorMessage = "Invalid value for lang.";
    //            break;
    //        case 305:
    //            resultCodeMessage = resultCode + " (BAD_AMOUNT)";
    //            errorMessage = "Invalid value for amount.";
    //            break;
    //        case 306:
    //            resultCodeMessage = resultCode + " (BAD_CUSTOMERREFNO)";
    //            errorMessage = "Invalid value for customer refno.";
    //            break;
    //        case 307:
    //            resultCodeMessage = resultCode + " (BAD_CURRENCY)";
    //            errorMessage = "Invalid value for currency.";
    //            break;
    //        case 308:
    //            resultCodeMessage = resultCode + " (BAD_PAYMENTMETHOD)";
    //            errorMessage = "Invalid value for payment method.";
    //            break;
    //        case 309:
    //            resultCodeMessage = resultCode + " (BAD_RETURNURL)";
    //            errorMessage = "Invalid value for return url.";
    //            break;
    //        case 310:
    //            resultCodeMessage = resultCode + " (BAD_LASTBOOKINGDAY)";
    //            errorMessage = "Invalid value for last booking day.";
    //            break;
    //        case 311:
    //            resultCodeMessage = resultCode + " (BAD_MAC)";
    //            errorMessage = "Invalid value for mac.";
    //            break;
    //        case 312:
    //            resultCodeMessage = resultCode + " (BAD_TRNUMBER)";
    //            errorMessage = "Invalid value for tr number.";
    //            break;
    //        case 313:
    //            resultCodeMessage = resultCode + " (BAD_QUANTITY)";
    //            errorMessage = "Invalid value for quantity.";
    //            break;
    //        case 314:
    //            resultCodeMessage = resultCode + " (BAD_CC_DESCR)";
    //            errorMessage = "Invalid value for cc_descr.";
    //            break;
    //        case 315:
    //            resultCodeMessage = resultCode + " (BAD_ERROR_CODE)";
    //            errorMessage = "Invalid value for error_code.";
    //            break;
    //        case 316:
    //            resultCodeMessage = resultCode + " (BAD_CARDNUMBER_OR_CARDTYPE_NOT_CONFIGURED)";
    //            errorMessage = "Card type not configured for merchant.";
    //            break;
    //        case 317:
    //            resultCodeMessage = resultCode + " (BAD_SSN)";
    //            errorMessage = "Invalid value for ssn.";
    //            break;
    //        case 318:
    //            resultCodeMessage = resultCode + " (BAD_VAT)";
    //            errorMessage = "Invalid value for vat.";
    //            break;
    //        case 319:
    //            resultCodeMessage = resultCode + " (BAD_CAPTURE_DATE)";
    //            errorMessage = "Invalid value for capture date.";
    //            break;
    //        case 320:
    //            resultCodeMessage = resultCode + " (BAD_CAMPAIGN_CODE_INVALID)";
    //            errorMessage = "Invalid value for campaign code. There are no valid matching campaign codes.";
    //            break;
    //        case 321:
    //            resultCodeMessage = resultCode + " (BAD_SUBSCRIPTION_TYPE)";
    //            errorMessage = "Invalid subscription type.";
    //            break;
    //        case 322:
    //            resultCodeMessage = resultCode + " (BAD_SUBSCRIPTION_ID)";
    //            errorMessage = "Invalid subscription id.";
    //            break;
    //        case 323:
    //            resultCodeMessage = resultCode + " (BAD_BASE64)";
    //            errorMessage = "Invalid base64.";
    //            break;
    //        case 324:
    //            resultCodeMessage = resultCode + " (BAD_CAMPAIGN_CODE)";
    //            errorMessage = "Invalid campaign code. Missing value.";
    //            break;
    //        case 325:
    //            resultCodeMessage = resultCode + " (BAD_CALLBACKURL)";
    //            errorMessage = "Invalid callbackurl.";
    //            break;
    //        case 326:
    //            resultCodeMessage = resultCode + " (THREE_D_CHECK_FAILED)";
    //            errorMessage = "3D check failed.";
    //            break;
    //        case 327:
    //            resultCodeMessage = resultCode + " (CARD_NOT_ENROLLED)";
    //            errorMessage = "Card not enrolled in 3D secure.";
    //            break;
    //        case 328:
    //            resultCodeMessage = resultCode + " (BAD_IPADDRESS)";
    //            errorMessage = "Provided ip address is incorrect.";
    //            break;
    //        case 329:
    //            resultCodeMessage = resultCode + " (BAD_MOBILE)";
    //            errorMessage = "Bad mobile phone number.";
    //            break;
    //        case 330:
    //            resultCodeMessage = resultCode + " (BAD_COUNTRY)";
    //            errorMessage = "Bad country parameter.";
    //            break;
    //        case 331:
    //            resultCodeMessage = resultCode + " (THREE_D_CHECK_NOT_AVAILABLE)";
    //            errorMessage = "Merchants 3D configuration invalid.";
    //            break;
    //        case 332:
    //            resultCodeMessage = resultCode + " (TIMEOUT)";
    //            errorMessage = "Timeout at Svea.";
    //            break;
    //        case 333:
    //            resultCodeMessage = resultCode + " (BAD_PERIOD)";
    //            errorMessage = "The reconciliation period is invalid.";
    //            break;
    //        case 334:
    //            resultCodeMessage = resultCode + " (BAD_ADDRESS_ID)";
    //            errorMessage = "AddressSelector is not valid for this CountryCode.";
    //            break;
    //        case 335:
    //            resultCodeMessage = resultCode + " (BAD_CUSTOMER_DATA)";
    //            errorMessage = "The supplied customer data is invalid.";
    //            break;
    //        case 336:
    //            resultCodeMessage = resultCode + " (BAD_UNIT)";
    //            errorMessage = "Invalid unit.";
    //            break;
    //        case 337:
    //            resultCodeMessage = resultCode + " (BAD_EXTERNAL_PAYMENT_REF)";
    //            errorMessage = "Invalid external payment reference.";
    //            break;
    //        case 338:
    //            resultCodeMessage = resultCode + " (BAD_STOREDCARDALIAS)";
    //            errorMessage = "Invalid stored card alias.";
    //            break;
    //        case 339:
    //            resultCodeMessage = resultCode + " (STOREDCARDALIS_NOT_ACTIVE)";
    //            errorMessage = "Stored card alias inactive.";
    //            break;
    //        case 340:
    //            resultCodeMessage = resultCode + " (STORED_CARDS_NOT_ENABLED)";
    //            errorMessage = "This merchant does not have stored cards enabled.";
    //            break;
    //        case 341:
    //            resultCodeMessage = resultCode + " (ONLY_DEBIT_CARDS_ALLOWED)";
    //            errorMessage = "Only debit cards allowed.";
    //            break;     
    //        case 500:
    //            resultCodeMessage = resultCode + " (ANTIFRAUD_CARDBIN_NOT_ALLOWED)";
    //            errorMessage = "Antifraud - cardbin not allowed.";
    //            break;
    //        case 501:
    //            resultCodeMessage = resultCode + " (ANTIFRAUD_IPLOCATION_NOT_ALLOWED)";
    //            errorMessage = "Antifraud – iplocation not allowed.";
    //            break;
    //        case 502:
    //            resultCodeMessage = resultCode + " (ANTIFRAUD_IPLOCATION_AND_BIN_DOESNT_MATCH)";
    //            errorMessage = "Antifraud – ip-location and bin does not match.";
    //            break;
    //        case 503:
    //            resultCodeMessage = resultCode + " (ANTIFRAUD_MAX_AMOUNT_PER_IP_EXCEEDED)";
    //            errorMessage = "Antifraud – max amount per ip exceeded.";
    //            break;
    //        case 504:
    //            resultCodeMessage = resultCode + " (ANTIFRAUD_MAX_TRANSACTIONS_PER_IP_EXCEEDED)";
    //            errorMessage = "Antifraud – max transactions per ip exceeded.";
    //            break;
    //        case 505:
    //            resultCodeMessage = resultCode + " (ANTIFRAUD_MAX_TRANSACTIONS_PER_CARDNO_EXCEEDED)";
    //            errorMessage = "Antifraud – max transactions per card number exceeded.";
    //            break;
    //        case 506:
    //            resultCodeMessage = resultCode + " (ANTIFRAUD_MAX_AMOUNT_PER_CARDNO_EXCEEDED)";
    //            errorMessage = "Antifraud – max amount per cardnumber exceeded.";
    //            break;
    //        case 507:
    //            resultCodeMessage = resultCode + " (ANTIFRAUD_IP_ADDRESS_BLOCKED)";
    //            errorMessage = "Antifraud – IP address blocked.";
    //            break;
    //        case 403:
    //            resultCodeMessage = resultCode + " (BAD_Request)";
    //            errorMessage = "Validation failed";
    //            break;
    //        default:
    //            resultCodeMessage = resultCode + " (UNKNOWN_ERROR)";
    //            errorMessage = "Unknown error.";
    //            break;
    //    }

    //    Tuple<string, string> resultTuple = new Tuple<string, string>(resultCodeMessage, errorMessage);
    //    return resultTuple;
    //}
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