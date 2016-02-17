using Webpay.Integration.CSharp.AdminService;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Admin.Actions;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class CreditAmountBuilder : Builder<CreditAmountBuilder>
    {
        private long _orderId;
        internal string Description { get; private set; }
        internal decimal AmountIncVat { get; private set; }
        public PaymentType OrderType { get; set; }

        public CreditAmountBuilder(IConfigurationProvider config) : base(config)
        {
            // intentionally left blank
        }

        public CreditAmountBuilder SetContractNumber(long orderId)
        {
            _orderId = orderId;
            return this;
        }

        public CreditAmountBuilder SetTransactionId(long orderId)
        {
            return this.SetContractNumber(orderId);
        }

        public long GetContractNumber()
        {
            return _orderId;
        }

        public long GetTransactionId()
        {
            return _orderId;
        }
        public override CreditAmountBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public CreditAmountBuilder SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public CreditAmountBuilder SetAmountIncVat(decimal amountIncVat)
        {
            AmountIncVat = amountIncVat;
            return this;
        }

        public AdminService.CreditAmountRequest CreditPaymentPlanAmount()
        {
            OrderType = PaymentType.PAYMENTPLAN;
            return new AdminService.CreditAmountRequest(this);
        }

        public AdminService.CreditTransactionRequest CreditCardAmount()
        {
            return new AdminService.CreditTransactionRequest(this);
        }
    }
}