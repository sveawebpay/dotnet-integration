using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.AspNetCore.Data;

namespace Sample.AspNetCore.Models;

public static class ProductGenerator
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new StoreDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<StoreDbContext>>());
        if (context.Products.Any())
        {
            return;
        }

        context.Products.AddRange(
            new Product
            {
                ProductId = 1,
                Class = "Identifier1",
                Type = "PRODUCT",
                Name = "Levis 511 Slim Fit",
                Reference = "Ref1",
                Price = 899,
                DiscountAmount = 0
            }, new Product
            {
                ProductId = 2,
                Class = "Identifier2",
                Type = "PRODUCT",
                Name = "Levis 501 Jeans",
                Reference = "Ref2",
                Price = 1190,
                DiscountAmount = 0
            },
            new Product
            {
                ProductId = 3,
                Class = "Identifier3",
                Type = "PRODUCT",
                Name = "Levis 512 Jeans",
                Reference = "Ref3",
                Price = 1000,
                DiscountAmount = 100,
                VatPercentage = 10
            },
            new Product
            {
                ProductId = 4,
                Class = "Identifier4",
                Type = "PRODUCT",
                Name = "Levis 520 Jeans",
                Reference = "Ref4",
                Price = 1000,
                DiscountPercent = 20,
                VatPercentage = 20
            },
            new Product
            {
                ProductId = 5,
                Class = "Identifier5",
                Type = "GIFTCARD",
                Name = "Rabattkod 1190",
                Reference = "Ref5",
                Price = -1190,
                DiscountPercent = 0,
            },
            new Product
            {
                ProductId = 6,
                Class = "Identifier6",
                Type = "GIFTCARD",
                Name = "Rabattkod 899",
                Reference = "Ref6",
                Price = -899,
                DiscountPercent = 0,
            },
            new Product
            {
                ProductId = 7,
                Class = "Identifier7",
                Type = "FREE",
                Name = "Gratisprodukt",
                Reference = "Ref6",
                Price = 0,
                DiscountPercent = 0,
            });

        context.SaveChanges();
    }
}