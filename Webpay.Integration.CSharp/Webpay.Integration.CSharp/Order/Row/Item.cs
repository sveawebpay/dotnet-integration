using Webpay.Integration.CSharp.Order.Identity;

namespace Webpay.Integration.CSharp.Order.Row
{
    /// <summary>
    /// Use to specify types of customer, fees, discounts or row.
    /// </summary>
    public static class Item
    {
        public static OrderRowBuilder OrderRow()
        {
            return new OrderRowBuilder();
        }

        public static IndividualCustomer IndividualCustomer()
        {
            return new IndividualCustomer();
        }

        public static CompanyCustomer CompanyCustomer()
        {
            return new CompanyCustomer();
        }

        public static ShippingFeeBuilder ShippingFee()
        {
            return new ShippingFeeBuilder();
        }

        public static InvoiceFeeBuilder InvoiceFee()
        {
            return new InvoiceFeeBuilder();
        }

        public static FixedDiscountBuilder FixedDiscount()
        {
            return new FixedDiscountBuilder();
        }

        public static RelativeDiscountBuilder RelativeDiscount()
        {
            return new RelativeDiscountBuilder();
        }
    }
}