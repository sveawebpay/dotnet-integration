namespace Webpay.Integration.CSharp.Order.Row
{
    public class FixedDiscountBuilder : IRowBuilder
    {
        private string _discountId;
        private string _name;
        private string _description;
        private string _unit = "";
        private decimal? _amountExVat;
        private decimal? _amountIncVat;
        private decimal? _vatPercent;

        public string GetDiscountId()
        {
            return _discountId;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="discountId"></param>
        /// <returns>FixedDiscountBuilder</returns>
        public FixedDiscountBuilder SetDiscountId(string discountId)
        {
            _discountId = discountId;
            return this;
        }

        public string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="name"></param>
        /// <returns>FixedDiscountBuilder</returns>
        public FixedDiscountBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public string GetDescription()
        {
            return _description;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="description"></param>
        /// <returns>FixedDiscountBuilder</returns>
        public FixedDiscountBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }

        /// <summary>
        /// GetUnit
        /// </summary>
        /// <returns>i.e. "pcs", "st" etc</returns>
        public string GetUnit()
        {
            return _unit;
        }

        public string GetArticleNumber()
        {
            //For fixed discounts the article number is synonymous with the discount id
            return GetDiscountId();
        }

        public decimal GetDiscountPercent()
        {
            //We do not give discounts on discounts
            return 0;
        }

        public decimal GetQuantity()
        {
            //There can only be one fixed discount per row
            return 1M;
        }

        /// <summary>
        /// SetUnit
        /// </summary>
        /// <param name="unit">i.e. "pcs", "st" etc</param>
        /// <returns>FixedDiscountBuilder</returns>
        public FixedDiscountBuilder SetUnit(string unit)
        {
            _unit = unit;
            return this;
        }

        public decimal? GetAmountExVat()
        {
            return _amountExVat;
        }

        public decimal? GetVatPercent()
        {
            return _vatPercent;
        }

        public decimal? GetAmountIncVat()
        {
            return _amountIncVat;
        }

        /// <summary>
        /// If only AmountIncVat is given, for Invoice and Payment plan payment methods we calculate the discount split across the tax (vat) rates present 
        /// in the order. This will ensure that the correct discount vat is applied to the order. This means that the discount will show up split across 
        /// multiple rows on the invoice, one for each tax rate present in the order.
        /// 
        /// For Card and Direct bank payments we only subtract the appropriate amount from the request, but we still honour the specified percentage, if
        /// given using two the functions below. 
        /// 
        /// Otherwise, it is required to use precisely two of the functions setAmountExVat(), setAmountIncVat() and setVatPercent().
        /// If two of these three attributes are specified, we respect the amount indicated and include the discount with the specified tax rate.
        /// </summary>
        /// <param name="amountDiscountOnTotalPrice"></param>
        /// <returns>FixedDiscountBuilder</returns>
        public FixedDiscountBuilder SetAmountIncVat(decimal amountDiscountOnTotalPrice)
        {
            _amountIncVat = amountDiscountOnTotalPrice;
            return this;
        }

        /// <summary>
        /// Required to use at least two of the functions setAmountExVat(), setAmountIncVat(), setVatPercent().
        /// If two of these three attributes are specified, we respect the amount indicated and include a discount with the appropriate tax rate.
        /// </summary>
        /// <param name="vatPercent"></param>
        /// <returns>FixedDiscountBuilder</returns>
        public FixedDiscountBuilder SetVatPercent(decimal vatPercent)
        {
            _vatPercent = vatPercent;
            return this;
        }

        /// <summary>
        /// Required to use at least two of the functions setAmountExVat(), setAmountIncVat(), setVatPercent().
        /// </summary>
        /// <param name="amountDiscountExVat"></param>
        /// <returns>FixedDiscountBuilder</returns>
        public FixedDiscountBuilder SetAmountExVat(decimal amountDiscountExVat)
        {
            _amountExVat = amountDiscountExVat;
            return this;
        }
    }
}