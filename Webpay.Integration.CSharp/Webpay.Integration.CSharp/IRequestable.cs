namespace Webpay.Integration.CSharp
{
    public interface IRequestable
    { 
        IRequestable PrepareRequest<T>();

        IRespondable DoRequest<T>();
    }

    public interface IRespondable
    {
    }
}