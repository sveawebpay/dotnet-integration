using Webpay.Integration.Order.Handle;

namespace Webpay.Integration.Webservice.Handleorder;

public class DeliverPaymentPlan : HandleOrder
{
    public DeliverPaymentPlan(DeliverOrderBuilder orderBuilder) : base(orderBuilder)
    {
    }
}