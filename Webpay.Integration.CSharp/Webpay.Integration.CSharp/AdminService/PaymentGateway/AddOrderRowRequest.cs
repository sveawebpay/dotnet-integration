using System;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions.PaymentGateway;
using Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Handle.PaymentGateway;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class AddOrderRowRequest : WebpayAdminRequest
    {
        private readonly AddOrderRowBuilder _builder;

        public AddOrderRowRequest(AddOrderRowBuilder builder) {
            _builder = builder;
        }

        public AddOrderRowResponse DoRequest()
        {
            var hostedActionRequest = 
            new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .AddOrderRow(new AddOrderRow(
                transactionId: _builder.Id,
                orderRows: _builder.OrderRows,
                correlationId: _builder.GetCorrelationId()))
            ;

            return hostedActionRequest.DoRequest<AddOrderRowResponse>();
        }

    }
}