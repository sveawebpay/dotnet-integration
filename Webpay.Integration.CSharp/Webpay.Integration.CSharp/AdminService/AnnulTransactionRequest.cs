using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class AnnulTransactionRequest
    {
        readonly CancelOrderBuilder _builder;

        public AnnulTransactionRequest(CancelOrderBuilder builder) {
            _builder = builder;
        }

        public AnnulResponse DoRequest()
        {
            // should validate _builder.GetOrderId() existence here

            var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Annul(new Annul(
                    transactionId: _builder.Id
                    ));

            return hostedActionRequest.DoRequest<AnnulResponse>();
        }
    }
}