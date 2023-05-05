using System;
using System.Linq;
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
    public class ConfirmPartialTransactionRequest : WebpayAdminRequest
    {
        private readonly DeliverOrderRowsBuilder _builder;

        public ConfirmPartialTransactionRequest(DeliverOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        public ConfirmPartialResponse DoRequest()
        {           
            // calculate delivered order rows total, incvat row sum over deliveredOrderRows
            var deliveredOrderTotal = 0M;
            foreach (int rowIndex in _builder.RowIndexesToDeliver)
            {
                var deliveredRow = _builder.NumberedOrderRows[(rowIndex - 1)]; // -1 as NumberedOrderRows is one-indexed
                deliveredOrderTotal += GetRowAmountIncVatFromBuilderOrderRow(
                    deliveredRow.GetVatPercent(), deliveredRow.GetAmountIncVat(), deliveredRow.GetAmountExVat(), deliveredRow.GetQuantity());
            }

            var deliverOrderRows = _builder.RowIndexesToDeliver
                         .Select(index => _builder.NumberedOrderRows[(int)index-1])
                         .ToList();
            if (deliveredOrderTotal > 0M)
            {
                var partialConfirmRequest = new HostedAdmin(_builder.GetConfig(), _builder.GetCountryCode())
                    .ConfirmPartial(new ConfirmPartial(
                        transactionId: _builder.Id,
                        callerReferenceId: _builder.CallerReferenceId, 
                        amount: Decimal.ToInt64(deliveredOrderTotal * 100),
                        orderRows: deliverOrderRows
                        ));

                var partialConfirmResponse = partialConfirmRequest.DoRequest<ConfirmPartialResponse>();

                if (!partialConfirmResponse.Accepted)
                {
                  
                    return GetDefaultResponse();
                }

                return partialConfirmResponse;

            }
            
            return GetDefaultResponse();
        }


        private ConfirmPartialResponse GetDefaultResponse()
        {
            var dummyInternalErrorResponseXml = new XmlDocument();
            dummyInternalErrorResponseXml.LoadXml(@"<?xml version='1.0' encoding='UTF-8'?>
                        <response>
                            <statuscode>100</statuscode>
                        </response>");

            return ConfirmPartial.Response(dummyInternalErrorResponseXml);
        }
    }
}