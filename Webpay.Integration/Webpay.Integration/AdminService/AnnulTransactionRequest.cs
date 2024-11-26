using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Hosted.Admin.Actions;
using Webpay.Integration.Hosted.Admin.Response;
using Webpay.Integration.Order.Handle;

namespace Webpay.Integration.AdminService;

public class AnnulTransactionRequest
{
    private readonly CancelOrderBuilder _builder;

    public AnnulTransactionRequest(CancelOrderBuilder builder) {
        _builder = builder;
    }

    public async Task<AnnulResponse> DoRequest()
    {
        var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .Annul(new Annul(
                transactionId: _builder.Id,
                correlationId: _builder.GetCorrelationId()
                ));

        return await hostedActionRequest.DoRequest<AnnulResponse>();
    }
}