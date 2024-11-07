using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class DeliverOrdersRequest : WebpayAdminRequest
{
    private readonly DeliverOrdersBuilder _builder;

    public DeliverOrdersRequest(DeliverOrdersBuilder builder)
    {
        _builder = builder;
    }

    public async Task<DeliveryResponse> DoRequest()
    {
        var auth = new Authentication()
        {
            Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
            Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
        };

        var ordersToDeliver = new List<DeliverOrderInformation>();
        var clientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode());
        var orderType = ConvertPaymentTypeToOrderType(_builder.OrderType);
        foreach (var orderId in _builder.OrderIds)
        {
            var orderToDeliver = new DeliverOrderInformation()
            {
                ClientId = clientId,
                SveaOrderId = orderId,
                OrderType = orderType
                //PrintType // optional for EU-clients
            };
            ordersToDeliver.Add(orderToDeliver);
        }

        var request = new DeliveryRequest()
        {
            Authentication = auth,
            OrdersToDeliver = ordersToDeliver.ToArray(),
            InvoiceDistributionType = ConvertDistributionTypeToInvoiceDistributionType(_builder.DistributionType)
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        
        var response = await adminWS.DeliverOrdersAsync(request);

        return response;
    }
}
