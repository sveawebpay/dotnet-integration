using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Hosted.Admin.Actions;
using Webpay.Integration.Hosted.Admin.Response;
using Webpay.Integration.Order.Handle;

namespace Webpay.Integration.AdminService;

public class LowerOrderRowRequest : WebpayAdminRequest
{
    private readonly LowerOrderRowBuilder _builder;

    public LowerOrderRowRequest(LowerOrderRowBuilder builder) {
        _builder = builder;
    }

    public async Task<LowerOrderRowResponse> DoRequestAsync()
    {
        // lower order by calculated amount
        var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
        .LowerOrderRow(new LowerOrderRow(
            transactionId: _builder.Id,
            orderRows:_builder.OrderRows,
            correlationId:_builder.GetCorrelationId()));

        return await hostedActionRequest.DoRequestAsync<LowerOrderRowResponse>();
    }
}
