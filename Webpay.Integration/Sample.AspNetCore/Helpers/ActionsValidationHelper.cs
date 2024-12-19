using AdminWS;
using System.Linq;

namespace Sample.AspNetCore.Helpers;

public static  class ActionsValidationHelper
{
    public static string ValidateOrderAction(Order order, string orderAction)
    {
        if (order == null)
        {
            return "Payment Order does not exist";
        }

        if (orderAction == null)
        {
            return null;
        }

        //if (!order.AvailableActions.Contains(orderAction))
        //{
        //    return "Operation not available";
        //}

        return null;
    }

    public static string ValidateOrderRowAction(Order order, long orderRowId, string orderRowAction)
    {
        var orderError = ValidateOrderAction(order, null);

        if(orderError != null)
        {
            return orderError;
        }

        // TODO
        var orderRow = order.OrderRows.FirstOrDefault(row => row.InvoiceId == orderRowId);

        if (orderRow == null)
        {
            return "Order row does not exist";
        }

        if (orderRowAction == null)
        {
            return null;
        }

        //if (!orderRow.AvailableActions.Contains(orderRowAction))
        //{
        //    return "Operation not available";
        //}

        return null;
    }

    public static string ValidateDeliveryAction(Order order, long deliveryId, string deliveryAction)
    {
        var orderError = ValidateOrderAction(order, null);

        if (orderError != null)
        {
            return orderError;
        }

        // TODO
        //var delivery = order.Deliveries.FirstOrDefault(dlv => dlv.Id == deliveryId);

        return "Order row does not exist";
        //if (delivery == null)
        //{
        //    return "Order row does not exist";
        //}

        //if (deliveryAction == null)
        //{
        //    return null;
        //}

        //if (!delivery.AvailableActions.Contains(deliveryAction))
        //{
        //    return "Operation not available";
        //}

        return null;
    }
}
