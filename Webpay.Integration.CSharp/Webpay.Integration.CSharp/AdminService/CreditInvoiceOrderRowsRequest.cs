using System.Linq;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class CreditInvoiceOrderRowsRequest : WebpayAdminRequest
    {
        private readonly CreditOrderRowsBuilder _builder;

        public CreditInvoiceOrderRowsRequest(CreditOrderRowsBuilder builder)
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

            var request = new AdminWS.CreditInvoiceRequest
            {
                Authentication = auth,
                InvoiceId = _builder.Id,
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
                RowNumbers = _builder.RowIndexesToCredit.ToArray(),
                InvoiceDistributionType = ConvertDistributionTypeToInvoiceDistributionType(_builder.DistributionType),
                NewCreditInvoiceRows = _builder.NewCreditOrderRows.Select( x => ConvertOrderRowBuilderToAdminWSOrderRow(x) ).ToArray()
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.CreditInvoiceRows(request);

            return response;
        }
    }
}