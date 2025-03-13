using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class LowerOrderRowConfirmRequest : WebpayAdminRequest
    {
        private readonly LowerOrderRowConfirmBuilder _builder;

        public LowerOrderRowConfirmRequest(LowerOrderRowConfirmBuilder builder) {
            _builder = builder;
        }

        public LowerOrderRowConfirmResponse DoRequest()
        {
            // lower order by calculated amount
            var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .LowerOrderRowConfirm(new LowerOrderRowConfirm(
                transactionId: _builder.Id,
                orderRows:_builder.OrderRows,
                captureDate:_builder.CaptureDate,
                correlationId:_builder.GetCorrelationId()));

            return hostedActionRequest.DoRequest<LowerOrderRowConfirmResponse>();
        }

    }
}