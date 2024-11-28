using AdminWS;
using Sample.AspNetCore.Models;
using System.Collections.Generic;

namespace Sample.AspNetCore.Extensions;

public static class CartLineExtensions
{
    public static IEnumerable<OrderRow> ToOrderItems(this IEnumerable<CartLine> lines)
    {
        var rowNumber = 1;
        foreach (var line in lines)
        {
            yield return new OrderRow
            {
                ArticleNumber = line.Product.Reference,
                Description = line.Product.Name,
                NumberOfUnits = line.Quantity,
                PricePerUnit = line.Product.Price,
                DiscountAmount = line.Product.DiscountAmount > 0 ? line.Product.DiscountAmount : (decimal?)null,
                DiscountPercent = line.Product.DiscountAmount == 0 && line.Product.DiscountPercent > 0 ? line.Product.DiscountPercent : 0,
                VatPercent = line.Product.VatPercentage,
                PriceIncludingVat = null,
                DiscountAmountIncludingVat = null,
                RowType = null,
                TemporaryReference = null,
                Unit = null,
            };
            rowNumber++;
        }
    }
}