namespace Webpay.Integration.CSharp.Hosted
{
    public class HostedOrderRowBuilder
    {
        private string _sku;
        private string _name;
        private string _description;
        private long _amount;
        private long _vat;
        private decimal _quantity;
        private string _unit;

        public string GetSku()
        {
            return _sku;
        }

        public HostedOrderRowBuilder SetSku(string sku)
        {
            _sku = sku;
            return this;
        }

        public string GetName()
        {
            return _name;
        }

        public HostedOrderRowBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public string GetDescription()
        {
            return _description;
        }

        public HostedOrderRowBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }

        //AmountIncVat
        public long GetAmount()
        {
            return _amount;
        }

        public HostedOrderRowBuilder SetAmount(long amount)
        {
            _amount = amount;
            return this;
        }

        public long GetVat()
        {
            return _vat;
        }

        public HostedOrderRowBuilder SetVat(long vat)
        {
            _vat = vat;
            return this;
        }

        public decimal GetQuantity()
        {
            return _quantity;
        }

        public HostedOrderRowBuilder SetQuantity(decimal quantity)
        {
            _quantity = quantity;
            return this;
        }

        /// <summary>
        /// GetUnit
        /// </summary>
        /// <returns>i.e. "pcs", "st" etc</returns>/
        public string GetUnit()
        {
            return _unit;
        }

        /// <summary>
        /// SetUnit
        /// </summary>
        /// <param name="unit">i.e. "pcs", "st" etc</param>
        /// <returns>HostedOrderRowBuilder</returns>
        public HostedOrderRowBuilder SetUnit(string unit)
        {
            _unit = unit;
            return this;
        }
    }
}