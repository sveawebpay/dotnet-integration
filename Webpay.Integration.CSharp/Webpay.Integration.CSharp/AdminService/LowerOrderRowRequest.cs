using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class LowerOrderRowRequest : WebpayAdminRequest
    {
        private readonly LowerOrderRowBuilder _builder;

        public LowerOrderRowRequest(LowerOrderRowBuilder builder) {
            _builder = builder;
        }

        public LowerOrderRowResponse DoRequest()
        {
            // lower order by calculated amount
            var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .LowerOrderRow(new LowerOrderRow(
                transactionId: _builder.Id,
                orderRows:_builder.OrderRows,
                correlationId:_builder.GetCorrelationId()));

            return hostedActionRequest.DoRequest<LowerOrderRowResponse>();
        }

    }
}