namespace Webpay.Integration.CSharp.Order
{
    public interface IBuilderCommand<T>
    {
        T Run(T order);
    }
}