using Webpay.Integration.CSharp.Order.Create;

namespace Webpay.Integration.CSharp.Order.Validator
{
    public class HostedOrderValidator : OrderValidator
    {
        public override string Validate(CreateOrderBuilder order)
        {
            Errors = "";

            if (order.GetCountryCode() == 0)
                Errors += "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n";
            ValidateClientOrderNumber(order);
            ValidateCurrency(order);
            ValidateRequiredFieldsForOrder(order);
            ValidateOrderRow(order);
 
            return Errors;
        }

        private void ValidateCurrency(CreateOrderBuilder order)
        {
            if (order.GetCurrency() == null)
                Errors += "MISSING VALUE - Currency is required. Use SetCurrency(...).\n";
        }

        private void ValidateClientOrderNumber(CreateOrderBuilder order)
        {
            if (order.GetClientOrderNumber() == null)
            {
                Errors += "MISSING VALUE - ClientOrderNumber is required. Use SetClientOrderNumber(...).\n";
            }
            else if ((order.GetClientOrderNumber() != null && "".Equals(order.GetClientOrderNumber())))
                Errors += "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n";
        }
    }
}
