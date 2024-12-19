using Webpay.Integration.Order.Create;

namespace Webpay.Integration.Order.Validator;

internal interface IOrderValidator
{
    string Validate(CreateOrderBuilder order);
}