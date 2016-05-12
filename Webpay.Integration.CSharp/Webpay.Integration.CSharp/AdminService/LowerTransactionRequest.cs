using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class LowerTransactionRequest : WebpayAdminRequest
    {
        private readonly CancelOrderRowsBuilder _builder;

        public LowerTransactionRequest(CancelOrderRowsBuilder builder) {
            _builder = builder;
        }

        public LowerAmountResponse DoRequest()
        {
            // calculate sum of cancelled order rows, applying RowsToCancel to passed in NumberedOrderRows
            var amountToLowerOrderBy = 0M;
            foreach (int rowIndex in _builder.RowIndexesToCancel)
            {
                var deliveredRow = _builder.NumberedOrderRows[(rowIndex - 1)]; // -1 as NumberedOrderRows is one-indexed
                amountToLowerOrderBy += GetRowAmountIncVatFromBuilderOrderRow(
                    deliveredRow.GetVatPercent(), deliveredRow.GetAmountIncVat(), deliveredRow.GetAmountExVat(), deliveredRow.GetQuantity());
            }

            // lower order by calculated amount
            var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
            .LowerAmount(new LowerAmount(
                transactionId: _builder.Id,
                amountToLower: Decimal.ToInt64(amountToLowerOrderBy * 100)    // centessimal
                ));

            return hostedActionRequest.DoRequest<LowerAmountResponse>();
        }

    }
}