namespace Webpay.Integration.CSharp.Order.Row
{
    public interface IRowBuilder
    {
        string GetName();
        string GetDescription();
        string GetUnit();
    }
}
