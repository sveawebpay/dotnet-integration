using Webpay.Integration.Exception;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.AdminService;

public class WebpayAdminRequest
{
    protected AdminWS.InvoiceDistributionType ConvertDistributionTypeToInvoiceDistributionType(DistributionType dt) =>
        dt switch
        {
            DistributionType.EMAIL => AdminWS.InvoiceDistributionType.Email,
            DistributionType.POST => AdminWS.InvoiceDistributionType.Post,
            _ => throw new SveaWebPayException("Invalid DistributionType")
        };

    protected AdminWS.OrderType ConvertPaymentTypeToOrderType(PaymentType pt) =>
        pt switch
        {
            PaymentType.INVOICE => AdminWS.OrderType.Invoice,
            PaymentType.PAYMENTPLAN => AdminWS.OrderType.PaymentPlan,
            PaymentType.ACCOUNTCREDIT => AdminWS.OrderType.AccountCredit,
            _ => throw new SveaWebPayException("Invalid PaymentType")
        };

    protected AdminWS.OrderRow ConvertOrderRowBuilderToAdminWSOrderRow(OrderRowBuilder orb)
    {
        var or = new AdminWS.OrderRow()
        {
            ArticleNumber = orb.GetArticleNumber(),
            Description = GetDescriptionFromBuilderOrderRow(orb.GetName(), orb.GetDescription()),
            DiscountPercent = orb.GetDiscountPercent(),
            NumberOfUnits = orb.GetQuantity(),
            PriceIncludingVat = orb.GetAmountIncVat().HasValue, // true if we have set amountIncVat
            PricePerUnit = (decimal)(orb.GetAmountIncVat() ?? orb.GetAmountExVat()),
            VatPercent = GetVatPercentFromBuilderOrderRow(orb.GetVatPercent(), orb.GetAmountIncVat(), orb.GetAmountExVat()),
        };
        return or;
    }

    protected AdminWS.NumberedOrderRow ConvertNumberedOrderRowBuilderToAdminWSNumberedOrderRow(NumberedOrderRowBuilder norb)
    {
        var nor = new AdminWS.NumberedOrderRow()
        {
            ArticleNumber = norb.GetArticleNumber(),
            Description = GetDescriptionFromBuilderOrderRow(norb.GetName(), norb.GetDescription()),
            DiscountPercent = norb.GetDiscountPercent(),
            NumberOfUnits = norb.GetQuantity(),
            PriceIncludingVat = norb.GetAmountIncVat().HasValue, // true if we have set amountIncVat
            PricePerUnit = (decimal)(norb.GetAmountIncVat() ?? norb.GetAmountExVat()),
            VatPercent = GetVatPercentFromBuilderOrderRow(norb.GetVatPercent(), norb.GetAmountIncVat(), norb.GetAmountExVat()),
            RowNumber = norb.GetRowNumber()
        };
        return nor;
    }

    protected static decimal GetVatPercentFromBuilderOrderRow(decimal? vp, decimal? incvat, decimal? exvat)
    {
        // Calculate vatPercent from 2 out of 3 of builder order row vat%, incVat, exVat 
        return (vp ?? (((incvat??0M)/(exvat??0M)) - 1M) * 100M);
    }

    protected static decimal GetAmountIncVatFromBuilderOrderRow(decimal? vp, decimal? incvat, decimal? exvat)
    {
        // Calculate amountIncVat from 2 out of 3 of builder order row vat%, incVat, exVat 
        return (incvat ?? ((exvat??0M) * (1 + GetVatPercentFromBuilderOrderRow(vp, incvat, exvat) / 100M)));
    }

    protected static decimal GetRowAmountIncVatFromBuilderOrderRow(decimal? vp, decimal? incvat, decimal? exvat, decimal quantity)
    {
        return (incvat ?? ((exvat ?? 0) * (1 + (vp ?? 0) / 100M))) * quantity;
    }

    protected static string GetDescriptionFromBuilderOrderRow(string name, string description)
    {
        // Calculate description as "<name>", "<description>" or if both, "<name>: <description>" from builder order row name, description
        return String.Format("{0}{1}{2}",
            name ?? "",
            (name == null) ? "" : ((description == null) ? "" : ": "),
            description);
    }
}