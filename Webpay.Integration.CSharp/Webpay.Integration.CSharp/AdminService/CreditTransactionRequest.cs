using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class CreditTransactionRequest
    {
        private readonly CreditAmountBuilder _builder;

        public CreditTransactionRequest(CreditAmountBuilder builder) {
            _builder = builder;
        }

        public CreditResponse DoRequest()
        {
            // should validate _builder.GetOrderId() existence here

            var hostedActionRequest = new HostedAdmin(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Credit(new Credit(
                    transactionId: _builder.GetTransactionId(),
                    amountToCredit: Decimal.ToInt64(_builder.AmountIncVat * 100)    //centessimal
                    ));

            return hostedActionRequest.DoRequest<CreditResponse>();
        }
    }
}