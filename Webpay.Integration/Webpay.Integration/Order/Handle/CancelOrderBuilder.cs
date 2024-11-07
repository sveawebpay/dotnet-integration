using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Handle;

public class CancelOrderBuilder : Builder<CancelOrderBuilder>
{
    internal long Id { get; private set; }
    internal PaymentType OrderType { get; private set; }

    public CancelOrderBuilder(IConfigurationProvider config) : base(config)
    {
    }

    public CancelOrderBuilder SetOrderId(long orderId)
    {
        Id = orderId;
        return this;
    }
    public CancelOrderBuilder SetTransactionId(long orderId)
    {
        return this.SetOrderId(orderId);
    }

    public override CancelOrderBuilder SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public AdminService.CancelOrderRequest CancelInvoiceOrder()
    {
        OrderType = PaymentType.INVOICE;
        return new AdminService.CancelOrderRequest(this);
    }

    public AdminService.CancelOrderRequest CancelPaymentPlanOrder()
    {
        OrderType = PaymentType.PAYMENTPLAN;
        return new AdminService.CancelOrderRequest(this);
    }

    public AdminService.AnnulTransactionRequest CancelCardOrder()
    {
        return new AdminService.AnnulTransactionRequest(this);
    }

    public override CancelOrderBuilder SetCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
}