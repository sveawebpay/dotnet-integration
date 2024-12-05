using Webpay.Integration.Util.Constant;

namespace Sample.AspNetCore.Models.ViewModels;

public class OrderViewModel
{
    public OrderViewModel(long orderId)
    {
        OrderId = orderId;
    }

    public long OrderId { get; set; }
    public bool IsLoaded { get; set; }
    public AdminWS.Order Order { get; set; }
    public PaymentType PaymentType { get; set; }
}