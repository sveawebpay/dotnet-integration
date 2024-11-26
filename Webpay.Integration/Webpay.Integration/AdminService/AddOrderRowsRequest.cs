using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class AddOrderRowsRequest : WebpayAdminRequest
{
    private readonly AddOrderRowsBuilder _builder;

    public AddOrderRowsRequest(AddOrderRowsBuilder builder)
    {
        _builder = builder;
    }

    public async Task<AddOrderRowsResponse> DoRequest()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
        };

        var request = new AdminWS.AddOrderRowsRequest()
        {
            Authentication = auth,
            SveaOrderId = _builder.Id,
            OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType),
            ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
            OrderRows = _builder.OrderRows.Select(x => ConvertOrderRowBuilderToAdminWSOrderRow(x)).ToArray()
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        var response = await adminWS.AddOrderRowsAsync(request);

        return response;
    }
}