using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class CreditAmountRequest
{
    private readonly CreditOrderBuilder _builder;

    public CreditAmountRequest(CreditOrderBuilder builder)
    {
        _builder = builder;
    }

    public async Task<CancelPaymentPlanAmountResponse> DoRequest()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
        };

        var request = new CancelPaymentPlanAmountRequest()
        {
            Authentication = auth,
            AmountInclVat = _builder.AmountIncVat,
            ContractNumber = _builder.Id,
            ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
            Description = _builder.Description
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        var response = await adminWS.CancelPaymentPlanAmountAsync(request);

        return response;
    }
}
