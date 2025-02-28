using System;
using System.Xml;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Hosted.Admin.Response;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class ConfirmTransactionRequest : WebpayAdminRequest
    {
        private readonly DeliverOrderRowsBuilder _builder;

        public ConfirmTransactionRequest(DeliverOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        public ConfirmResponse DoRequest()
        {
            // calculate original order rows total, incvat row sum over numberedOrderRows
            var originalOrderTotal = 0M;
            foreach(NumberedOrderRowBuilder originalRow in _builder.NumberedOrderRows)
            {
                originalOrderTotal += GetRowAmountIncVatFromBuilderOrderRow(
                    originalRow.GetVatPercent(), originalRow.GetAmountIncVat(), originalRow.GetAmountExVat(), originalRow.GetQuantity());
            }

            // calculate delivered order rows total, incvat row sum over deliveredOrderRows
            var deliveredOrderTotal = 0M;
            foreach (int rowIndex in _builder.RowIndexesToDeliver)
            {
                var deliveredRow = _builder.NumberedOrderRows[(rowIndex - 1)]; // -1 as NumberedOrderRows is one-indexed
                deliveredOrderTotal += GetRowAmountIncVatFromBuilderOrderRow(
                    deliveredRow.GetVatPercent(), deliveredRow.GetAmountIncVat(), deliveredRow.GetAmountExVat(), deliveredRow.GetQuantity());
            }

            var amountToLowerOrderBy = originalOrderTotal - deliveredOrderTotal;

            if (amountToLowerOrderBy > 0M)
            {
                // first loweramount, then confirm!
                var lowerAmountRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
                    .LowerAmount(new LowerAmount(
                        transactionId: _builder.Id,
                        amountToLower: Decimal.ToInt64(amountToLowerOrderBy *100),    // centessimal
                        correlationId: new Guid()));

                var lowerAmountResponse = lowerAmountRequest.DoRequest<LowerAmountResponse>();

                // if error lowering amount, return a dummy ConfirmRespose response w/status code 100 INTERNAL_ERROR
                if (!lowerAmountResponse.Accepted)
                {
                    var dummyInternalErrorResponseXml = new XmlDocument();
                    dummyInternalErrorResponseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                        <response>
                            <statuscode>100</statuscode>
                        </response>");

                    return Confirm.Response(dummyInternalErrorResponseXml);
                }
            }
            var hostedActionRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
                .Confirm(new Confirm(
                    transactionId: _builder.Id,
                    captureDate: _builder.CaptureDate ?? DateTime.Now , // if no captureDate set, use today's date as default.
                    correlationId: new Guid()
                    ));

            return hostedActionRequest.DoRequest<ConfirmResponse>();
        }
    }
}