using System.Text.RegularExpressions;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Validator;

public class WebServiceOrderValidator : OrderValidator
{
    public override string Validate(CreateOrderBuilder order)
    {
        if (order.GetCustomerIdentity() == null)
        {
            Errors += "MISSING VALUE - CustomerIdentity must be set.\n";
        }

        var identityValidator = new IdentityValidator();

        Errors += order.GetCountryCode() switch
        {
            CountryCode.NONE => "MISSING VALUE - CountryCode is required. Use SetCountryCode().\n",
            CountryCode.FI or CountryCode.DK or CountryCode.NO or CountryCode.SE => identityValidator.ValidateNordicIdentity(order),
            CountryCode.DE => identityValidator.ValidateDeIdentity(order),
            CountryCode.NL => identityValidator.ValidateNlIdentity(order),
            _ => "NOT VALID - Given countrycode does not exist in our system.\n"
        };

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
        string validation = ValidatePeppolIdString(order.GetPeppolId());

        if (validation != "")
        {
            return validation;
        }

        if (order.GetIsCompanyIdentity() == false)
        {
            return "NOT VALID - CustomerType must be a company when using PeppolId.";
        }

        return "";
    }

    public static string ValidatePeppolIdString(string peppolId) =>
        peppolId.Length switch
        {
            < 6 => "NOT VALID - PeppolId is too short, must be 6 characters or longer.",
            > 55 => "NOT VALID - PeppolId is too long, must be 55 characters or fewer.",
            _ when !Regex.IsMatch(peppolId.Substring(0, 4), @"^\d+$") => "NOT VALID - First 4 characters of PeppolId must be numeric.",
            _ when peppolId[4] != ':' => "NOT VALID - The fifth character of PeppolId must be \":\"",
            _ when !peppolId.Substring(5).All(char.IsLetterOrDigit) => "NOT VALID - All characters after the fifth character in PeppolId must be alphanumeric.",
            _ => ""
        };
}