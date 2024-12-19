using Webpay.Integration.Order.Create;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Validator;

public class HostedOrderValidator : OrderValidator
{
    public override string Validate(CreateOrderBuilder order)
    {
        Errors = "";

        if (order.GetCountryCode() == CountryCode.NONE)
        {
            Errors += "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n";
        }
        ValidateClientOrderNumber(order);
        ValidateCurrency(order);
        ValidateRequiredFieldsForOrder(order);
        ValidateOrderRow(order);

        return Errors;
    }

    private void ValidateCurrency(CreateOrderBuilder order)
    {
        if (order.GetCurrency() == null)
        {
            Errors += "MISSING VALUE - Currency is required. Use SetCurrency(...).\n";
        }
    }

    private void ValidateClientOrderNumber(CreateOrderBuilder order)
    {
        if (order.GetClientOrderNumber() == null)
        {
            Errors += "MISSING VALUE - ClientOrderNumber is required. Use SetClientOrderNumber(...).\n";
        }
        else if (order.GetClientOrderNumber().Trim().Length == 0)
        {
            Errors +=
                "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n";
        }
    }
}