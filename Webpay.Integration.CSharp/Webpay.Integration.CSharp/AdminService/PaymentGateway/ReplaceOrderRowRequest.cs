using System;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Handle.PaymentGateway;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService.PaymentGateway
{
    public class ReplaceOrderRowRequest : WebpayAdminRequest
    {
        private readonly ReplaceOrderRowBuilder _builder;

        public ReplaceOrderRowRequest(ReplaceOrderRowBuilder builder) {
            _builder = builder;
        }

        public ReplaceOrderRowResponse DoRequest()
        {
            var hostedActionRequest = 
            new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .ReplaceOrderRow(new ReplaceOrderRow(
                transactionId: _builder.Id,
                orderRows: _builder.OrderRows,
                correlationId: _builder.GetCorrelationId()))
            ;

            return hostedActionRequest.DoRequest<ReplaceOrderRowResponse>();
        }

    }
}