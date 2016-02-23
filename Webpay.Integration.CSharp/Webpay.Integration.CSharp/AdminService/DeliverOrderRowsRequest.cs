using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class DeliverOrderRowsRequest : WebpayAdminRequest
    {
        private readonly DeliverOrderRowsBuilder _builder;

        public DeliverOrderRowsRequest(DeliverOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        public Webpay.Integration.CSharp.AdminWS.DeliveryResponse DoRequest()
        {
            var auth = new AdminWS.Authentication()
            {
                Password = _builder.GetConfig().GetPassword(_builder.OrderType,_builder.GetCountryCode()),
                Username = _builder.GetConfig().GetUsername(_builder.OrderType,_builder.GetCountryCode())                
            };

            var orderToDeliver = new AdminWS.DeliverOrderInformation()
            {
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
                SveaOrderId = _builder.Id,
                OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType)
                //PrintType // optional for EU-clients, and integration package only supports EU-clients
            };

            var request = new AdminWS.PartialDeliveryRequest()
            {
                Authentication = auth,
                OrderToDeliver = orderToDeliver,
                RowNumbers = _builder.RowIndexesToDeliver.ToArray(),
                InvoiceDistributionType = ConvertDistributionTypeToInvoiceDistributionType(_builder.DistributionType)
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.DeliverPartial(request);

            return response;
        }
    }
}