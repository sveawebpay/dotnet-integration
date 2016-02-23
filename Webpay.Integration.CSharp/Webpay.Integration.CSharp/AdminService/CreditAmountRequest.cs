using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class CreditAmountRequest
    {
        private readonly CreditAmountBuilder _builder;

        public CreditAmountRequest(CreditAmountBuilder builder)
        {
            _builder = builder;
        }

        public CancelPaymentPlanAmountResponse DoRequest()
        {
            var auth = new Webpay.Integration.CSharp.AdminWS.Authentication()
            {
                Password = _builder.GetConfig().GetPassword(_builder.OrderType, _builder.GetCountryCode()),
                Username = _builder.GetConfig().GetUsername(_builder.OrderType, _builder.GetCountryCode())
            };

            var request = new Webpay.Integration.CSharp.AdminWS.CancelPaymentPlanAmountRequest()
            {
                Authentication = auth,
                AmountInclVat = _builder.AmountIncVat,
                ContractNumber = _builder.Id,
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
                Description = _builder.Description
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.CancelPaymentPlanAmount(request);

            return response;
        }
    }
}