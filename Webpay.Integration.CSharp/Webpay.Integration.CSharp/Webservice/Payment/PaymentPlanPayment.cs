using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;
using OrderType = Webpay.Integration.CSharp.WebpayWS.OrderType;

namespace Webpay.Integration.CSharp.Webservice.Payment
{
    public class PaymentPlanPayment : WebServicePayment
    {
        public PaymentPlanPayment(CreateOrderBuilder orderBuilder) 
            : base(orderBuilder)
        {
            PayType = PaymentType.PAYMENTPLAN;
        }

        protected override CreateOrderInformation SetOrderType(CreateOrderInformation information)
        {
            if (CrOrderBuilder.GetIsCompanyIdentity() && CrOrderBuilder.GetCompanyCustomer().GetAddressSelector() != null)
                OrderInfo.AddressSelector = CrOrderBuilder.GetCompanyCustomer().GetAddressSelector();
            else
                OrderInfo.AddressSelector = "";

            OrderInfo.OrderType = OrderType.PaymentPlan;
            return OrderInfo;
        }
    }
}
