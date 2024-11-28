using System.Collections.Generic;

namespace Sample.AspNetCore.Models.ViewModels;

public class OrderListViewModel
{
    public List<OrderViewModel> PaymentOrders { get; set; }
}