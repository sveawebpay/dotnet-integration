namespace Sample.AspNetCore.Models.ViewModels;

public class OrderViewModel
{
    public OrderViewModel(long orderId)
    {
        OrderId = orderId;
    }

    public long OrderId { get; set; }
    public bool IsLoaded { get; set; }
    public Order Order { get; set; }
    public string ShippingStatus { get; set; }
    public string ShippingDescription { get; set; }
}