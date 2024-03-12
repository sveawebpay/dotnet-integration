namespace Webpay.Integration.CSharp.Order.Row.credit
{
    public class CreditOrderRowBuilder 
    {
        public int RowId { get; set; }
        public string Name { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal? VatPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? DiscountPercent { get; set; }
        public string Unit { get; set; }
        public string ArticleNumber { get; set; }
       
    }
}