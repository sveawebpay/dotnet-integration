using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class CancelOrderRequest : WebpayAdminRequest
{
    private readonly CancelOrderBuilder _builder;

    public CancelOrderRequest(CancelOrderBuilder builder)
    {
        _builder = builder;
    }

    public async Task<CancelOrderResponse> DoRequestAsync()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
        };

        var request = new AdminWS.CancelOrderRequest()
        {
            Authentication = auth,
            SveaOrderId = _builder.Id,
            OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType),
            ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode())               
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        var response = await adminWS.CancelOrderAsync(request);

        return response;
    }
}
