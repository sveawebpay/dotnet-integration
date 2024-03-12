using System;
using System.Xml;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class CreditTransactionRequest
    {
        private readonly CreditOrderBuilder _builder;

        public CreditTransactionRequest(CreditOrderBuilder builder)
        {
            _builder = builder;
        }

        public CreditResponse DoRequest()
        {
            var creditRequest = new Credit(
                    transactionId: _builder.Id,
                    amountToCredit: Decimal.ToInt64(_builder.AmountIncVat * 100),
                    orderRows: _builder.CreditOrderRows,
                    correlationId: _builder.GetCorrelationId());
            CreditResponse validationResoponse = null;
            if(creditRequest.ValidateCreditRequest(out validationResoponse))
            {
                var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
                .Credit(creditRequest);

                return hostedActionRequest.DoRequest<CreditResponse>();
            }
            return validationResoponse;


        }

      
    }
}