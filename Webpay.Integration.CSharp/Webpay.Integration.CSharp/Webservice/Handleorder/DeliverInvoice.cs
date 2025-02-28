using Webpay.Integration.CSharp.Order.Handle;

namespace Webpay.Integration.CSharp.Webservice.Handleorder
{
    public class DeliverInvoice : HandleOrder
    {
        public DeliverInvoice(DeliverOrderBuilder orderBuilder) : base(orderBuilder)
        {
        }
    }
}