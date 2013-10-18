namespace Webpay.Integration.CSharp.Order.Row
{
    public interface IRowBuilder
    {
        string GetName();
        string GetDescription();
        string GetUnit();

        string GetArticleNumber();
        int GetDiscountPercent();
        decimal GetQuantity();

        decimal? GetAmountExVat();
        decimal? GetVatPercent();
        decimal? GetAmountIncVat();
    }
}