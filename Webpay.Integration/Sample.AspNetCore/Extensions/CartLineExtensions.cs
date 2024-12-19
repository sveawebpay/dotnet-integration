using Sample.AspNetCore.Models;
using System.Collections.Generic;
using Webpay.Integration.Order.Row;
using WebpayWS;

namespace Sample.AspNetCore.Extensions;

public static class CartLineExtensions
{
    public static OrderRowBuilder ToOrderRowBuilder(this CartLine line, bool isCompany)
    {
        var orderRow = Item.OrderRow()
            .SetArticleNumber(line.Product.Reference)
            .SetDescription(line.Product.Name)
            .SetQuantity(line.Quantity)
            .SetUnit("pcs")
            .SetDiscountPercent((int)line.Product.DiscountPercent)
            .SetVatPercent(line.Product.VatPercentage);

        if (isCompany)
        {
            orderRow.SetAmountExVat(line.Product.Price - line.Product.DiscountAmount);
        }
        else
        {
            orderRow.SetAmountIncVat(line.Product.Price * (1 + (line.Product.VatPercentage / 100)) - line.Product.DiscountAmount);
        }

        return orderRow;
    }
}