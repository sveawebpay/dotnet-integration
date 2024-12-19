namespace Webpay.Integration.Exception;

[Serializable]
public class SveaWebPayValidationException : SveaWebPayException
{
    public SveaWebPayValidationException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }

    public SveaWebPayValidationException(string message)
        : base(message, null)
    {
    }
}