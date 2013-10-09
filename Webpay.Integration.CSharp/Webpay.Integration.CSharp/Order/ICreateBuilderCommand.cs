using Webpay.Integration.CSharp.Order.Create;

namespace Webpay.Integration.CSharp.Order
{
    public interface ICreateBuilderCommand
    {
        CreateOrderBuilder Run(CreateOrderBuilder order);
    }
}
