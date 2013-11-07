namespace Webpay.Integration.CSharp.Order.Row
{
    public class OrderRowBuilder : IRowBuilder
    {
        private string _articleNumber;
        private string _name;
        private string _description;
        private decimal? _amountExVat;
        private decimal? _amountIncVat;
        private decimal? _vatPercent;
        private decimal _quantity;
        private string _unit;
        private int _vatDiscount;
        private int _discountPercent;

        public string GetArticleNumber()
        {
            return _articleNumber;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="articleNumber"></param>
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetArticleNumber(string articleNumber)
        {
            _articleNumber = articleNumber;
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
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetName(string name)
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
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }

        public decimal? GetAmountExVat()
        {
            return _amountExVat;
        }

        /// <summary>
        /// Optional
        /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
        /// </summary>
        /// <param name="dExVatAmount"></param>
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetAmountExVat(decimal dExVatAmount)
        {
            _amountExVat = dExVatAmount;
            return this;
        }

        public decimal? GetVatPercent()
        {
            return _vatPercent;
        }

        /// <summary>
        /// Optional
        /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
        /// </summary>
        /// <param name="vatPercent"></param>
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetVatPercent(decimal vatPercent)
        {
            _vatPercent = vatPercent;
            return this;
        }

        public decimal GetQuantity()
        {
            return _quantity;
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetQuantity(decimal quantity)
        {
            _quantity = quantity;
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

        /// <summary>
        /// SetUnit
        /// </summary>
        /// <param name="unit">i.e. "pcs", "st" etc</param>
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetUnit(string unit)
        {
            _unit = unit;
            return this;
        }

        public int GetVatDiscount()
        {
            return _vatDiscount;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="vatDiscount"></param>
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetVatDiscount(int vatDiscount)
        {
            _vatDiscount = vatDiscount;
            return this;
        }

        public decimal GetDiscountPercent()
        {
            return _discountPercent;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="discountPercent"></param>
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetDiscountPercent(int discountPercent)
        {
            _discountPercent = discountPercent;
            return this;
        }

        public decimal? GetAmountIncVat()
        {
            return _amountIncVat;
        }

        /// <summary>
        /// Optional
        /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
        /// </summary>
        /// <param name="amountIncVat"></param>
        /// <returns>OrderRowBuilder</returns>
        public OrderRowBuilder SetAmountIncVat(decimal amountIncVat)
        {
            _amountIncVat = amountIncVat;
            return this;
        }
    }
}