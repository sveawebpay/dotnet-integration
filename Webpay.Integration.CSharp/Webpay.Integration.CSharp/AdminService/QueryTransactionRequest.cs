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

            var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
                .Query(new QueryByTransactionId(
                    transactionId: _builder.Id,
                    correlationId: _builder.GetCorrelationId()
                    ));

            return hostedActionRequest.DoRequest<QueryResponse>();
        }
    }
}