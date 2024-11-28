using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sample.AspNetCore.Models;

using System.Text.Json.Serialization;

public class Cart
{
    private List<CartLine> CartLineCollection { get; set; } = new List<CartLine>();

    [JsonInclude]
    public virtual IEnumerable<CartLine> CartLines
    {
        get => CartLineCollection;
        private set => CartLineCollection = value.ToList();
    }

    public string SveaOrderId { get; set; }
    public bool Vat { get; set; }
    public bool IsInternational { get; set; }
    public string ShippingStatus { get; set; }
    public string ShippingDescription { get; set; }


    public virtual void AddItem(Product product, int quantity)
    {
        var line = CartLineCollection.FirstOrDefault(p => p.Product.ProductId == product.ProductId);

        if (line == null)
            CartLineCollection.Add(new CartLine
            {
                Product = product,
                Quantity = quantity
            });
        else
            line.Quantity += quantity;
    }


    public virtual decimal CalculateTotal()
    {
        return CartLineCollection.Sum(e =>
        {
            var subTotalBeforeDiscount = (e.Product.Price * e.Quantity);

            if (e.Product.DiscountAmount != 0)
            {
                return subTotalBeforeDiscount - e.Product.DiscountAmount;
            }
            else if (e.Product.DiscountPercent != 0)
            {
                return subTotalBeforeDiscount - (subTotalBeforeDiscount * e.Product.DiscountPercent / 100);
            }
            else
            {
                return subTotalBeforeDiscount;
            }
        });
    }


    public virtual void Clear()
    {
        CartLineCollection.Clear();
    }


    public virtual void RemoveItem(Product product, int quantity)
    {
        var line = CartLineCollection.FirstOrDefault(p => p.Product.ProductId == product.ProductId);

        if (line != null)
        {
            if (quantity >= line.Quantity)
                CartLineCollection.Remove(line);
            else
                line.Quantity -= quantity;
        }
    }


    public virtual void Update()
    {
    }
}

public class CartLine
{
    public int CartLineId { get; set; }
    public Product Product { get; set; }

    [Required(ErrorMessage = "Please provide a number greater than zero!")]
    [Display(Name = "Unit quantity")]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }


    public decimal CalculateTotal()
    {
        return Quantity * (Product.Price - Product.DiscountAmount);
    }
}
