using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Hosted.Admin.Actions;
using Webpay.Integration.Hosted.Admin.Response;
using Webpay.Integration.Order.Handle;

namespace Webpay.Integration.AdminService;

public class CreditTransactionRequest
{
    private readonly CreditOrderBuilder _builder;

    public CreditTransactionRequest(CreditOrderBuilder builder)
    {
        _builder = builder;
    }

    public async Task<CreditResponse> DoRequestAsync()
    {
        var creditRequest = new Credit(
                transactionId: _builder.Id,
                amountToCredit: Decimal.ToInt64(_builder.AmountIncVat * 100),
                deliveries: _builder.Deliveries,
                correlationId: _builder.GetCorrelationId());

        CreditResponse validationResoponse;

        if (creditRequest.ValidateCreditRequest(out validationResoponse))
        {
            var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .Credit(creditRequest);
            return await hostedActionRequest.DoRequestAsync<CreditResponse>();
        }

        return validationResoponse;
    }
}