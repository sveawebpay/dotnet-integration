using System;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class WebpayAdminRequest
    {
        protected AdminWS.InvoiceDistributionType ConvertDistributionTypeToInvoiceDistributionType(DistributionType dt)
        {
            switch (dt)
            {
                case DistributionType.NONE:
                    throw new SveaWebPayException("Invalid DistributionType");

                case DistributionType.EMAIL:
                    return AdminWS.InvoiceDistributionType.Email;

                case DistributionType.POST:
                    return AdminWS.InvoiceDistributionType.Post;

                default:
                    throw new SveaWebPayException("Invalid DistributionType");
            }
        }

        protected AdminWS.OrderType ConvertPaymentTypeToOrderType(Util.Constant.PaymentType pt)
        {
            switch (pt)
            {
                #pragma warning disable 0162 //CS0162 Unreachable code detected
                case PaymentType.INVOICE:
                    return AdminWS.OrderType.Invoice;
                    break;
                case PaymentType.PAYMENTPLAN:
                    return AdminWS.OrderType.PaymentPlan;
                    break;
                default:
                    throw new SveaWebPayException("Invalid PaymentType");
                #pragma warning restore 0162
            }
        }

        protected AdminWS.OrderRow ConvertOrderRowBuilderToAdminWSOrderRow(OrderRowBuilder orb)
        {
            var or = new AdminWS.OrderRow()
            {
                ArticleNumber = orb.GetArticleNumber(),
                Description = GetDescriptionFromBuilderOrderRow(orb.GetName(), orb.GetDescription()),
                //DiscountAmount = ,
                //DiscountAmountIncludingVat = ,
                DiscountPercent = orb.GetDiscountPercent(),
                //ExtensionData = ,
                NumberOfUnits = orb.GetQuantity(),
                PriceIncludingVat = orb.GetAmountIncVat().HasValue, // true iff we have set amountIncVat
                PricePerUnit = (decimal)(orb.GetAmountIncVat() ?? orb.GetAmountExVat()),
                VatPercent = GetVatPercentFromBuilderOrderRow(orb.GetVatPercent(), orb.GetAmountIncVat(), orb.GetAmountExVat()),
            };
            return or;
        }

        protected AdminWS.OrderRow ConvertInvoiceFeeBuilderToAdminWSOrderRow(InvoiceFeeBuilder invoiceFee)
        {
            var orderRow = new AdminWS.OrderRow()
            {
                ArticleNumber = invoiceFee.GetArticleNumber(),
                Description = GetDescriptionFromBuilderOrderRow(invoiceFee.GetName(), invoiceFee.GetDescription()),
                DiscountPercent = invoiceFee.GetDiscountPercent(),
                NumberOfUnits = invoiceFee.GetQuantity(),
                PriceIncludingVat = invoiceFee.GetAmountIncVat().HasValue,
                PricePerUnit = (decimal)(invoiceFee.GetAmountIncVat() ?? invoiceFee.GetAmountExVat()),
                Unit = invoiceFee.GetUnit(),
                RowType = AdminWS.RowType.InvoiceFee,
                VatPercent = GetVatPercentFromBuilderOrderRow(invoiceFee.GetVatPercent(), invoiceFee.GetAmountIncVat(), invoiceFee.GetAmountExVat())
            };
            return orderRow;
        }

        protected AdminWS.NumberedOrderRow ConvertInvoiceFeeBuilderToAdminWSNumberedOrderRow(InvoiceFeeBuilder invoiceFee)
        {
            var orderRow = new AdminWS.NumberedOrderRow()
            {
                ArticleNumber = invoiceFee.GetArticleNumber(),
                Description = GetDescriptionFromBuilderOrderRow(invoiceFee.GetName(), invoiceFee.GetDescription()),
                DiscountPercent = invoiceFee.GetDiscountPercent(),
                NumberOfUnits = invoiceFee.GetQuantity(),
                PriceIncludingVat = invoiceFee.GetAmountIncVat().HasValue,
                PricePerUnit = (decimal)(invoiceFee.GetAmountIncVat() ?? invoiceFee.GetAmountExVat()),
                Unit = invoiceFee.GetUnit(),
                RowType = AdminWS.RowType.InvoiceFee,
                VatPercent = GetVatPercentFromBuilderOrderRow(invoiceFee.GetVatPercent(), invoiceFee.GetAmountIncVat(), invoiceFee.GetAmountExVat())
            };
            return orderRow;
        }

        protected AdminWS.NumberedOrderRow ConvertNumberedOrderRowBuilderToAdminWSNumberedOrderRow(NumberedOrderRowBuilder norb)
        {
            var nor = new AdminWS.NumberedOrderRow()
            {
                ArticleNumber = norb.GetArticleNumber(),
                Description = GetDescriptionFromBuilderOrderRow(norb.GetName(), norb.GetDescription()),
                //DiscountAmount = ,
                //DiscountAmountIncludingVat = ,
                DiscountPercent = norb.GetDiscountPercent(),
                //ExtensionData = ,
                NumberOfUnits = norb.GetQuantity(),
                PriceIncludingVat = norb.GetAmountIncVat().HasValue, // true iff we have set amountIncVat
                PricePerUnit = (decimal)(norb.GetAmountIncVat() ?? norb.GetAmountExVat()),
                VatPercent = GetVatPercentFromBuilderOrderRow(norb.GetVatPercent(), norb.GetAmountIncVat(), norb.GetAmountExVat()),
                //DiscountAmount = ,                    // not yet supported
                //DiscountAmountIncludingVat = ,        // not yet supported
                RowNumber = norb.GetRowNumber()
            };
            return nor;
        }

        protected static decimal GetVatPercentFromBuilderOrderRow(decimal? vp, decimal? incvat, decimal? exvat)
        {
            // calculate vatPercent from 2 out of 3 of builder order row vat%, incVat, exVat 
            return (vp ?? (((incvat??0M)/(exvat??0M)) - 1M) * 100M);
        }
        protected static decimal GetAmountIncVatFromBuilderOrderRow(decimal? vp, decimal? incvat, decimal? exvat)
        {
            // calculate amountIncVat from 2 out of 3 of builder order row vat%, incVat, exVat 
            return (incvat ?? ((exvat??0M) * (1 + GetVatPercentFromBuilderOrderRow(vp, incvat, exvat) / 100M)));
        }

        protected static decimal GetRowAmountIncVatFromBuilderOrderRow(decimal? vp, decimal? incvat, decimal? exvat, decimal quantity)
        {
            return (incvat ?? ((exvat ?? 0) * (1 + (vp ?? 0) / 100M))) * quantity;
        }

        protected static string GetDescriptionFromBuilderOrderRow(string name, string description)
        {
            // calculate description as "<name>", "<description>" or if both, "<name>: <description>" from builder order row name, description
            return String.Format("{0}{1}{2}",
                name ?? "",
                (name == null) ? "" : ((description == null) ? "" : ": "),
                description);
        }

    }
}