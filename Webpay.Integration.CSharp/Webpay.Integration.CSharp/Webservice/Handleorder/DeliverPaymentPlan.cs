using Webpay.Integration.CSharp.Order.Handle;

namespace Webpay.Integration.CSharp.Webservice.Handleorder
{
    public class DeliverPaymentPlan : HandleOrder
    {
        public DeliverPaymentPlan(DeliverOrderBuilder orderBuilder) : base(orderBuilder)
        {
        }
    }
}
