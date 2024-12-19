using Webpay.Integration.Order.Handle;

namespace Webpay.Integration.Webservice.Handleorder;

public class DeliverInvoice : HandleOrder
{
    public DeliverInvoice(DeliverOrderBuilder orderBuilder) : base(orderBuilder)
    {
    }
}