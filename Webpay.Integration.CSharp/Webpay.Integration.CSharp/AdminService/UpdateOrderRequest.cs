using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class UpdateOrderRequest : WebpayAdminRequest
    {
        private readonly UpdateOrderBuilder _builder;

        public UpdateOrderRequest(UpdateOrderBuilder builder)
        {
            _builder = builder;
        }

        public Webpay.Integration.CSharp.AdminWS.UpdateOrderResponse DoRequest()
        {
            var auth = new AdminWS.Authentication()
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

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.UpdateOrder(request);

            return response;
        }
    }
}