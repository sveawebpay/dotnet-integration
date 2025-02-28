using System.Linq;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class AddOrderRowsRequest : WebpayAdminRequest
    {
        private readonly AddOrderRowsBuilder _builder;

        public AddOrderRowsRequest(AddOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        public Webpay.Integration.CSharp.AdminWS.AddOrderRowsResponse DoRequest()
        {
            var auth = new AdminWS.Authentication()
            {
                Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
                Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
            };

            var request = new AdminWS.AddOrderRowsRequest()
            {
                Authentication = auth,
                SveaOrderId = _builder.Id,
                OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType),    // not required for EU-clients
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
                OrderRows = _builder.OrderRows.Select(x => ConvertOrderRowBuilderToAdminWSOrderRow(x)).ToArray()
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.AddOrderRows(request);

            return response;
        }
    }
}