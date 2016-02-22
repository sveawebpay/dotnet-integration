using System;
using System.Globalization;
using System.Xml;
using Webpay.Integration.CSharp.Response.Hosted;
using Webpay.Integration.CSharp.Util.Calculation;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public abstract class SpecificHostedAdminResponseBase
    {
        protected SpecificHostedAdminResponseBase(XmlNode response)
        {
            StatusCode = TextInt(response, "/response/statuscode").GetValueOrDefault(101);
            Accepted = StatusCode == 0;
            var errorMessage = SveaResponse.StatusCodeToMessage(StatusCode);
            ErrorMessage = errorMessage.Item2;
        }

        public int StatusCode { get; private set; }
        public bool Accepted { get; private set; }
        public string ErrorMessage { get; private set; }

        protected static long? AttributeLong(XmlNode response, string element, string attribute)
        {
            try
            {
                return Convert.ToInt64(response.SelectSingleNode(element).Attributes[attribute].Value);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        protected static string AttributeString(XmlNode response, string element, string attribute)
        {
            try
            {
                return response.SelectSingleNode(element).Attributes[attribute].Value;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        protected static int? TextInt(XmlNode response, string element)
        {
            try
            {
                return Convert.ToInt32(response.SelectSingleNode(element).InnerText);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        protected static decimal? TextDecimal(XmlNode response, string element)
        {
            decimal parsedDecimal;

            return Decimal.TryParse(response.SelectSingleNode(element).InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsedDecimal)
                ? parsedDecimal
                : (decimal?)null;
        }



        protected static string TextString(XmlNode response, string element)
        {
            try
            {
                return response.SelectSingleNode(element).InnerText;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static decimal? MinorCurrencyToDecimalAmount(int? amountMinor)
        {
            return amountMinor.HasValue ? MathUtil.BankersRound(((decimal)amountMinor) / 100) : (decimal?) null;
        }

        public static decimal MinorCurrencyToDecimalAmount(int amountMinor)
        {
            return MathUtil.BankersRound(((decimal)amountMinor) / 100);
        }
    }
}