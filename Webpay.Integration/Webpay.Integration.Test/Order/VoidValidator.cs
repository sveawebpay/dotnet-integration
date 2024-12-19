using Webpay.Integration.Order.Create;
using Webpay.Integration.Order.Validator;

namespace Webpay.Integration.Test.Order;

internal class VoidValidator : OrderValidator
{
    public int NoOfCalls;

    public int GetNoOfCalls()
    {
        return NoOfCalls;
    }

    public override string Validate(CreateOrderBuilder order)
    {
        Errors = "";
        NoOfCalls++;
        return "";
    }
}