namespace Webpay.Integration.CSharp.Order.Row
{
    public class RelativeDiscountBuilder : IRowBuilder
    {
        private string _discountId;
        private string _name;
        private string _description;
        private decimal _discountPercent;
        private string _unit;

        public string GetDiscountId()
        {
            return _discountId;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="discountId"></param>
        /// <returns>RelativeDiscountBuilder</returns>
        public RelativeDiscountBuilder SetDiscountId(string discountId)
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
        /// <returns>RelativeDiscountBuilder</returns>
        public RelativeDiscountBuilder SetName(string name)
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
        /// <returns>RelativeDiscountBuilder</returns>
        public RelativeDiscountBuilder SetDescription(string description)
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
            //For relative discounts the article number is synonomous with the discount id
            return GetDiscountId();
        }

        /// <summary>
        /// SetUnit
        /// </summary>
        /// <param name="unit">i.e. "pcs", "st" etc</param>
        /// <returns>RelativeDiscountBuilder</returns>
        public RelativeDiscountBuilder SetUnit(string unit)
        {
            _unit = unit;
            return this;
        }

        public decimal GetDiscountPercent()
        {
            return _discountPercent;
        }

        public decimal GetQuantity()
        {
            //There can only be one fixed discount per row
            return 1M;
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
        /// <param name="discountPercent"></param>
        /// <returns>RelativeDiscountBuilder</returns>
        public RelativeDiscountBuilder SetDiscountPercent(int discountPercent)
        {
            _discountPercent = discountPercent;
            return this;
        }
    }
}