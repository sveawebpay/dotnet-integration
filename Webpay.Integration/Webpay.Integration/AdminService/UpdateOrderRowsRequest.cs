using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class UpdateOrderRowsRequest : WebpayAdminRequest
{
    private readonly UpdateOrderRowsBuilder _builder;

    public UpdateOrderRowsRequest(UpdateOrderRowsBuilder builder)
    {
        _builder = builder;
    }

    public async Task<UpdateOrderRowsResponse> DoRequestAsync()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())                
        };

        var request = new AdminWS.UpdateOrderRowsRequest()
        {
            Authentication = auth,
            SveaOrderId = _builder.Id,
            OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType),
            ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
            UpdatedOrderRows = _builder.NumberedOrderRows.Select(ConvertNumberedOrderRowBuilderToAdminWSNumberedOrderRow).ToArray()
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        var response = await adminWS.UpdateOrderRowsAsync(request);

        return response;
    }
}
