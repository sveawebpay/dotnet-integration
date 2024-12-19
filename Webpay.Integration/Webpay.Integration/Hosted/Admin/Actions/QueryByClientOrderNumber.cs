namespace Webpay.Integration.Hosted.Admin.Actions;

/// <summary>
///     Convenience class if you use ClientOrderNumber instead of CustomerRefNo in your code
/// </summary>
public class QueryByClientOrderNumber : QueryByCustomerRefNo
{
    public QueryByClientOrderNumber(string clientOrderNumber, Guid? correlationId) : base(clientOrderNumber, correlationId)
    {
    }
}