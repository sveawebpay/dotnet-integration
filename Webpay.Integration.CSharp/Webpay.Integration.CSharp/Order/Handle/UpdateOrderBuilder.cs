using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class UpdateOrderBuilder : Builder<UpdateOrderBuilder>
    {
        internal long SveaOrderId { get; private set; }
        internal PaymentType OrderType { get; private set; }
        internal string ClientOrderNumber { get; private set; }
        internal string Notes { get; private set; }

        public UpdateOrderBuilder(IConfigurationProvider config) : base(config)
        {}

        public UpdateOrderBuilder SetOrderId(long orderId)
        {
            SveaOrderId = orderId;
            return this;
        }

        public override UpdateOrderBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public UpdateOrderBuilder SetClientOrderNumber(string clientOrderNumber)
        {
            ClientOrderNumber = clientOrderNumber;
            return this;
        }

        public UpdateOrderBuilder SetNotes(string notes)
        {
            Notes = notes;
            return this;
        }

        public AdminService.UpdateOrderRequest UpdateInvoiceOrder()
        {
            OrderType = PaymentType.INVOICE;
            return new AdminService.UpdateOrderRequest(this);
        }

        public AdminService.UpdateOrderRequest UpdatePaymentPlanOrder()
        {
            OrderType = PaymentType.PAYMENTPLAN;
            return new AdminService.UpdateOrderRequest(this);
        }

        public override UpdateOrderBuilder SetCorrelationId(Guid? correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}
