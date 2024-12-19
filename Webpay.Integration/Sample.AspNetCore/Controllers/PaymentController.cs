using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Helpers;

namespace Sample.AspNetCore.Controllers;

public class PaymentController : Controller
{

    public PaymentController()
    {
    }

    //#region Order

    //[HttpGet]
    //public async Task<ActionResult> Cancel(long paymentId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanCancelOrder);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            await paymentOrder.Actions.Cancel(new CancelRequest(true)).ConfigureAwait(false);

    //            TempData["CancelMessage"] = $"Payment has been cancelled: {paymentId}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> CancelOrderRows(long paymentId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanCancelOrderRow);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var orderRow = paymentOrder.OrderRows.FirstOrDefault(x => x.AvailableActions.Contains(OrderRowActionType.CanCancelRow));

    //            if (orderRow == null)
    //            {
    //                throw new Exception();
    //            }

    //            await paymentOrder.Actions.CancelOrderRows(new CancelOrderRowsRequest(new long[] { orderRow.OrderRowId })).ConfigureAwait(false);

    //            TempData["CancelMessage"] = $"Order row with id : {orderRow.OrderRowId} has been cancelled.";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpPost]
    //public async Task<ActionResult> AddOrderRow(long paymentId, int? amountDiscountOrderRow, int? percentDiscountOrderRow)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        var discount = amountDiscountOrderRow.HasValue ? amountDiscountOrderRow : percentDiscountOrderRow.HasValue ? percentDiscountOrderRow : 0;
    //        var useDiscountPercent = !amountDiscountOrderRow.HasValue && (percentDiscountOrderRow.HasValue);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanAddOrderRow);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var response = await paymentOrder.Actions.AddOrderRow(
    //                new AddOrderRowRequest(
    //                    articleNumber: "1234567890",
    //                    name: "Slim Fit 512",
    //                    quantity: 2,
    //                    unitPrice: 1000,
    //                    discount: discount.Value,
    //                    vatPercent: 12,
    //                    unit: "SEK",
    //                    useDiscountPercent
    //                ),
    //                new PollingTimeout(15)
    //            ).ConfigureAwait(false);

    //            TempData["OrderRowMessage"] = $"Order row has been added -> {response.ResourceUri.AbsoluteUri}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpPost]
    //public async Task<ActionResult> AddOrderRows(long paymentId, int? amountDiscountOrderRows, int? percentDiscountOrderRows)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        var discount1 = percentDiscountOrderRows.HasValue ? percentDiscountOrderRows.Value : 0;
    //        var useDiscountPercent1 = percentDiscountOrderRows.HasValue;

