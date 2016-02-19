using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using InvoiceDistributionType = Webpay.Integration.CSharp.AdminWS.InvoiceDistributionType;
using OrderType = Webpay.Integration.CSharp.AdminWS.OrderType;

namespace Webpay.Integration.CSharp.AdminService
{
    public class CreditPaymentPlanOrderRowsRequest
    {
        private CreditOrderRowsBuilder _builder;

        public CreditPaymentPlanOrderRowsRequest(CreditOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        private AdminWS.InvoiceDistributionType ConvertDistributionTypeToInvoiceDistributionType(DistributionType dt)
        {
            switch (dt)
            {
                #pragma warning disable 0162 //CS0162 Unreachable code detected
                case DistributionType.NONE:
                    throw new SveaWebPayException("Invalid DistributionType");
                    break;
                case DistributionType.EMAIL:
                    return AdminWS.InvoiceDistributionType.Email;
                    break;
                case DistributionType.POST:
                    return AdminWS.InvoiceDistributionType.Post;
                    break;
                default:
                    throw new SveaWebPayException("Invalid DistributionType");
                #pragma warning restore 0162
            }
        }

        //private AdminWS.OrderType ConvertPaymentTypeToOrderType(Util.Constant.PaymentType pt)
        //{
        //    switch (pt)
        //    {
        //        #pragma warning disable 0162 //CS0162 Unreachable code detected
        //        case PaymentType.INVOICE:
        //            return AdminWS.OrderType.Invoice;
        //            break;
        //        case PaymentType.PAYMENTPLAN:
        //            return AdminWS.OrderType.PaymentPlan;
        //            break;
        //        default:
        //            throw new SveaWebPayException("Invalid PaymentType");
        //        #pragma warning restore 0162
        //    }
        //}

        private AdminWS.OrderRow ConvertOrderRowBuilderToAdminWSCancellationRow(OrderRowBuilder orb)
        {
            var or = new AdminWS.OrderRow()
            {
                ArticleNumber = orb.GetArticleNumber(),
                Description = orb.GetDescription(),
                //DiscountAmount = ,
                //DiscountAmountIncludingVat = ,
                DiscountPercent = orb.GetDiscountPercent(),
                //ExtensionData = ,
                NumberOfUnits = orb.GetQuantity(),
                PriceIncludingVat = orb.GetAmountIncVat().HasValue, // true iff we have set amountIncVat
                PricePerUnit = (decimal)(orb.GetAmountIncVat() ?? orb.GetAmountExVat()),
                VatPercent = (decimal)(orb.GetVatPercent() ?? (((orb.GetAmountIncVat()/orb.GetAmountExVat()) - 1M)*100M))
            };
            return or;
        }

        public Webpay.Integration.CSharp.AdminWS.CancelPaymentPlanRowsResponse DoRequest()
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