﻿namespace Webpay.Integration.Order.Row.credit;

public class NewCreditOrderRowBuilder 
{
    public string Name { get; set; }
    public long UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal VatPercent { get; set; }
    public long DiscountAmount { get; set; }
    public decimal DiscountPercent { get; set; }
    public string Unit { get; set; }
    public string ArticleNumber { get; set; }
}