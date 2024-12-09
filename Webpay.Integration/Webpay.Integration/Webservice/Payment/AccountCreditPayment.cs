using Webpay.Integration.Order.Create;
using Webpay.Integration.Util.Constant;
using WebpayWS;
using OrderType = WebpayWS.OrderType;

namespace Webpay.Integration.Webservice.Payment;

public class AccountCreditPayment : WebServicePayment
{
    public AccountCreditPayment(CreateOrderBuilder orderBuilder)
        : base(orderBuilder)
    {
        PayType = PaymentType.ACCOUNTCREDIT;
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

        OrderInfo.OrderType = OrderType.AccountCredit;
        return OrderInfo;
    }
}
