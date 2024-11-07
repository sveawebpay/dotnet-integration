using Webpay.Integration.Order.Create;
using Webpay.Integration.Util.Constant;
using WebpayWS;
using OrderType = WebpayWS.OrderType;

namespace Webpay.Integration.Webservice.Payment;

public class InvoicePayment : WebServicePayment
{
    public InvoicePayment(CreateOrderBuilder orderBuilder) : base(orderBuilder)
    {
        PayType = PaymentType.INVOICE;
    }

    protected override CreateOrderInformation SetOrderType(CreateOrderInformation information)
    {
        if (CrOrderBuilder.GetIsCompanyIdentity() &&
            CrOrderBuilder.GetCompanyCustomer().GetAddressSelector() != null)
        {
            OrderInfo.AddressSelector = CrOrderBuilder.GetCompanyCustomer().GetAddressSelector();
        }
        else
        {
            OrderInfo.AddressSelector = "";
        }

        OrderInfo.OrderType = OrderType.Invoice;
        return OrderInfo;
    }
}