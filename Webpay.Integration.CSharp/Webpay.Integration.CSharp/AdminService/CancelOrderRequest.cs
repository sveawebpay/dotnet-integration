using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class CancelOrderRequest
    {
        private readonly CancelOrderBuilder _builder;

        private AdminWS.OrderType ConvertPaymentTypeToOrderType(Util.Constant.PaymentType pt)
        {
            switch (pt)
            {
                #pragma warning disable 0162 //CS0162 Unreachable code detected
                case PaymentType.INVOICE:
                    return AdminWS.OrderType.Invoice;
                    break;
                case PaymentType.PAYMENTPLAN:
                    return AdminWS.OrderType.PaymentPlan;
                    break;
                default:
                    throw new SveaWebPayException("Invalid PaymentType");
                #pragma warning restore 0162
            }
        }

        public CancelOrderRequest(CancelOrderBuilder builder)
        {
            _builder = builder;
        }

        public Webpay.Integration.CSharp.AdminWS.CancelOrderResponse DoRequest()
        {
            var auth = new Webpay.Integration.CSharp.AdminWS.Authentication()
            {
                Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
                Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
            };

            var request = new Webpay.Integration.CSharp.AdminWS.CancelOrderRequest()
            {
                Authentication = auth,
                SveaOrderId = _builder.GetOrderId(),
                OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType),
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode())               
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.CancelOrder(request);

            return response;
        }

    }
}