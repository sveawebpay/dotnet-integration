namespace Webpay.Integration.CSharp.Order.Row
{
    public class FixedDiscountBuilder : IRowBuilder
    {
        private string _discountId;
        private string _name;
        private string _description;
        private string _unit = "";
        private decimal _amount;

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
            //For fixed discounts the article number is synonomous with the discount id
            return GetDiscountId();
        }

        public int GetDiscountPercent()
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

        public decimal GetAmount()
        {
            return _amount;
        }

        /// <summary>
        /// Do not use.
        /// Will return null.
        /// </summary>
        /// <returns>null</returns>
        public decimal? GetAmountExVat()
        {
            return null;
        }

        /// <summary>
        /// Do not use.
        /// Will return null.
        /// </summary>
        /// <returns>null</returns>
        public decimal? GetVatPercent()
        {
            return null;
        }

        /// <summary>
        /// Do not use.
        /// Will return null.
        /// </summary>
        /// <returns>null</returns>
        public decimal? GetAmountIncVat()
        {
            return null;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="amountDiscountOnTotalPrice"></param>
        /// <returns>FixedDiscountBuilder</returns>
        public FixedDiscountBuilder SetAmountIncVat(decimal amountDiscountOnTotalPrice)
        {
            _amount = amountDiscountOnTotalPrice;
            return this;
        }
    }
}