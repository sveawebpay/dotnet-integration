using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Order.Handle.PaymentGateway;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class UpdateOrderRowRequest : WebpayAdminRequest
    {
        private readonly UpdateOrderRowBuilder _builder;

        public UpdateOrderRowRequest(UpdateOrderRowBuilder builder) {
            _builder = builder;
        }

        public UpdateOrderRowResponse DoRequest()
        {
            var hostedActionRequest = 
            new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .EditOrderRow(new UpdateOrderRow(
                transactionId: _builder.Id,
                orderRows: _builder.OrderRows,
                correlationId: _builder.GetCorrelationId()));

            return hostedActionRequest.DoRequest<UpdateOrderRowResponse>();
        }

    }
}