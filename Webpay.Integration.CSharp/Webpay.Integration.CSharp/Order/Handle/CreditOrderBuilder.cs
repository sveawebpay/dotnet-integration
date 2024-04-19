using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Row.credit;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    // For direct bank , mobile , card payments
    public class CreditOrderBuilder : Builder<CreditOrderBuilder>
    {
        internal long Id { get; private set; }
        internal string Description { get; private set; }
        internal decimal AmountIncVat { get; private set; }
        internal List<Delivery> Deliveries { get; private set; }
        internal PaymentType OrderType { get; private set; }

        public CreditOrderBuilder(IConfigurationProvider config) : base(config)
        {
            Deliveries = new List<Delivery>();
        }
        public CreditOrderBuilder AddDeliveries(List<Delivery> deliveries)
        {
            Deliveries.AddRange(deliveries);
            return this;
        }
       
        public CreditOrderBuilder SetContractNumber(long orderId)
        {
            Id = orderId;
            return this;
        }

        public CreditOrderBuilder SetTransactionId(long orderId)
        {
            return this.SetContractNumber(orderId);
        }

        public override CreditOrderBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public CreditOrderBuilder SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public CreditOrderBuilder SetAmountIncVat(decimal amountIncVat)
        {
            AmountIncVat = amountIncVat;
            return this;
        }

        public AdminService.CreditAmountRequest CreditPaymentPlanAmount()
        {
            OrderType = PaymentType.PAYMENTPLAN;
            return new AdminService.CreditAmountRequest(this);
        }

        public AdminService.CreditTransactionRequest CreditCardPayment()
        {
            return new AdminService.CreditTransactionRequest(this);
        }

        public AdminService.CreditTransactionRequest CreditDirectBankPayment()
        {
            return new AdminService.CreditTransactionRequest(this);
        }

        public override CreditOrderBuilder SetCorrelationId(Guid? correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}