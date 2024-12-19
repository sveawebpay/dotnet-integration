using Webpay.Integration.Order.Row.credit;

namespace Webpay.Integration.Order;

public class Delivery
{
    public long? Id {  get; set; }
    public List<NewCreditOrderRowBuilder> NewOrderRows { get; set; }
    public List<CreditOrderRowBuilder> OrderRows { get; set; }

    public Delivery() {
        NewOrderRows = new List<NewCreditOrderRowBuilder>();
        OrderRows = new List<CreditOrderRowBuilder>();
    }

    public Delivery AddOrderRows(List<NewCreditOrderRowBuilder> newOrderRows, List<CreditOrderRowBuilder> orderRows)
    {
        NewOrderRows.AddRange(newOrderRows);
        OrderRows.AddRange(orderRows);
        return this;
    }
}
