using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class DeliverOrderRowsRequest : WebpayAdminRequest
{
    private readonly DeliverOrderRowsBuilder _builder;

    public DeliverOrderRowsRequest(DeliverOrderRowsBuilder builder)
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

        var orderToDeliver = new DeliverOrderInformation()
        {
            ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
            SveaOrderId = _builder.Id,
            OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType)
            //PrintType // Optional for EU-clients, and integration package only supports EU-clients
        };

        var request = new PartialDeliveryRequest()
        {
            Authentication = auth,
            OrderToDeliver = orderToDeliver,
            RowNumbers = _builder.RowIndexesToDeliver.ToArray(),
            InvoiceDistributionType = ConvertDistributionTypeToInvoiceDistributionType(_builder.DistributionType)
        };

        var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
        var adminWS = new AdminServiceClient(AdminServiceClient.EndpointConfiguration.WcfAdminSoapService, endpoint);
        
        var response = await adminWS.DeliverPartialAsync(request);

        return response;
    }
}
