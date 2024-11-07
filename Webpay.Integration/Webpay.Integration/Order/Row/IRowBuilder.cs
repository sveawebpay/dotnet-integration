namespace Webpay.Integration.Order.Row;

public interface IRowBuilder
{
    string GetName();
    string GetDescription();
    string GetUnit();

    string GetArticleNumber();
    decimal GetDiscountPercent();
    decimal GetQuantity();

    decimal? GetAmountExVat();
    decimal? GetVatPercent();
    decimal? GetAmountIncVat();
}