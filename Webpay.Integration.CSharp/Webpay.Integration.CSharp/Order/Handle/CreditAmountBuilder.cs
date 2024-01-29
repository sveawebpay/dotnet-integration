using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class CreditAmountBuilder : Builder<CreditAmountBuilder>
    {
        internal long Id { get; private set; }
        internal string Description { get; private set; }
        internal decimal AmountIncVat { get; private set; }
        internal PaymentType OrderType { get; private set; }

        public CreditAmountBuilder(IConfigurationProvider config) : base(config)
        {
            // intentionally left blank
        }

        public CreditAmountBuilder SetContractNumber(long orderId)
        {
            Id = orderId;
            return this;
        }

        public CreditAmountBuilder SetTransactionId(long orderId)
        {
            return this.SetContractNumber(orderId);
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

        public AdminService.CreditTransactionRequest CreditDirectBankAmount()
        {
            return new AdminService.CreditTransactionRequest(this);
        }

        public override CreditAmountBuilder SetCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}