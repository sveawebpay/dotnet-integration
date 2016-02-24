using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class QueryTransactionRequest
    {
        readonly QueryOrderBuilder _builder;

        public QueryTransactionRequest(QueryOrderBuilder builder) {
            _builder = builder;
        }

        public QueryResponse DoRequest()
        {
            // should validate _builder.GetOrderId() existence here

            var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Query(new QueryByTransactionId(
                    transactionId: _builder.Id
                    ));

            return hostedActionRequest.DoRequest<QueryResponse>();
        }
    }
}