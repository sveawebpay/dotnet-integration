using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class QueryTransactionRequest
    {
        QueryOrderBuilder _builder;

        public QueryTransactionRequest(QueryOrderBuilder builder) {
            _builder = builder;
        }

        public QueryResponse DoRequest()
        {
            // should validate _builder.GetOrderId() existence here

            var hostedActionRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Query(new QueryByTransactionId(
                    transactionId: _builder.GetOrderId()
                    ));

            return hostedActionRequest.DoRequest<QueryResponse>();
        }

    }
}