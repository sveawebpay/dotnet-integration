using System;
using System.Xml;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Response.Hosted
{
    public class SveaResponse : Response
    {
        public readonly SveaConfig Config = new SveaConfig();

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

        /// <summary>
        /// SveaResponse
        /// </summary>
        /// <param name="responseXmlBase64"></param>
        /// <exception cref="Exception"></exception>
        public SveaResponse(string responseXmlBase64)
        {
            SetValues(responseXmlBase64);
        }

        private void SetValues(string xmlBase64)
        {
            Xml = Base64Util.DecodeBase64String(xmlBase64);

            var d1 = new XmlDocument();
            d1.LoadXml(Xml);

            foreach (XmlElement element in d1.GetElementsByTagName("response"))
            {
                var status = int.Parse(GetTagValue(element, "statuscode"));
                if (status == 0)
                {
                    OrderAccepted = true;
                    ResultCode = "0 (ORDER_ACCEPTED)";
                }
                else
                {
                    OrderAccepted = false;
                    SetErrorParams(status);
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
            switch (resultCode)
            {
                case 1:
                    ResultCode = resultCode + " (REQUIRES_MANUAL_REVIEW)";
                    ErrorMessage =
                        "Request performed successfully but requires manual review from merchant. Applicable PaymentMethod: PAYPAL.";
                    break;
                case 100:
                    ResultCode = resultCode + " (INTERNAL_ERROR)";
                    ErrorMessage = "Invalid – contact integrator.";
                    break;
                case 101:
                    ResultCode = resultCode + " (XMLPARSEFAIL)";
                    ErrorMessage = "Invalid XML.";
                    break;
                case 102:
                    ResultCode = resultCode + " (ILLEGAL_ENCODING)";
                    ErrorMessage = "Invalid encoding.";
                    break;
                case 104:
                    ResultCode = resultCode + " (ILLEGAL_URL)";
                    ErrorMessage = "Illegal Url.";
                    break;
                case 105:
                    ResultCode = resultCode + " (ILLEGAL_TRANSACTIONSTATUS)";
                    ErrorMessage = "Invalid transaction status.";
                    break;
                case 106:
                    ResultCode = resultCode + " (EXTERNAL_ERROR)";
                    ErrorMessage = "Failure at third party e.g. failure at the bank.";
                    break;
                case 107:
                    ResultCode = resultCode + " (DENIED_BY_BANK)";
                    ErrorMessage = "Transaction rejected by bank.";
                    break;
                case 108:
                    ResultCode = resultCode + " (CANCELLED)";
                    ErrorMessage = "Transaction cancelled.";
                    break;
                case 109:
                    ResultCode = resultCode + " (NOT_FOUND_AT_BANK)";
                    ErrorMessage = "Transaction not found at the bank.";
                    break;
                case 110:
                    ResultCode = resultCode + " (ILLEGAL_TRANSACTIONID)";
                    ErrorMessage = "Invalid transaction ID.";
                    break;
                case 111:
                    ResultCode = resultCode + " (MERCHANT_NOT_CONFIGURED)";
                    ErrorMessage = "Merchant not configured.";
                    break;
                case 112:
                    ResultCode = resultCode + " (MERCHANT_NOT_CONFIGURED_AT_BANK)";
                    ErrorMessage = "Merchant not configured at the bank.";
                    break;
                case 113:
                    ResultCode = resultCode + " (PAYMENTMETHOD_NOT_CONFIGURED)";
                    ErrorMessage = "Payment method not configured for merchant.";
                    break;
                case 114:
                    ResultCode = resultCode + " (TIMEOUT_AT_BANK)";
                    ErrorMessage = "Timeout at the bank.";
                    break;
                case 115:
                    ResultCode = resultCode + " (MERCHANT_NOT_ACTIVE)";
                    ErrorMessage = "The merchant is disabled.";
                    break;
                case 116:
                    ResultCode = resultCode + " (PAYMENTMETHOD_NOT_ACTIVE)";
                    ErrorMessage = "The payment method is disabled.";
                    break;
                case 117:
                    ResultCode = resultCode + " (ILLEGAL_AUTHORIZED_AMOUNT)";
                    ErrorMessage = "Invalid authorized amount.";
                    break;
                case 118:
                    ResultCode = resultCode + " (ILLEGAL_CAPTURED_AMOUNT)";
                    ErrorMessage = "Invalid captured amount.";
                    break;
                case 119:
                    ResultCode = resultCode + " (ILLEGAL_CREDITED_AMOUNT)";
                    ErrorMessage = "Invalid credited amount.";
                    break;
                case 120:
                    ResultCode = resultCode + " (NOT_SUFFICIENT_FUNDS)";
                    ErrorMessage = "Not enough founds.";
                    break;
                case 121:
                    ResultCode = resultCode + " (EXPIRED_CARD)";
                    ErrorMessage = "The card has expired.";
                    break;
                case 122:
                    ResultCode = resultCode + " (STOLEN_CARD)";
                    ErrorMessage = "Stolen card.";
                    break;
                case 123:
                    ResultCode = resultCode + " (LOST_CARD)";
                    ErrorMessage = "Lost card.";
                    break;
                case 124:
                    ResultCode = resultCode + " (EXCEEDS_AMOUNT_LIMIT)";
                    ErrorMessage = "Amount exceeds the limit.";
                    break;
                case 125:
                    ResultCode = resultCode + " (EXCEEDS_FREQUENCY_LIMIT)";
                    ErrorMessage = "Frequency limit exceeded.";
                    break;
                case 126:
                    ResultCode = resultCode + " (TRANSACTION_NOT_BELONGING_TO_MERCHANT)";
                    ErrorMessage = "Transaction does not belong to merchant.";
                    break;
                case 127:
                    ResultCode = resultCode + " (CUSTOMERREFNO_ALREADY_USED)";
                    ErrorMessage = "Customer reference number already used in another transaction.";
                    break;
                case 128:
                    ResultCode = resultCode + " (NO_SUCH_TRANS)";
                    ErrorMessage = "Transaction does not exist.";
                    break;
                case 129:
                    ResultCode = resultCode + " (DUPLICATE_TRANSACTION)";
                    ErrorMessage = "More than one transaction found for the given customer reference number.";
                    break;
                case 130:
                    ResultCode = resultCode + " (ILLEGAL_OPERATION)";
                    ErrorMessage = "Operation not allowed for the given payment method.";
                    break;
                case 131:
                    ResultCode = resultCode + " (COMPANY_NOT_ACTIVE)";
                    ErrorMessage = "Company inactive.";
                    break;
                case 132:
                    ResultCode = resultCode + " (SUBSCRIPTION_NOT_FOUND)";
                    ErrorMessage = "No subscription exist.";
                    break;
                case 133:
                    ResultCode = resultCode + " (SUBSCRIPTION_NOT_ACTIVE)";
                    ErrorMessage = "Subscription not active.";
                    break;
                case 134:
                    ResultCode = resultCode + " (SUBSCRIPTION_NOT_SUPPORTED)";
                    ErrorMessage = "Payment method doesn’t support subscriptions.";
                    break;
                case 135:
                    ResultCode = resultCode + " (ILLEGAL_DATE_FORMAT)";
                    ErrorMessage = "Illegal date format.";
                    break;
                case 136:
                    ResultCode = resultCode + " (ILLEGAL_RESPONSE_DATA)";
                    ErrorMessage = "Illegal response data.";
                    break;
                case 137:
                    ResultCode = resultCode + " (IGNORE_CALLBACK)";
                    ErrorMessage = "Ignore callback.";
                    break;
                case 138:
                    ResultCode = resultCode + " (CURRENCY_NOT_CONFIGURED)";
                    ErrorMessage = "Currency not configured.";
                    break;
                case 139:
                    ResultCode = resultCode + " (CURRENCY_NOT_ACTIVE)";
                    ErrorMessage = "Currency not active.";
                    break;
                case 140:
                    ResultCode = resultCode + " (CURRENCY_ALREADY_CONFIGURED)";
                    ErrorMessage = "Currency is already configured.";
                    break;
                case 141:
                    ResultCode = resultCode + " (ILLEGAL_AMOUNT_OF_RECURS_TODAY)";
                    ErrorMessage = "Ilegal amount of recurs per day.";
                    break;
                case 142:
                    ResultCode = resultCode + " (NO_VALID_PAYMENT_METHODS)";
                    ErrorMessage = "No valid PaymentMethod.";
                    break;
                case 143:
                    ResultCode = resultCode + " (CREDIT_DENIED_BY_BANK)";
                    ErrorMessage = "Credit denied by bank.";
                    break;
                case 144:
                    ResultCode = resultCode + " (ILLEGAL_CREDIT_USER)";
                    ErrorMessage = "User is not allowed to perform credit operation.";
                    break;
                case 300:
                    ResultCode = resultCode + " (BAD_CARDHOLDER_NAME)";
                    ErrorMessage = "Invalid value for cardholder name.";
                    break;
                case 301:
                    ResultCode = resultCode + " (BAD_TRANSACTION_ID)";
                    ErrorMessage = "Invalid value for transaction id.";
                    break;
                case 302:
                    ResultCode = resultCode + " (BAD_REV)";
                    ErrorMessage = "Invalid value for rev.";
                    break;
                case 303:
                    ResultCode = resultCode + " (BAD_MERCHANT_ID)";
                    ErrorMessage = "nvalid value for merchant id.";
                    break;
                case 304:
                    ResultCode = resultCode + " (BAD_LANG)";
                    ErrorMessage = "Invalid value for lang.";
                    break;
                case 305:
                    ResultCode = resultCode + " (BAD_AMOUNT)";
                    ErrorMessage = "Invalid value for amount.";
                    break;
                case 306:
                    ResultCode = resultCode + " (BAD_CUSTOMERREFNO)";
                    ErrorMessage = "Invalid value for customer refno.";
                    break;
                case 307:
                    ResultCode = resultCode + " (BAD_CURRENCY)";
                    ErrorMessage = "Invalid value for currency.";
                    break;
                case 308:
                    ResultCode = resultCode + " (BAD_PAYMENTMETHOD)";
                    ErrorMessage = "Invalid value for payment method.";
                    break;
                case 309:
                    ResultCode = resultCode + " (BAD_RETURNURL)";
                    ErrorMessage = "Invalid value for return url.";
                    break;
                case 310:
                    ResultCode = resultCode + " (BAD_LASTBOOKINGDAY)";
                    ErrorMessage = "Invalid value for last booking day.";
                    break;
                case 311:
                    ResultCode = resultCode + " (BAD_MAC)";
                    ErrorMessage = "Invalid value for mac.";
                    break;
                case 312:
                    ResultCode = resultCode + " (BAD_TRNUMBER)";
                    ErrorMessage = "Invalid value for tr number.";
                    break;
                case 313:
                    ResultCode = resultCode + " (BAD_AUTHCODE)";
                    ErrorMessage = "Invalid value for authcode.";
                    break;
                case 314:
                    ResultCode = resultCode + " (BAD_CC_DESCR)";
                    ErrorMessage = "Invalid value for cc_descr.";
                    break;
                case 315:
                    ResultCode = resultCode + " (BAD_ERROR_CODE)";
                    ErrorMessage = "Invalid value for error_code.";
                    break;
                case 316:
                    ResultCode = resultCode + " (BAD_CARDNUMBER_OR_CARDTYPE_NOT_CONFIGURED)";
                    ErrorMessage = "Card type not configured for merchant.";
                    break;
                case 317:
                    ResultCode = resultCode + " (BAD_SSN)";
                    ErrorMessage = "Invalid value for ssn.";
                    break;
                case 318:
                    ResultCode = resultCode + " (BAD_VAT)";
                    ErrorMessage = "Invalid value for vat.";
                    break;
                case 319:
                    ResultCode = resultCode + " (BAD_CAPTURE_DATE)";
                    ErrorMessage = "Invalid value for capture date.";
                    break;
                case 320:
                    ResultCode = resultCode + " (BAD_CAMPAIGN_CODE_INVALID)";
                    ErrorMessage = "Invalid value for campaign code. There are no valid matching campaign codes.";
                    break;
                case 321:
                    ResultCode = resultCode + " (BAD_SUBSCRIPTION_TYPE)";
                    ErrorMessage = "Invalid subscription type.";
                    break;
                case 322:
                    ResultCode = resultCode + " (BAD_SUBSCRIPTION_ID)";
                    ErrorMessage = "Invalid subscription id.";
                    break;
                case 323:
                    ResultCode = resultCode + " (BAD_BASE64)";
                    ErrorMessage = "Invalid base64.";
                    break;
                case 324:
                    ResultCode = resultCode + " (BAD_CAMPAIGN_CODE)";
                    ErrorMessage = "Invalid campaign code. Missing value.";
                    break;
                case 325:
                    ResultCode = resultCode + " (BAD_CALLBACKURL)";
                    ErrorMessage = "Invalid callbackurl.";
                    break;
                case 326:
                    ResultCode = resultCode + " (THREE_D_CHECK_FAILED)";
                    ErrorMessage = "3D check failed.";
                    break;
                case 327:
                    ResultCode = resultCode + " (CARD_NOT_ENROLLED)";
                    ErrorMessage = "Card not enrolled in 3D secure.";
                    break;
                case 328:
                    ResultCode = resultCode + " (BAD_IPADDRESS)";
                    ErrorMessage = "Provided ip address is incorrect.";
                    break;
                case 329:
                    ResultCode = resultCode + " (BAD_MOBILE)";
                    ErrorMessage = "Bad mobile phone number.";
                    break;
                case 330:
                    ResultCode = resultCode + " (BAD_COUNTRY)";
                    ErrorMessage = "Bad country parameter.";
                    break;
                case 331:
                    ResultCode = resultCode + " (THREE_D_CHECK_NOT_AVAILABLE)";
                    ErrorMessage = "Merchants 3D configuration invalid.";
                    break;
                case 332:
                    ResultCode = resultCode + " (TIMEOUT)";
                    ErrorMessage = "Timeout at Svea.";
                    break;
                case 500:
                    ResultCode = resultCode + " (ANTIFRAUD_CARDBIN_NOT_ALLOWED)";
                    ErrorMessage = "Antifraud - cardbin not allowed.";
                    break;
                case 501:
                    ResultCode = resultCode + " (ANTIFRAUD_IPLOCATION_NOT_ALLOWED)";
                    ErrorMessage = "Antifraud – iplocation not allowed.";
                    break;
                case 502:
                    ResultCode = resultCode + " (ANTIFRAUD_IPLOCATION_AND_BIN_DOESNT_MATCH)";
                    ErrorMessage = "Antifraud – ip-location and bin does not match.";
                    break;
                case 503:
                    ResultCode = resultCode + " (ANTIFRAUD_MAX_AMOUNT_PER_IP_EXCEEDED)";
                    ErrorMessage = "Antofraud – max amount per ip exceeded.";
                    break;
                case 504:
                    ResultCode = resultCode + " (ANTIFRAUD_MAX_TRANSACTIONS_PER_IP_EXCEEDED)";
                    ErrorMessage = "Antifraud – max transactions per ip exceeded.";
                    break;
                case 505:
                    ResultCode = resultCode + " (ANTIFRAUD_MAX_TRANSACTIONS_PER_CARDNO_EXCEEDED)";
                    ErrorMessage = "Antifraud – max transactions per card number exceeded.";
                    break;
                case 506:
                    ResultCode = resultCode + " (ANTIFRAUD_MAX_AMOUNT_PER_CARDNO_EXCEEDED)";
                    ErrorMessage = "Antifraud – max amount per cardnumer exceeded.";
                    break;
                case 507:
                    ResultCode = resultCode + " (ANTIFRAUD_IP_ADDRESS_BLOCKED)";
                    ErrorMessage = "Antifraud – IP address blocked.";
                    break;
                default:
                    ResultCode = resultCode + " (UNKNOWN_ERROR)";
                    ErrorMessage = "Unknown error.";
                    break;
            }
        }
    }
}