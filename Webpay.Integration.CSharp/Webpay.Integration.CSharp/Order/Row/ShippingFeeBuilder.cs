namespace Webpay.Integration.CSharp.Order.Row
{
    public class ShippingFeeBuilder : IRowBuilder, IPriced<ShippingFeeBuilder>
    {
        private string _shippingId;
        private string _name;
        private string _description;
        private decimal? _amountExVat;
        private decimal? _amountIncVat;
        private decimal? _vatPercent;
        private string _unit;
        private int _discountPercent;

        public string GetShippingId()
        {
            return _shippingId;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ShippingFeeBuilder</returns>
        public ShippingFeeBuilder SetShippingId(string id)
        {
            _shippingId = id;
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
        /// <returns>ShippingFeeBuilder</returns>
        public ShippingFeeBuilder SetName(string name)
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
        /// <returns>ShippingFeeBuilder</returns>
        public ShippingFeeBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }

        public decimal GetQuantity()
        {
            //There can only be one shipping fee per row
            return 1M;
        }

        public decimal? GetAmountExVat()
        {
            return _amountExVat;
        }

        /// <summary>
        /// Optional
        /// Required to use at least two of the methods setAmountExVat(), setAmountIncVat() or setVatPercent()
        /// </summary>
        /// <param name="amountExVat"></param>
        /// <returns>ShippingFeeBuilder</returns>
        public ShippingFeeBuilder SetAmountExVat(decimal amountExVat)
        {
            _amountExVat = amountExVat;
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
        /// <returns>ShippingFeeBuilder</returns>
        public ShippingFeeBuilder SetVatPercent(decimal vatPercent)
        {
            _vatPercent = vatPercent;
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
            //For shippping fees the article number is synonymous with the shipping Id
            return GetShippingId();
        }

        /// <summary>
        /// SetUnit
        /// </summary>
        /// <param name="unit">i.e. "pcs", "st" etc</param>
        /// <returns>ShippingFeeBuilder</returns>
        public ShippingFeeBuilder SetUnit(string unit)
        {
            _unit = unit;
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
        /// <returns>ShippingFeeBuilder</returns>
        public ShippingFeeBuilder SetDiscountPercent(int discountPercent)
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
        /// <returns>ShippingFeeBuilder</returns>
        public ShippingFeeBuilder SetAmountIncVat(decimal amountIncVat)
        {
            _amountIncVat = amountIncVat;
            return this;
        }
    }
}