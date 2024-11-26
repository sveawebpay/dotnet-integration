using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Hosted.Admin.Actions;
using Webpay.Integration.Hosted.Admin.Response;
using Webpay.Integration.Order.Handle;

namespace Webpay.Integration.AdminService;

public class QueryTransactionRequest
{
    readonly QueryOrderBuilder _builder;

    public QueryTransactionRequest(QueryOrderBuilder builder) {
        _builder = builder;
    }

    public async Task<QueryResponse> DoRequest()
    {
        var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .Query(new QueryByTransactionId(
                transactionId: _builder.Id,
                correlationId: _builder.GetCorrelationId()
                ));

        return await hostedActionRequest.DoRequest<QueryResponse>();
    }
}