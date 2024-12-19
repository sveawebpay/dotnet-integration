namespace Webpay.Integration.Order.Row;

public interface IPriced<T>
{
    decimal? GetAmountExVat();

    /// <summary>
    /// Optional
    /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
    /// </summary>
    /// <param name="dExVatAmount"></param>
    /// <returns>OrderRowBuilder</returns>
    T SetAmountExVat(decimal dExVatAmount);

    decimal? GetVatPercent();

    /// <summary>
    /// Optional
    /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
    /// </summary>
    /// <param name="vatPercent"></param>
    /// <returns>OrderRowBuilder</returns>
    T SetVatPercent(decimal vatPercent);

    decimal? GetAmountIncVat();

    /// <summary>
    /// Optional
    /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
    /// </summary>
    /// <param name="amountIncVat"></param>
    /// <returns>OrderRowBuilder</returns>
    T SetAmountIncVat(decimal amountIncVat);
}