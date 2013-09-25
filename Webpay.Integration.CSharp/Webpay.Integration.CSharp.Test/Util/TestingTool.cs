using Webpay.Integration.CSharp.Order.Row;

namespace Webpay.Integration.CSharp.Test.Util
{
    public static class TestingTool
    {
        public static OrderRowBuilder CreateOrderRow()
        {
            return Item.OrderRow()
                    .SetArticleNumber("1")
                    .SetName("Prod")
                    .SetDescription("Specification")
                    .SetAmountExVat(new decimal(100.00))
                    .SetQuantity(2)
                    .SetUnit("st")
                    .SetVatPercent(25)
                    .SetVatDiscount(0);
        }

        public static OrderRowBuilder CreateOrderRowDe()
        {
            return Item.OrderRow()
                    .SetArticleNumber("1")
                    .SetName("Prod")
                    .SetDescription("Specification")
                    .SetAmountExVat(new decimal(100.00))
                    .SetQuantity(2)
                    .SetUnit("st")
                    .SetVatPercent(19)
                    .SetVatDiscount(0);
        }

        public static OrderRowBuilder CreateOrderRowNl()
        {
            return Item.OrderRow()
                    .SetArticleNumber("1")
                    .SetName("Prod")
                    .SetDescription("Specification")
                    .SetAmountExVat(new decimal(100.00))
                    .SetQuantity(2)
                    .SetUnit("st")
                    .SetVatPercent(6)
                    .SetVatDiscount(0);
        }
    }
}