    //        var discount2 = amountDiscountOrderRows.HasValue ? amountDiscountOrderRows.Value : 0;
    //        var useDiscountPercent2 = false;

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanAddOrderRow);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var newOrderRows = new List<NewOrderRow>
    //            {
    //                 new NewOrderRow(
    //                    name: "Slim Fit 512",
    //                    quantity: 2,
    //                    unitPrice: 1000,
    //                    vatPercent: 12,
    //                    discount: discount1,
    //                    rowId: null,
    //                    unit: "SEK",
    //                    articleNumber: "0987654321",
    //                    useDiscountPercent1
    //                ),
    //                new NewOrderRow(
    //                    name: "Slim Fit 513",
    //                    quantity: 2,
    //                    unitPrice: 1000,
    //                    vatPercent: 12,
    //                    discount: discount2,
    //                    rowId: null,
    //                    unit: "SEK",
    //                    articleNumber: "1234567890",
    //                    useDiscountPercent2
    //                )
    //            };

    //            var response = await paymentOrder.Actions.AddOrderRows(new AddOrderRowsRequest(newOrderRows), new PollingTimeout(15)).ConfigureAwait(false);

    //            TempData["OrderRowMessage"] = $"Order row has been added -> {string.Join(", ", response.ResourceUri.AbsoluteUri) }";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> CancelAmount(long paymentId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanCancelAmount);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            await paymentOrder.Actions.CancelAmount(new CancelAmountRequest(1)).ConfigureAwait(false);
    //            TempData["CancelMessage"] = $"Cancelling parts of the total amount: {paymentId}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<IActionResult> DeliverOrder(long paymentId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanDeliverOrder);
    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var orderRowIds = paymentOrder.OrderRows.Select(row => (long)row.OrderRowId).ToList();

    //            var response = await paymentOrder.Actions.DeliverOrder(
    //                new DeliveryRequest(orderRowIds, null, null, true), new PollingTimeout(15)).ConfigureAwait(false);

    //            TempData["DeliverMessage"] = $"Order delivered -> {response.ResourceUri.AbsoluteUri}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> DeliverOrderPartially(long paymentId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanDeliverOrderPartially);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var orderRow = new List<long> { paymentOrder.OrderRows.First(x => x.AvailableActions.Contains(OrderRowActionType.CanDeliverRow)).OrderRowId };
    //            var deliverRequest = new DeliveryRequest(orderRow);
    //            var response = await paymentOrder.Actions.DeliverOrder(deliverRequest, new PollingTimeout(15)).ConfigureAwait(false);

    //            TempData["DeliverMessage"] = $"Order delivered partially -> {response.ResourceUri.AbsoluteUri}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> UpdateOrderRows(long paymentId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanAddOrderRow);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var existingOrderRows = paymentOrder.OrderRows;

    //            var newOrderRows = new List<NewOrderRow>();
    //            foreach (var orderRow in existingOrderRows)
    //            {
    //                var discount = orderRow.DiscountAmount != 0 ? orderRow.DiscountAmount : orderRow.DiscountPercent;
    //                var useDiscountPercent = orderRow.DiscountAmount != 0 ? false : true;

    //                newOrderRows.Add(new NewOrderRow(
    //                    orderRow.Name,
    //                  (orderRow.Quantity + 1) % 4 + 1,
    //                    orderRow.UnitPrice,
    //                    orderRow.VatPercent,
    //                    discount,
    //                    orderRow.OrderRowId,
    //                    orderRow.Unit,
    //                    orderRow.ArticleNumber,
    //                    useDiscountPercent
    //                ));
    //            }

    //            await paymentOrder.Actions.UpdateOrderRows(new UpdateOrderRowsRequest(newOrderRows)).ConfigureAwait(false);

    //            TempData["OrderRowMessage"] = "Order rows have been updated";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> ReplaceOrderRows(long paymentId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanAddOrderRow);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var newOrderRows = new List<NewOrderRow>
    //            {
    //                new NewOrderRow(
    //                    name: "Slim Fit 512",
    //                        quantity: 2,
    //                    unitPrice: 100,
    //                    vatPercent: 12,
    //                    discount: 0,
    //                    rowId: null,
    //                    unit: "SEK",
    //                    articleNumber: "1234567890"
    //                ),
    //                new NewOrderRow(
    //                    name: "Slim Fit 513",
    //                    quantity:3,
    //                    unitPrice: 200,
    //                    vatPercent: 25,
    //                    0,
    //                    rowId: null,
    //                    unit: "SEK",
    //                    articleNumber: "0987654321"
    //                )
    //            };

    //            await paymentOrder.Actions.ReplaceOrderRows(new ReplaceOrderRowsRequest(newOrderRows)).ConfigureAwait(false);

    //            TempData["OrderRowMessage"] = "Order rows have been updated";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //#endregion Order

    //#region OrderRow

    //[HttpGet]
    //public async Task<ActionResult> CancelOrderRow(long paymentId, long orderRowId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderRowAction(paymentOrder, orderRowId, OrderRowActionType.CanCancelRow);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var orderRow = paymentOrder.OrderRows.FirstOrDefault(row => row.OrderRowId == orderRowId);
    //            await orderRow.Actions.CancelOrderRow(new CancelRequest(true)).ConfigureAwait(false);

    //            TempData["CancelMessage"] = $"Order row cancelled. Order row id: {orderRow.OrderRowId}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> UpdateOrderRow(long paymentId, long orderRowId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //      
    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderRowAction(paymentOrder, orderRowId, OrderRowActionType.CanUpdateRow);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var orderRow = paymentOrder.OrderRows.FirstOrDefault(row => row.OrderRowId == orderRowId);

    //            var discount = orderRow.DiscountAmount != 0 ? orderRow.DiscountAmount : orderRow.DiscountPercent;
    //            var useDiscountPercent = orderRow.DiscountAmount != 0 ? false : true;

    //            await orderRow.Actions.UpdateOrderRow(
    //                new UpdateOrderRowRequest(
    //                    orderRow.ArticleNumber,
    //                    orderRow.Name + " Updated",
    //                    orderRow.Quantity,
    //                    orderRow.UnitPrice,
    //                    discount,
    //                    orderRow.VatPercent,
    //                    orderRow.Unit,
    //                    useDiscountPercent
    //                )
    //            ).ConfigureAwait(false);

    //            TempData["CancelMessage"] = $"Order row updated. Order row id: {orderRow.OrderRowId}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpPost]
    //public async Task<ActionResult> DeliverOrderRow(long paymentId, long orderRowId, int? quantity)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateOrderAction(paymentOrder, OrderActionType.CanDeliverOrder);
    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var orderRow = paymentOrder.OrderRows.First(x => x.OrderRowId == orderRowId);

    //            quantity = quantity != null ? quantity : int.Parse((orderRow.Quantity.InLowestMonetaryUnit / 100).ToString());

    //            var deliveryOptions = new List<RowDeliveryOptions> { new RowDeliveryOptions(orderRow.OrderRowId, quantity) };

    //            var response = await paymentOrder.Actions.DeliverOrder(
    //                new DeliveryRequest(new List<long> { orderRow.OrderRowId }, rowDeliveryOptions: deliveryOptions), new PollingTimeout(15)).ConfigureAwait(false);

    //            TempData["DeliverMessage"] = $"Order delivered -> {response.ResourceUri.AbsoluteUri}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //#endregion OrderRow

    //#region Delivery

    //[HttpGet]
    //public async Task<ActionResult> CreditAmount(long paymentId, long deliveryId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateDeliveryAction(paymentOrder, deliveryId, DeliveryActionType.CanCreditAmount);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var delivery = paymentOrder.Deliveries.FirstOrDefault(dlv => dlv.Id == deliveryId);

    //            var response = await delivery.Actions.CreditAmount(new CreditAmountRequest(100)).ConfigureAwait(false);

    //            TempData["CreditMessage"] = $"Delivery credited. Credit id: {response.CreditId}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> CreditNewRow(long paymentId, long deliveryId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateDeliveryAction(paymentOrder, deliveryId, DeliveryActionType.CanCreditNewRow);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var delivery = paymentOrder.Deliveries.FirstOrDefault(dlv => dlv.Id == deliveryId);

    //            var response = await delivery.Actions.CreditNewRow(
    //                new CreditNewOrderRowRequest(
    //                    new CreditOrderRow(
    //                        name: "Slim Fit 512",
    //                        100, 12, 1)
    //                ), new PollingTimeout(15)).ConfigureAwait(false);

    //            TempData["CreditMessage"] = $"New credit row created. -> {response.ResourceUri.AbsoluteUri}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> CreditOrderRows(long paymentId, long deliveryId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateDeliveryAction(paymentOrder, deliveryId, DeliveryActionType.CanCreditOrderRows);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var delivery = paymentOrder.Deliveries.FirstOrDefault(dlv => dlv.Id == deliveryId);
    //            var orderRowIds = delivery.OrderRows.Where(row => row.AvailableActions.Contains(OrderRowActionType.CanCreditRow)).Select(row => (long)row.OrderRowId).ToList();
    //            var creditingOptions = new List<RowCreditingOptions> { new RowCreditingOptions(orderRowIds.First(), 1) };

    //            var response = await delivery.Actions.CreditOrderRows(
    //                new CreditOrderRowsRequest(orderRowIds, creditingOptions), new PollingTimeout(15)).ConfigureAwait(false);

    //            TempData["CreditMessage"] = $"Delivery order rows credited. -> {response.ResourceUri.AbsoluteUri}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //[HttpGet]
    //public async Task<ActionResult> CreditOrderRowsWithFee(long paymentId, long deliveryId)
    //{
    //    try
    //    {
    //        var paymentOrder = await this._sveaClient.PaymentAdmin.GetOrder(paymentId).ConfigureAwait(false);

    //        TempData["ErrorMessage"] = ActionsValidationHelper.ValidateDeliveryAction(paymentOrder, deliveryId, DeliveryActionType.CanCreditOrderRows);

    //        if (TempData["ErrorMessage"] == null)
    //        {
    //            var delivery = paymentOrder.Deliveries.FirstOrDefault(dlv => dlv.Id == deliveryId);
    //            var orderRowIds = delivery.OrderRows
    //                .Where(row => row.AvailableActions.Contains(OrderRowActionType.CanCreditRow))
    //                .Select(row => (long)row.OrderRowId).ToList();
    //            var row = delivery.OrderRows.First();
    //            var feeAmount = new MinorUnit(200);
    //            var fee = new Fee(row.ArticleNumber, row.Name, feeAmount, 25);

    //            var creditingOptions = orderRowIds.Select(x => new RowCreditingOptions(x, delivery.OrderRows.First(y => y.OrderRowId == x).Quantity)).ToList();

    //            var response = await delivery.Actions.CreditOrderRowsWithFee(new CreditOrderRowWithFeeRequest(orderRowIds, fee, creditingOptions), new PollingTimeout(15)).ConfigureAwait(false);

    //            TempData["CreditMessage"] = $"Delivery order rows credited with fee ({feeAmount.InLowestMonetaryUnit}). -> {response.ResourceUri.AbsoluteUri}";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
    //    }

    //    return RedirectToAction("Details", "Orders");
    //}

    //#endregion Delivery
}