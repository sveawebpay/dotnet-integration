namespace Webpay.Integration.CSharp.Order.Row
{
    public class InvoiceFeeBuilder : IRowBuilder
    {
        private string _name;
        private string _description;
        private decimal? _amountExVat;
        private decimal? _amountIncVat;
        private decimal? _vatPercent;
        private string _unit;
        private int _discountPercent;

        public string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="name"></param>
        /// <returns>InvoiceFeeBuilder</returns>
        public InvoiceFeeBuilder SetName(string name)
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
        /// <returns>InvoiceFeeBuilder</returns>
        public InvoiceFeeBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }

        public decimal GetQuantity()
        {
            //There can only be one invoice fee per row
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
        /// <returns>InvoiceFeeBuilder</returns>
        public InvoiceFeeBuilder SetAmountExVat(decimal amountExVat)
        {
            _amountExVat = amountExVat;
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
            //Invoice fees have no article number
            return "";
        }

        /// <summary>
        /// SetUnit
        /// </summary>
        /// <param name="unit">i.e. "pcs", "st" etc</param>
        /// <returns>InvoiceFeeBuilder</returns>
        public InvoiceFeeBuilder SetUnit(string unit)
        {
            _unit = unit;
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
        /// <returns>InvoiceFeeBuilder</returns>
        public InvoiceFeeBuilder SetVatPercent(decimal vatPercent)
        {
            _vatPercent = vatPercent;
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
        /// <returns>InvoiceFeeBuilder</returns>
        public InvoiceFeeBuilder SetDiscountPercent(int discountPercent)
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
        /// <returns>InvoiceFeeBuilder</returns>
        public InvoiceFeeBuilder SetAmountIncVat(decimal amountIncVat)
        {
            _amountIncVat = amountIncVat;
            return this;
        }
    }
}