using Webpay.Integration.CSharp.Order.Create;

namespace Webpay.Integration.CSharp.Order.Validator
{
    interface IOrderValidator
    {
        string Validate(CreateOrderBuilder order);
    }
}
