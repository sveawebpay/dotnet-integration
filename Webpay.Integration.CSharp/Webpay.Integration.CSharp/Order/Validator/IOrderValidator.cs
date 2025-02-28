using Webpay.Integration.CSharp.Order.Create;

namespace Webpay.Integration.CSharp.Order.Validator
{
    internal interface IOrderValidator
    {
        string Validate(CreateOrderBuilder order);
    }
}