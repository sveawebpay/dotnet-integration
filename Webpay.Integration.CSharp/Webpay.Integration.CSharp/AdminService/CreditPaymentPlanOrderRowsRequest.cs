using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class CreditPaymentPlanOrderRowsRequest
    {
        private readonly CreditOrderRowsBuilder _builder;

        public CreditPaymentPlanOrderRowsRequest(CreditOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        public AdminWS.CancelPaymentPlanRowsResponse DoRequest()
        {
            var auth = new AdminWS.Authentication()
            {
                Password = _builder.GetConfig().GetPassword(_builder.OrderType,_builder.GetCountryCode()),
                Username = _builder.GetConfig().GetUsername(_builder.OrderType,_builder.GetCountryCode())                
            };

            var cancellationRows = new List<AdminWS.CancellationRow>();
            foreach (var rowIndex in _builder.RowIndexesToCredit)
            {
                var cancellationRow = new AdminWS.CancellationRow()
                {
                    AmountInclVat = 0M,
                    VatPercent = 0M,
                    Description = null,
                    RowNumber = (int)rowIndex
                };
                cancellationRows.Add(cancellationRow);
            }
            foreach (var ncr in _builder.NewCreditOrderRows)
            {
                // calculate amountIncVat from 2 out of 3 of incVat, exVat and vat% 
                var vatPercent = (decimal)(ncr.GetVatPercent() ?? (((ncr.GetAmountIncVat() / ncr.GetAmountExVat()) - 1M) * 100M));
                var amountIncVat = (decimal)(ncr.GetAmountIncVat() ?? (ncr.GetAmountExVat()*(1 + vatPercent/100M)));

                // "<name>", "<description>" or if both, "<name>: <description>"
                var rowNameAndDescription = String.Format("{0}{1}{2}",
                    ncr.GetName() ?? "",
                    (ncr.GetName() == null) ? "" : ((ncr.GetDescription() == null) ? "" : ": "),
                    ncr.GetDescription());

                var cancellationRow = new AdminWS.CancellationRow()
                {
                    AmountInclVat = amountIncVat,
                    VatPercent = vatPercent,
                    Description = rowNameAndDescription,
                    RowNumber = null
                };
                cancellationRows.Add(cancellationRow);
            }

            var request = new AdminWS.CancelPaymentPlanRowsRequest()
            {
                Authentication = auth,
                ContractNumber = _builder.Id,
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
                CancellationRows = cancellationRows.ToArray()
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.CancelPaymentPlanRows(request);

            return response;
        }
    }
}