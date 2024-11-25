using System.Xml;
using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Hosted.Admin.Actions;
using Webpay.Integration.Hosted.Admin.Response;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Order.Row;

namespace Webpay.Integration.AdminService;

public class ConfirmTransactionRequest : WebpayAdminRequest
{
    private readonly DeliverOrderRowsBuilder _builder;

    public ConfirmTransactionRequest(DeliverOrderRowsBuilder builder)
    {
        _builder = builder;
    }

    public ConfirmResponse DoRequest()
    {
        // Calculate original order rows total, incvat row sum over numberedOrderRows
        var originalOrderTotal = 0M;
        foreach(NumberedOrderRowBuilder originalRow in _builder.NumberedOrderRows)
        {
            originalOrderTotal += GetRowAmountIncVatFromBuilderOrderRow(
                originalRow.GetVatPercent(), originalRow.GetAmountIncVat(), originalRow.GetAmountExVat(), originalRow.GetQuantity());
        }

        // Calculate delivered order rows total, incvat row sum over deliveredOrderRows
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
            // First loweramount, then confirm
            var lowerAmountRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
                .LowerAmount(new LowerAmount(
                    transactionId: _builder.Id,
                    amountToLower: Decimal.ToInt64(amountToLowerOrderBy *100), // Centessimal
                    correlationId: new Guid()));

            var lowerAmountResponse = lowerAmountRequest.DoRequest<LowerAmountResponse>();

            // If error lowering amount, return a dummy ConfirmRespose response w/status code 100 INTERNAL_ERROR
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
                captureDate: _builder.CaptureDate ?? DateTime.Now , // If no captureDate set, use today's date as default.
                correlationId: new Guid()
                ));

        return hostedActionRequest.DoRequest<ConfirmResponse>();
    }
}