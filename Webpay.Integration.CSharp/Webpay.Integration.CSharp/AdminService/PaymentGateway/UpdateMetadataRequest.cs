using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Order.Handle.PaymentGateway;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService.PaymentGateway
{
    public class UpdateMetadataRequest : WebpayAdminRequest
    {
        private readonly UpdatePaymentMetadataBuilder _builder;

        public UpdateMetadataRequest(UpdatePaymentMetadataBuilder builder) {
            _builder = builder;
        }

        public UpdateMetadataResponse DoRequest()
        {
            var hostedActionRequest = 
            new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .UpdateMetadata(new UpdateMetadata(
                transactionId: _builder.TransactionId,
                metadata: _builder.Metadata,
                correlationId: _builder.GetCorrelationId()));

            return hostedActionRequest.DoRequest<UpdateMetadataResponse>();
        }

    }
}