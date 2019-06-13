using System;
using System.Text;
using System.Xml;
using System.Linq;
using System.Security.Cryptography;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Security;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Response.Hosted
{
    public class SveaResponse : Response
    {
       // public readonly SveaConfig Config = new SveaConfig();

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
                string calculatedMac = stringToHashString(responseXmlBase64, secret);
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
                if(StatusCode == 0 && MacValidation == 2)
                {
                    OrderAccepted = false;
                    ErrorMessage = "Mac validation failed.";
                }
                else if (StatusCode == 0 && MacValidation <= 1)
                {
                    OrderAccepted = true;
                    ResultCode = "0 (ORDER_ACCEPTED)";
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
        public static Tuple<string, string> StatusCodeToMessage(int resultCode)
        {
            var resultCodeMessage = "";
            var errorMessage = "";


            switch (resultCode)
            {
                case 0:
                    resultCodeMessage = resultCode + " (OK)";
                    errorMessage =
                        "";
                    break;
                case 1:
                    resultCodeMessage = resultCode + " (REQUIRES_MANUAL_REVIEW)";
                    errorMessage =
                        "Request performed successfully but requires manual review from merchant. Applicable PaymentMethod: PAYPAL.";
                    break;
                case 100:
                    resultCodeMessage = resultCode + " (INTERNAL_ERROR)";
                    errorMessage = "Invalid – contact integrator.";
                    break;
                case 101:
                    resultCodeMessage = resultCode + " (XMLPARSEFAIL)";
                    errorMessage = "Invalid XML.";
                    break;
                case 102:
                    resultCodeMessage = resultCode + " (ILLEGAL_ENCODING)";
                    errorMessage = "Invalid encoding.";
                    break;
                case 104:
                    resultCodeMessage = resultCode + " (ILLEGAL_URL)";
                    errorMessage = "Illegal Url.";
                    break;
                case 105:
                    resultCodeMessage = resultCode + " (ILLEGAL_TRANSACTIONSTATUS)";
                    errorMessage = "Invalid transaction status.";
                    break;
                case 106:
                    resultCodeMessage = resultCode + " (EXTERNAL_ERROR)";
                    errorMessage = "Failure at third party e.g. failure at the bank.";
                    break;
                case 107:
                    resultCodeMessage = resultCode + " (DENIED_BY_BANK)";
                    errorMessage = "Transaction rejected by bank.";
                    break;
                case 108:
                    resultCodeMessage = resultCode + " (CANCELLED)";
                    errorMessage = "Transaction cancelled.";
                    break;
                case 109:
                    resultCodeMessage = resultCode + " (NOT_FOUND_AT_BANK)";
                    errorMessage = "Transaction not found at the bank.";
                    break;
                case 110:
                    resultCodeMessage = resultCode + " (ILLEGAL_TRANSACTIONID)";
                    errorMessage = "Invalid transaction ID.";
                    break;
                case 111:
                    resultCodeMessage = resultCode + " (MERCHANT_NOT_CONFIGURED)";
                    errorMessage = "Merchant not configured.";
                    break;
                case 112:
                    resultCodeMessage = resultCode + " (MERCHANT_NOT_CONFIGURED_AT_BANK)";
                    errorMessage = "Merchant not configured at the bank.";
                    break;
                case 113:
                    resultCodeMessage = resultCode + " (PAYMENTMETHOD_NOT_CONFIGURED)";
                    errorMessage = "Payment method not configured for merchant.";
                    break;
                case 114:
                    resultCodeMessage = resultCode + " (TIMEOUT_AT_BANK)";
                    errorMessage = "Timeout at the bank.";
                    break;
                case 115:
                    resultCodeMessage = resultCode + " (MERCHANT_NOT_ACTIVE)";
                    errorMessage = "The merchant is disabled.";
                    break;
                case 116:
                    resultCodeMessage = resultCode + " (PAYMENTMETHOD_NOT_ACTIVE)";
                    errorMessage = "The payment method is disabled.";
                    break;
                case 117:
                    resultCodeMessage = resultCode + " (ILLEGAL_AUTHORIZED_AMOUNT)";
                    errorMessage = "Invalid authorized amount.";
                    break;
                case 118:
                    resultCodeMessage = resultCode + " (ILLEGAL_CAPTURED_AMOUNT)";
                    errorMessage = "Invalid captured amount.";
                    break;
                case 119:
                    resultCodeMessage = resultCode + " (ILLEGAL_CREDITED_AMOUNT)";
                    errorMessage = "Invalid credited amount.";
                    break;
                case 120:
                    resultCodeMessage = resultCode + " (NOT_SUFFICIENT_FUNDS)";
                    errorMessage = "Not enough founds.";
                    break;
                case 121:
                    resultCodeMessage = resultCode + " (EXPIRED_CARD)";
                    errorMessage = "The card has expired.";
                    break;
                case 122:
                    resultCodeMessage = resultCode + " (STOLEN_CARD)";
                    errorMessage = "Stolen card.";
                    break;
                case 123:
                    resultCodeMessage = resultCode + " (LOST_CARD)";
                    errorMessage = "Lost card.";
                    break;
                case 124:
                    resultCodeMessage = resultCode + " (EXCEEDS_AMOUNT_LIMIT)";
                    errorMessage = "Amount exceeds the limit.";
                    break;
                case 125:
                    resultCodeMessage = resultCode + " (EXCEEDS_FREQUENCY_LIMIT)";
                    errorMessage = "Frequency limit exceeded.";
                    break;
                case 126:
                    resultCodeMessage = resultCode + " (TRANSACTION_NOT_BELONGING_TO_MERCHANT)";
                    errorMessage = "Transaction does not belong to merchant.";
                    break;
                case 127:
                    resultCodeMessage = resultCode + " (CUSTOMERREFNO_ALREADY_USED)";
                    errorMessage = "Customer reference number already used in another transaction.";
                    break;
                case 128:
                    resultCodeMessage = resultCode + " (NO_SUCH_TRANS)";
                    errorMessage = "Transaction does not exist.";
                    break;
                case 129:
                    resultCodeMessage = resultCode + " (DUPLICATE_TRANSACTION)";
                    errorMessage = "More than one transaction found for the given customer reference number.";
                    break;
                case 130:
                    resultCodeMessage = resultCode + " (ILLEGAL_OPERATION)";
                    errorMessage = "Operation not allowed for the given payment method.";
                    break;
                case 131:
                    resultCodeMessage = resultCode + " (COMPANY_NOT_ACTIVE)";
                    errorMessage = "Company inactive.";
                    break;
                case 132:
                    resultCodeMessage = resultCode + " (SUBSCRIPTION_NOT_FOUND)";
                    errorMessage = "No subscription exist.";
                    break;
                case 133:
                    resultCodeMessage = resultCode + " (SUBSCRIPTION_NOT_ACTIVE)";
                    errorMessage = "Subscription not active.";
                    break;
                case 134:
                    resultCodeMessage = resultCode + " (SUBSCRIPTION_NOT_SUPPORTED)";
                    errorMessage = "Payment method doesn’t support subscriptions.";
                    break;
                case 135:
                    resultCodeMessage = resultCode + " (ILLEGAL_DATE_FORMAT)";
                    errorMessage = "Illegal date format.";
                    break;
                case 136:
                    resultCodeMessage = resultCode + " (ILLEGAL_RESPONSE_DATA)";
                    errorMessage = "Illegal response data.";
                    break;
                case 137:
                    resultCodeMessage = resultCode + " (IGNORE_CALLBACK)";
                    errorMessage = "Ignore callback.";
                    break;
                case 138:
                    resultCodeMessage = resultCode + " (CURRENCY_NOT_CONFIGURED)";
                    errorMessage = "Currency not configured.";
                    break;
                case 139:
                    resultCodeMessage = resultCode + " (CURRENCY_NOT_ACTIVE)";
                    errorMessage = "Currency not active.";
                    break;
                case 140:
                    resultCodeMessage = resultCode + " (CURRENCY_ALREADY_CONFIGURED)";
                    errorMessage = "Currency is already configured.";
                    break;
                case 141:
                    resultCodeMessage = resultCode + " (ILLEGAL_AMOUNT_OF_RECURS_TODAY)";
                    errorMessage = "Ilegal amount of recurs per day.";
                    break;
                case 142:
                    resultCodeMessage = resultCode + " (NO_VALID_PAYMENT_METHODS)";
                    errorMessage = "No valid PaymentMethod.";
                    break;
                case 143:
                    resultCodeMessage = resultCode + " (CREDIT_DENIED_BY_BANK)";
                    errorMessage = "Credit denied by bank.";
                    break;
                case 144:
                    resultCodeMessage = resultCode + " (ILLEGAL_CREDIT_USER)";
                    errorMessage = "User is not allowed to perform credit operation.";
                    break;
                case 146:
                    resultCodeMessage = resultCode + " (CUSTOMER_NOT_FOUND)";
                    errorMessage = "Customer not found.";
                    break;
                case 147:
                    resultCodeMessage = resultCode + " (AGE_LIMIT_EXCEEDED)";
                    errorMessage = "E.g. the transaction is too old.";
                    break;
                case 148:
                    resultCodeMessage = resultCode + " (BROWSER_NOT_SUPPORTED)";
                    errorMessage = "Browser not supported.";
                    break;
                case 149:
                    resultCodeMessage = resultCode + " (PENDING)";
                    errorMessage = "Waiting for final Status code.";
                    break;
                case 150:
                    resultCodeMessage = resultCode + " (CREDIT_PENDING)";
                    errorMessage = "Credit is currently pending.";
                    break;
                case 300:
                    resultCodeMessage = resultCode + " (BAD_CARDHOLDER_NAME)";
                    errorMessage = "Invalid value for cardholder name.";
                    break;
                case 301:
                    resultCodeMessage = resultCode + " (BAD_TRANSACTION_ID)";
                    errorMessage = "Invalid value for transaction id.";
                    break;
                case 302:
                    resultCodeMessage = resultCode + " (BAD_REV)";
                    errorMessage = "Invalid value for rev.";
                    break;
                case 303:
                    resultCodeMessage = resultCode + " (BAD_MERCHANT_ID)";
                    errorMessage = "nvalid value for merchant id.";
                    break;
                case 304:
                    resultCodeMessage = resultCode + " (BAD_LANG)";
                    errorMessage = "Invalid value for lang.";
                    break;
                case 305:
                    resultCodeMessage = resultCode + " (BAD_AMOUNT)";
                    errorMessage = "Invalid value for amount.";
                    break;
                case 306:
                    resultCodeMessage = resultCode + " (BAD_CUSTOMERREFNO)";
                    errorMessage = "Invalid value for customer refno.";
                    break;
                case 307:
                    resultCodeMessage = resultCode + " (BAD_CURRENCY)";
                    errorMessage = "Invalid value for currency.";
                    break;
                case 308:
                    resultCodeMessage = resultCode + " (BAD_PAYMENTMETHOD)";
                    errorMessage = "Invalid value for payment method.";
                    break;
                case 309:
                    resultCodeMessage = resultCode + " (BAD_RETURNURL)";
                    errorMessage = "Invalid value for return url.";
                    break;
                case 310:
                    resultCodeMessage = resultCode + " (BAD_LASTBOOKINGDAY)";
                    errorMessage = "Invalid value for last booking day.";
                    break;
                case 311:
                    resultCodeMessage = resultCode + " (BAD_MAC)";
                    errorMessage = "Invalid value for mac.";
                    break;
                case 312:
                    resultCodeMessage = resultCode + " (BAD_TRNUMBER)";
                    errorMessage = "Invalid value for tr number.";
                    break;
                case 313:
                    resultCodeMessage = resultCode + " (BAD_AUTHCODE)";
                    errorMessage = "Invalid value for authcode.";
                    break;
                case 314:
                    resultCodeMessage = resultCode + " (BAD_CC_DESCR)";
                    errorMessage = "Invalid value for cc_descr.";
                    break;
                case 315:
                    resultCodeMessage = resultCode + " (BAD_ERROR_CODE)";
                    errorMessage = "Invalid value for error_code.";
                    break;
                case 316:
                    resultCodeMessage = resultCode + " (BAD_CARDNUMBER_OR_CARDTYPE_NOT_CONFIGURED)";
                    errorMessage = "Card type not configured for merchant.";
                    break;
                case 317:
                    resultCodeMessage = resultCode + " (BAD_SSN)";
                    errorMessage = "Invalid value for ssn.";
                    break;
                case 318:
                    resultCodeMessage = resultCode + " (BAD_VAT)";
                    errorMessage = "Invalid value for vat.";
                    break;
                case 319:
                    resultCodeMessage = resultCode + " (BAD_CAPTURE_DATE)";
                    errorMessage = "Invalid value for capture date.";
                    break;
                case 320:
                    resultCodeMessage = resultCode + " (BAD_CAMPAIGN_CODE_INVALID)";
                    errorMessage = "Invalid value for campaign code. There are no valid matching campaign codes.";
                    break;
                case 321:
                    resultCodeMessage = resultCode + " (BAD_SUBSCRIPTION_TYPE)";
                    errorMessage = "Invalid subscription type.";
                    break;
                case 322:
                    resultCodeMessage = resultCode + " (BAD_SUBSCRIPTION_ID)";
                    errorMessage = "Invalid subscription id.";
                    break;
                case 323:
                    resultCodeMessage = resultCode + " (BAD_BASE64)";
                    errorMessage = "Invalid base64.";
                    break;
                case 324:
                    resultCodeMessage = resultCode + " (BAD_CAMPAIGN_CODE)";
                    errorMessage = "Invalid campaign code. Missing value.";
                    break;
                case 325:
                    resultCodeMessage = resultCode + " (BAD_CALLBACKURL)";
                    errorMessage = "Invalid callbackurl.";
                    break;
                case 326:
                    resultCodeMessage = resultCode + " (THREE_D_CHECK_FAILED)";
                    errorMessage = "3D check failed.";
                    break;
                case 327:
                    resultCodeMessage = resultCode + " (CARD_NOT_ENROLLED)";
                    errorMessage = "Card not enrolled in 3D secure.";
                    break;
                case 328:
                    resultCodeMessage = resultCode + " (BAD_IPADDRESS)";
                    errorMessage = "Provided ip address is incorrect.";
                    break;
                case 329:
                    resultCodeMessage = resultCode + " (BAD_MOBILE)";
                    errorMessage = "Bad mobile phone number.";
                    break;
                case 330:
                    resultCodeMessage = resultCode + " (BAD_COUNTRY)";
                    errorMessage = "Bad country parameter.";
                    break;
                case 331:
                    resultCodeMessage = resultCode + " (THREE_D_CHECK_NOT_AVAILABLE)";
                    errorMessage = "Merchants 3D configuration invalid.";
                    break;
                case 332:
                    resultCodeMessage = resultCode + " (TIMEOUT)";
                    errorMessage = "Timeout at Svea.";
                    break;
                case 333:
                    resultCodeMessage = resultCode + " (BAD_PERIOD)";
                    errorMessage = "The reconciliation period is invalid.";
                    break;
                case 334:
                    resultCodeMessage = resultCode + " (BAD_ADDRESS_ID)";
                    errorMessage = "AddressSelector is not valid for this CountryCode.";
                    break;
                case 335:
                    resultCodeMessage = resultCode + " (BAD_CUSTOMER_DATA)";
                    errorMessage = "The supplied customer data is invalid.";
                    break;
                case 336:
                    resultCodeMessage = resultCode + " (BAD_UNIT)";
                    errorMessage = "Invalid unit.";
                    break;
                case 337:
                    resultCodeMessage = resultCode + " (BAD_EXTERNAL_PAYMENT_REF)";
                    errorMessage = "Invalid external payment reference.";
                    break;
                case 338:
                    resultCodeMessage = resultCode + " (BAD_STOREDCARDALIAS)";
                    errorMessage = "Invalid stored card alias.";
                    break;
                case 339:
                    resultCodeMessage = resultCode + " (STOREDCARDALIS_NOT_ACTIVE)";
                    errorMessage = "Stored card alias inactive.";
                    break;
                case 340:
                    resultCodeMessage = resultCode + " (STORED_CARDS_NOT_ENABLED)";
                    errorMessage = "This merchant does not have stored cards enabled.";
                    break;
                case 341:
                    resultCodeMessage = resultCode + " (ONLY_DEBIT_CARDS_ALLOWED)";
                    errorMessage = "Only debit cards allowed.";
                    break;     
                case 500:
                    resultCodeMessage = resultCode + " (ANTIFRAUD_CARDBIN_NOT_ALLOWED)";
                    errorMessage = "Antifraud - cardbin not allowed.";
                    break;
                case 501:
                    resultCodeMessage = resultCode + " (ANTIFRAUD_IPLOCATION_NOT_ALLOWED)";
                    errorMessage = "Antifraud – iplocation not allowed.";
                    break;
                case 502:
                    resultCodeMessage = resultCode + " (ANTIFRAUD_IPLOCATION_AND_BIN_DOESNT_MATCH)";
                    errorMessage = "Antifraud – ip-location and bin does not match.";
                    break;
                case 503:
                    resultCodeMessage = resultCode + " (ANTIFRAUD_MAX_AMOUNT_PER_IP_EXCEEDED)";
                    errorMessage = "Antifraud – max amount per ip exceeded.";
                    break;
                case 504:
                    resultCodeMessage = resultCode + " (ANTIFRAUD_MAX_TRANSACTIONS_PER_IP_EXCEEDED)";
                    errorMessage = "Antifraud – max transactions per ip exceeded.";
                    break;
                case 505:
                    resultCodeMessage = resultCode + " (ANTIFRAUD_MAX_TRANSACTIONS_PER_CARDNO_EXCEEDED)";
                    errorMessage = "Antifraud – max transactions per card number exceeded.";
                    break;
                case 506:
                    resultCodeMessage = resultCode + " (ANTIFRAUD_MAX_AMOUNT_PER_CARDNO_EXCEEDED)";
                    errorMessage = "Antifraud – max amount per cardnumber exceeded.";
                    break;
                case 507:
                    resultCodeMessage = resultCode + " (ANTIFRAUD_IP_ADDRESS_BLOCKED)";
                    errorMessage = "Antifraud – IP address blocked.";
                    break;
                default:
                    resultCodeMessage = resultCode + " (UNKNOWN_ERROR)";
                    errorMessage = "Unknown error.";
                    break;
            }

            Tuple<string, string> resultTuple = new Tuple<string, string>(resultCodeMessage, errorMessage);
            return resultTuple;
        }
    }
}