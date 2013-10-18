using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;

namespace Webpay.Integration.CSharp.Order.Validator
{
    public abstract class OrderValidator : IOrderValidator
    {
        protected string Errors = "";

        public abstract string Validate(CreateOrderBuilder order);

        protected void ValidateRequiredFieldsForOrder(CreateOrderBuilder order)
        {
            if (order.GetOrderRows() != null && order.GetOrderRows().Count > 0)
            {
                return;
            }

            Errors +=
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n";
        }

        protected void ValidateOrderRow(CreateOrderBuilder order)
        {
            foreach (OrderRowBuilder orderRow in order.GetOrderRows())
            {
                if (orderRow == null)
                {
                    Errors +=
                        "MISSING VALUES - AmountExVat, Quantity and VatPercent are required for Orderrow. Use SetAmountExVat(), SetQuantity() and SetVatPercent().\n";
                    continue;
                }

                if (orderRow.GetQuantity() <= 0)
                {
                    Errors += "MISSING VALUE - Quantity is required in Item object. Use Item.SetQuantity().\n";
                }

                if (orderRow.GetAmountExVat() == null && orderRow.GetVatPercent() == null &&
                    orderRow.GetAmountIncVat() == null)
                {
                    Errors +=
                        "MISSING VALUE - Two of the values must be set: AmountExVat(not set), AmountIncVat(not set) or VatPercent(not set) for Orderrow. Use two of: SetAmountExVat(), SetAmountIncVat or SetVatPercent().\n";
                }
                else if (orderRow.GetAmountExVat() != null && orderRow.GetVatPercent() == null &&
                         orderRow.GetAmountIncVat() == null)
                {
                    Errors +=
                        "MISSING VALUE - At least one of the values must be set in combination with AmountExVat: AmountIncVat or VatPercent for Orderrow. Use one of: SetAmountIncVat() or SetVatPercent().\n";
                }
                else if (orderRow.GetAmountExVat() == null && orderRow.GetVatPercent() == null &&
                         orderRow.GetAmountIncVat() != null)
                {
                    Errors +=
                        "MISSING VALUE - At least one of the values must be set in combination with AmountIncVat: AmountExVat or VatPercent for Orderrow. Use one of: SetAmountExVat() or SetVatPercent().\n";
                }
                else if (orderRow.GetAmountExVat() == null && orderRow.GetVatPercent() != null &&
                         orderRow.GetAmountIncVat() == null)
                {
                    Errors +=
                        "MISSING VALUE - At least one of the values must be set in combination with VatPercent: AmountIncVat or AmountExVat for Orderrow. Use one of: SetAmountExVat() or SetAmountIncVat().\n";
                }
            }
        }
    }
}