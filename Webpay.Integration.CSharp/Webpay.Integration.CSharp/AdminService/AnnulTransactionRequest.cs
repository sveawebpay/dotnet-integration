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
        private readonly CancelOrderBuilder _builder;

        public AnnulTransactionRequest(CancelOrderBuilder builder) {
            _builder = builder;
        }

        public AnnulResponse DoRequest()
        {
            var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
                .Annul(new Annul(
                    transactionId: _builder.Id
                    ));

            return hostedActionRequest.DoRequest<AnnulResponse>();
        }
    }
}