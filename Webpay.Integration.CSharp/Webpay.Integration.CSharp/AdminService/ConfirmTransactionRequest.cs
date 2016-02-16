using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class ConfirmTransactionRequest
    {
        DeliverOrderRowsBuilder _builder;

        public ConfirmTransactionRequest(DeliverOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        public ConfirmResponse DoRequest()
        {
            // should validate _builder.GetOrderId() existence here

            // calculate original order rows total, incvat row sum over numberedOrderRows
            var originalOrderTotal = 0M;
            foreach(NumberedOrderRowBuilder OriginalRow in _builder._numberedOrderRows)
            {
                originalOrderTotal += OriginalRow.GetAmountExVat()??0 * (1 + OriginalRow.GetVatPercent()??0/100M)* OriginalRow.GetQuantity();
            }

            // calculate delivered order rows total, incvat row sum over deliveredOrderRows
        	var deliveredOrderTotal = 0M;
            foreach (int rowIndex in _builder._rowIndexesToDeliver)
            {
                var deliveredRow = _builder._numberedOrderRows[(rowIndex - 1)]; // -1 as NumberedOrderRows is one-indexed
                deliveredOrderTotal += deliveredRow.GetAmountExVat()??0 * (1+deliveredRow.GetVatPercent()??0/100M) * deliveredRow.GetQuantity();
            }

            var amountToLowerOrderBy = originalOrderTotal - deliveredOrderTotal;

            if (amountToLowerOrderBy > 0M)
            {
                // first lower, then confirm!
                throw new NotImplementedException();
            }

            var hostedActionRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), CountryCode.SE)
                .Confirm(new Confirm(
                    transactionId: _builder.GetOrderId(),
                    captureDate: _builder._captureDate ?? DateTime.Now  // if no captureDate set, use today's date as default.
                    ));

            return hostedActionRequest.DoRequest<ConfirmResponse>();
        }
    }
}