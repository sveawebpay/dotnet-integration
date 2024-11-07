using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class UpdateOrderRequest : WebpayAdminRequest
{
    private readonly UpdateOrderBuilder _builder;

    public UpdateOrderRequest(UpdateOrderBuilder builder)
    {
        _builder = builder;
    }

    public async Task<UpdateOrderResponse> DoRequest()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
        };

        var request = new AdminWS.UpdateOrderRequest()
        {
            Authentication = auth,
            SveaOrderId = _builder.SveaOrderId,
            OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType),
            ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
            Notes = _builder.Notes,
            ClientOrderNumber = _builder.ClientOrderNumber
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        var response = await adminWS.UpdateOrderAsync(request);

        return response;
    }
}
