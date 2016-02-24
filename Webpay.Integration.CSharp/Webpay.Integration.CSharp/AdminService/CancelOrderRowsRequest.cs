using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class CancelOrderRowsRequest : WebpayAdminRequest
    {
        private readonly CancelOrderRowsBuilder _builder;

        public CancelOrderRowsRequest(CancelOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        public Webpay.Integration.CSharp.AdminWS.CancelOrderRowsResponse DoRequest()
        {
            var auth = new AdminWS.Authentication()
            {
                Password = _builder.GetConfig().GetPassword(_builder.OrderType,_builder.GetCountryCode()),
                Username = _builder.GetConfig().GetUsername(_builder.OrderType,_builder.GetCountryCode())                
            };

            var request = new AdminWS.CancelOrderRowsRequest()
            {
                Authentication = auth,
                SveaOrderId = _builder.Id,
                OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType),
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
                OrderRowNumbers = _builder.RowIndexesToCancel.ToArray(),
                //NewCreditInvoiceRows = _builder.NewCreditOrderRows.Select( x => ConvertOrderRowBuilderToAdminWSOrderRow(x) ).ToArray()
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.CancelOrderRows(request);

            return response;
        }
    }
}