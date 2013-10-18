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
            if (string.IsNullOrEmpty(order.GetOrderDate()))
            {
                Errors += "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            }

            return Errors;
        }
    }
}