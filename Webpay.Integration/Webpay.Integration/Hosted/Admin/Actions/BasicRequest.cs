namespace Webpay.Integration.Hosted.Admin.Actions;

public class BasicRequest
{
    public readonly Guid? CorrelationId;

    public BasicRequest(Guid? correlationId )
    {
        CorrelationId = correlationId;
    }
}
