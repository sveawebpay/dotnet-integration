using System;
using System.Linq;
using System.Text.RegularExpressions;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Validator
{
    public class WebServiceOrderValidator : OrderValidator
    {
        public override string Validate(CreateOrderBuilder order)
        {
            if (order.GetCustomerIdentity() == null)
            {
                Errors += "MISSING VALUE - CustomerIdentity must be set.\n";
            }

            var identityValidator = new IdentityValidator();

            switch (order.GetCountryCode())
            {
                case CountryCode.NONE:
                    Errors += "MISSING VALUE - CountryCode is required. Use SetCountryCode().\n";
                    break;
                case CountryCode.FI:
                case CountryCode.DK:
                case CountryCode.NO:
                case CountryCode.SE:
                    Errors += identityValidator.ValidateNordicIdentity(order);
                    break;
                case CountryCode.DE:
                    Errors += identityValidator.ValidateDeIdentity(order);
                    break;
                case CountryCode.NL:
                    Errors += identityValidator.ValidateNlIdentity(order);
                    break;
                default:
                    Errors += "NOT VALID - Given countrycode does not exist in our system.\n";
                    break;
            }

            ValidateRequiredFieldsForOrder(order);
            ValidateOrderRow(order);
            if (order.GetOrderDate() == DateTime.MinValue)
            {
                Errors += "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            }

            if(!String.IsNullOrEmpty(order.GetPeppolId()))
            {
                Errors += ValidatePeppolId(order);
            }

            return Errors;
        }

        private static string ValidatePeppolId(CreateOrderBuilder order)
        {
            if (order.GetPeppolId().Length < 6)
            {
                return "NOT VALID - PeppolId is too short, must be 6 characters or longer.";
            }

            if (order.GetPeppolId().Length > 55)
            {
                return "NOT VALID - PeppolId is too long, must be 55 characters or fewer.";
            }

            if (Regex.IsMatch(order.GetPeppolId().Substring(0, 4), @"^\d+$") == false)
            {
                return "NOT VALID - First 4 characters of PeppolId must be numeric.";
            }

            if (order.GetPeppolId().Substring(4,1) != ":")
            {
                return "NOT VALID - The fifth character of PeppolId must be \":\"";
            }

            if (order.GetPeppolId().Substring(5).All(char.IsLetterOrDigit) == false)
            {
                return "NOT VALID - All characters after the fifth character in PeppolId must be alphanumeric.";
            }

            if (order.GetIsCompanyIdentity() == false)
            {
                return "NOT VALID - CustomerType must be a company when using PeppolId.";
            }

            return "";
        }
    }
}