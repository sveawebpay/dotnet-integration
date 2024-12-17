using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.AspNetCore.Data;
using Sample.AspNetCore.Models.ViewModels;
using Sample.AspNetCore.Webpay;
using Sample.AspNetCore.Extensions;
using Webpay.Integration;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using Webpay.Integration.Order.Handle;
using Sample.AspNetCore.Models;
using System.Text.Json;
using Order = Sample.AspNetCore.Models.Order;
using System.Net.Http;

namespace Sample.AspNetCore.Controllers;

public class OrdersController : Controller
{
    private readonly StoreDbContext context;
    private static readonly WebpayConfig Config = new WebpayConfig();
    private static List<OrderViewModel> orderViewModels = new List<OrderViewModel>();
    private static readonly Dictionary<string, List<long>> SveaOrderDeliveryReferences = new();
    private static readonly Dictionary<string, List<long>> CreditedDeliveryReferences = new();
    private static readonly Dictionary<string, List<long>> DeliveryReferenceOrderRows = new();
    private static string pdfString;

    public OrdersController(StoreDbContext context)
    {
        this.context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        var orders = await context.Orders.ToListAsync();
        if (orders != null)
        {
            context.Orders.RemoveRange(orders);
            await context.SaveChangesAsync();
        }

        orderViewModels.Clear();
        SveaOrderDeliveryReferences.Clear();
        CreditedDeliveryReferences.Clear();
        DeliveryReferenceOrderRows.Clear();

        return RedirectToAction(nameof(Index));
    }

    // GET: Orders/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Orders/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,PaymentOrderId")] Order order)
    {
        if (ModelState.IsValid)
        {
            context.Add(order);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(order);
    }

    //GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        Thread.Sleep(TimeSpan.FromSeconds(1));

        var orders = new List<Order>();
        if (id.HasValue)
        {
            var order = await context.Orders.FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            orders.Add(order);
        }
        else
        {
            orders = await context.Orders.ToListAsync();
        }
        
        orderViewModels = new List<OrderViewModel>();
        foreach (var order in orders)
        {
            var orderId = int.Parse(order.SveaOrderId);
            var orderViewModel = new OrderViewModel(orderId);
            if (!string.IsNullOrWhiteSpace(order.SveaOrderId))
            {
                try
                {
                    var queryOrderBuilder = WebpayAdmin.QueryOrder(Config)
                        .SetOrderId(long.Parse(order.SveaOrderId))
                        .SetCountryCode(CountryCode.SE);

                    var response = order.PaymentType switch
                    {
                        PaymentType.INVOICE => await queryOrderBuilder.QueryInvoiceOrder().DoRequestAsync(),
                        PaymentType.PAYMENTPLAN => await queryOrderBuilder.QueryPaymentPlanOrder().DoRequestAsync(),
                        PaymentType.ACCOUNTCREDIT => await queryOrderBuilder.QueryAccountCreditOrder().DoRequestAsync(),
                        _ => throw new InvalidOperationException("Unsupported PaymentType")
                    };

                    var newOrder = response.Orders.FirstOrDefault();

                    orderViewModel.Order = newOrder;
                    orderViewModel.IsLoaded = true;
                    orderViewModel.PaymentType = order.PaymentType;
                }
                catch {}

                orderViewModels.Add(orderViewModel);
            }
        }

        var orderDeliveryReferences = SveaOrderDeliveryReferences
            .Where(pair => orderViewModels.Any(order => order.Order.SveaOrderId.ToString() == pair.Key))
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        var invoiceDeliveryReferences = SveaOrderDeliveryReferences
            .Where(kvp => orderViewModels.Any(order => order.Order.SveaOrderId == long.Parse(kvp.Key) && order.Order.OrderType == "Invoice"))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        TempData["SveaOrderDeliveryReferences"] = JsonSerializer.Serialize(orderDeliveryReferences);
        TempData["SveaInvoiceDeliveryReferences"] = JsonSerializer.Serialize(invoiceDeliveryReferences);
        TempData["CreditedDeliveryReferences"] = JsonSerializer.Serialize(CreditedDeliveryReferences);
        TempData["DeliveryReferenceOrderRows"] = JsonSerializer.Serialize(DeliveryReferenceOrderRows);
        TempData.Keep("SveaOrderDeliveryReferences");
        TempData.Keep("SveaInvoiceDeliveryReferences");
        TempData.Keep("CreditedDeliveryReferences");
        TempData.Keep("DeliveryReferenceOrderRows");

        return View(new OrderListViewModel
        {
            PaymentOrders = orderViewModels
        });
    }

    // GET: Orders/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var order = await context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();
        return View(order);
    }

    // POST: Orders/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,PaymentOrderId")] Order order)
    {
        if (id != order.OrderId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(order);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteAction(string OrderId, string Action, List<SelectableNumberedOrderRow> OrderRows, List<AdminWS.OrderRow> NewOrderRows, long? SveaDeliveryReference, long? SveaInvoiceDeliveryReference)
    {
        ClearTempData();

        if (string.IsNullOrWhiteSpace(OrderId) || string.IsNullOrWhiteSpace(Action))
        {
            TempData["ErrorMessage"] = "Invalid parameters.";
            return View("Details", new OrderListViewModel
            {
                PaymentOrders = orderViewModels
            });
        }

        var order = await context.Orders.FirstOrDefaultAsync(o => o.SveaOrderId == OrderId);
        var paymentType = order.PaymentType;

        // Fetch order again to get latest data
        var queryOrderBuilder = WebpayAdmin.QueryOrder(Config)
            .SetOrderId(long.Parse(OrderId))
            .SetCountryCode(CountryCode.SE);

        var response = await queryOrderBuilder.QueryPaymentTypeOrder(paymentType).DoRequestAsync();
        var newOrder = response.Orders.FirstOrDefault();

        if (OrderRows != null && newOrder?.OrderRows != null)
        {
            foreach (var row in OrderRows)
            {
                var matchingRow = newOrder.OrderRows.FirstOrDefault(newRow => newRow.RowNumber == row.RowNumber);
                if (matchingRow != null)
                {
                    row.Status = matchingRow.Status;
                }
            }
        }

        var selectedRows = OrderRows?.Where(row => row.IsSelected).ToList();
        var isCompany = newOrder.Customer.CustomerType == AdminWS.CustomerType.Company;

        switch (Action)
        {
            case "CloseOrderEu":
                await HandleCloseOrderEu(OrderId, paymentType);
                break;

            case "DeliverOrderEu":
                await HandleDeliverOrderEu(OrderId, paymentType, newOrder, isCompany);
                break;

            case "AddOrderRows":
                await HandleAddOrderRows(OrderId, paymentType, NewOrderRows, isCompany);
                break;

            case "UpdateOrderRows":
                await HandleUpdateOrderRows(OrderId, paymentType, OrderRows, isCompany);
                break;

            case "CancelOrderRows":
                await HandleCancelOrderRows(OrderId, paymentType, selectedRows);
                break;

            case "UpdateOrder":
                await HandleUpdateOrder(OrderId, paymentType);
                break;

            case "DeliverPartial":
                await HandleDeliverPartial(OrderId, paymentType, selectedRows);
                break;

            case "CancelOrder":
                await HandleCancelOrder(OrderId, paymentType);
                break;

            case "CreditRows":
                if (!SveaDeliveryReference.HasValue)
                {
                    TempData["ErrorMessage"] = "No delivery reference selected.";
                    return View("Details", new OrderListViewModel
                    {
                        PaymentOrders = orderViewModels
                    });
                }
                else
                {
                    await HandleCreditRows(OrderId, paymentType, NewOrderRows, selectedRows, SveaDeliveryReference.Value, isCompany);
                }
                break;

            case "GetInvoicePdfLink":
                if (!SveaInvoiceDeliveryReference.HasValue)
                {
                    TempData["ErrorMessage"] = "No Invoice ID selected.";
                    return View("Details", new OrderListViewModel
                    {
                        PaymentOrders = orderViewModels
                    });
                }
                else
                {
                    await HandleGetInvoicePdfLink(SveaInvoiceDeliveryReference.Value);
                }

                break;

            #region Deprecated
            case "GetContractPdfEu":
                if (!SveaOrderDeliveryReferences.TryGetValue(OrderId, out var contractNumbers) || !contractNumbers.Any())
                {
                    TempData["ErrorMessage"] = "No delivery references found for this order.";
                    return View("Details", new OrderListViewModel
                    {
                        PaymentOrders = orderViewModels
                    });
                }
                else
                {
                    await HandleGetContractPdfEu(OrderId, contractNumbers);
                }
                break;

            case "ApproveInvoice":
                if (!SveaOrderDeliveryReferences.TryGetValue(OrderId, out var invoiceDeliveryReferences) || !invoiceDeliveryReferences.Any())
                {
                    TempData["ErrorMessage"] = "No delivery references found for this order.";
                    return View("Details", new OrderListViewModel
                    {
                        PaymentOrders = orderViewModels
                    });
                }
                else
                {
                    await HandleApproveInvoice(OrderId, invoiceDeliveryReferences);
                }
                break;
            #endregion

            default:
                return BadRequest("Invalid action.");
        }

        if (string.IsNullOrWhiteSpace(TempData["ErrorMessage"] as string) && string.IsNullOrWhiteSpace(TempData["SuccessMessage"] as string))
            TempData["SuccessMessage"] = "Action successfully executed!";

        return RedirectToAction("Details");
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteGeneralAction(string Action, string SelectedPaymentType, List<string> SelectedIds)
    {
        ClearTempData();

        if (string.IsNullOrWhiteSpace(Action))
        {
            TempData["ErrorMessage"] = "Action must be selected.";
            return RedirectToAction("Details");
        }

        try
        {
            switch (Action)
            {
                case "DeliverOrders":
                    if (SelectedIds == null || !SelectedIds.Any())
                    {
                        TempData["ErrorMessage"] = "No orders selected for delivery.";
                        return RedirectToAction("Details");
                    }
                    else
                    {
                        await HandleDeliverOrders(SelectedPaymentType, SelectedIds);
                        return RedirectToAction("Details");
                    }

                case "GetInvoices":
                    if (SelectedIds == null || !SelectedIds.Any())
                    {
                        TempData["ErrorMessage"] = "No invoices selected for GetInvoices.";
                        return RedirectToAction("Details");
                    }
                    else
                    {
                        await HandleGetInvoices(SelectedIds);
                    }
                    break;

                case "GetFinancialReport":
                    await HandleGetFinancialReport();
                    break;

                case "GetInvoiceReport":
                    await HandleGetInvoiceReport();
                    break;

                case "GetPaymentPlanReport":
                    await HandleGetPaymentPlanReport();
                    break;

                case "GetAccountingReport":
                    await HandleGetAccountingReport();
                    break;

                case "GetRegressionReport":
                    await HandleGetRegressionReport();
                    break;

                default:
                    TempData["ErrorMessage"] = "Invalid action selected.";
                    break;
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }

        return View("Details", new OrderListViewModel
        {
            PaymentOrders = orderViewModels
        });
    }

    // GET: Orders
    public IActionResult Index()
    {
        return View();
    }

    private async Task HandleCloseOrderEu(string orderId, PaymentType paymentType)
    {
        var closeOrderRequest = WebpayConnection.CloseOrder(Config)
            .SetOrderId(long.Parse(orderId))
            .SetCountryCode(CountryCode.SE);

        var closeOrderResponse = await closeOrderRequest.CloseOrderByOrderType(paymentType.ToString()).DoRequestAsync();

        if (closeOrderResponse.ResultCode != 0)
            TempData["ErrorMessage"] = closeOrderResponse.ErrorMessage;
    }

    private async Task HandleDeliverOrderEu(string orderId, PaymentType paymentType, AdminWS.Order newOrder, bool isCompany)
    {
        var orderRowBuilders = newOrder.OrderRows
            .Where(row => row.Status == "NotDelivered")
            .Select(row => row.ToOrderRowBuilder(isCompany))
            .ToList();

        var deliverOrderRequest = WebpayConnection.DeliverOrder(Config)
            .AddOrderRows(orderRowBuilders)
            .SetOrderId(long.Parse(orderId))
            .SetCountryCode(CountryCode.SE);

        if (paymentType == PaymentType.INVOICE)
        {
            deliverOrderRequest.SetInvoiceDistributionType(DistributionType.POST);
        }
        var deliverOrderResponse = await deliverOrderRequest.DeliverOrderByPaymentType(paymentType).DoRequestAsync();

        if (deliverOrderResponse.ResultCode != 0)
            TempData["ErrorMessage"] = deliverOrderResponse.ErrorMessage;
        else
        {
            var deliveryReferenceNumber = paymentType switch
            {
                PaymentType.INVOICE => deliverOrderResponse.DeliverOrderResult.InvoiceResultDetails.InvoiceId,
                PaymentType.PAYMENTPLAN => deliverOrderResponse.DeliverOrderResult.PaymentPlanResultDetails.ContractNumber,
                PaymentType.ACCOUNTCREDIT => deliverOrderResponse.DeliverOrderResult.AccountCreditResultDetails.AccountCreditId,
                _ => throw new InvalidOperationException("Unsupported PaymentType for delivering order.")
            };

            if (!SveaOrderDeliveryReferences.ContainsKey(orderId))
                SveaOrderDeliveryReferences[orderId] = new List<long>();

            SveaOrderDeliveryReferences[orderId].Add(deliveryReferenceNumber);

            // Save delivered rows
            if (!DeliveryReferenceOrderRows.ContainsKey(deliveryReferenceNumber.ToString()))
            {
                DeliveryReferenceOrderRows[deliveryReferenceNumber.ToString()] = new List<long>();
            }

            var notDeliveredRowNumbers = newOrder.OrderRows
                .Where(row => row.Status == "NotDelivered")
                .Select(row => row.RowNumber)
                .ToList();
            DeliveryReferenceOrderRows[deliveryReferenceNumber.ToString()].AddRange(notDeliveredRowNumbers);
        }
    }

    private async Task HandleAddOrderRows(string orderId, PaymentType paymentType, List<AdminWS.OrderRow> newOrderRows, bool isCompany)
    {
        if (newOrderRows != null && newOrderRows.Any())
        {
            var newOrderRowBuilders = newOrderRows
                .Select(row => row.ToOrderRowBuilder(isCompany, true))
                .ToList();

            var addOrderRowsBuilder = WebpayAdmin.AddOrderRows(Config)
                .SetOrderId(long.Parse(orderId))
                .SetCountryCode(CountryCode.SE)
                .AddOrderRows(newOrderRowBuilders);

            var addition = await addOrderRowsBuilder.AddOrderRowsByPaymentType(paymentType).DoRequestAsync();

            if (addition.ResultCode != 0)
                TempData["ErrorMessage"] = addition.ErrorMessage;
        }
        else
        {
            TempData["ErrorMessage"] = "No new order rows found.";
        }
    }

    private async Task HandleUpdateOrderRows(string orderId, PaymentType paymentType, List<SelectableNumberedOrderRow> orderRows, bool isCompany)
    {
        if (orderRows != null)
        {
            var newOrderRowBuilders = orderRows
                .Where(row => row.Status == "NotDelivered")
                .Select(row => row.ToNumberedOrderRowBuilder(isCompany))
                .ToList();
            var updateOrderRowsBuilder = WebpayAdmin.UpdateOrderRows(Config)
                .SetOrderId(long.Parse(orderId))
                .SetCountryCode(CountryCode.SE)
                .AddUpdateOrderRows(newOrderRowBuilders);

            var addition = await updateOrderRowsBuilder.UpdateOrderRowsByPaymentType(paymentType).DoRequestAsync();

            if (addition.ResultCode != 0)
                TempData["ErrorMessage"] = addition.ErrorMessage;
        }
    }

    private async Task HandleCancelOrderRows(string orderId, PaymentType paymentType, List<SelectableNumberedOrderRow> selectedRows)
    {
        var rowIndexesToCancel = selectedRows.Select(row => row.RowNumber).ToList();

        var cancellation = WebpayAdmin.CancelOrderRows(Config)
            .SetOrderId(long.Parse(orderId))
            .SetRowsToCancel(rowIndexesToCancel)
            .SetCountryCode(CountryCode.SE);

        var cancellationResponse = await cancellation.CancelPaymentTypeOrderRows(paymentType).DoRequestAsync();

        if (cancellationResponse.ResultCode != 0)
            TempData["ErrorMessage"] = cancellationResponse.ErrorMessage;
    }

    private async Task HandleUpdateOrder(string orderId, PaymentType paymentType)
    {
        var clientOrderIdText = "Updated clientOrderId";
        var notesText = "Updated notes";

        var updateBuilder = new UpdateOrderBuilder(Config)
            .SetOrderId(long.Parse(orderId))
            .SetCountryCode(CountryCode.SE)
            .SetClientOrderNumber(clientOrderIdText)
            .SetNotes(notesText);

        var updateResponse = await updateBuilder.UpdatePaymentTypeOrder(paymentType).DoRequestAsync();

        if (updateResponse.ResultCode != 0)
            TempData["ErrorMessage"] = updateResponse.ErrorMessage;
    }

    private async Task HandleDeliverPartial(string orderId, PaymentType paymentType, List<SelectableNumberedOrderRow> selectedRows)
    {
        var rowIndexesToDeliver = selectedRows.Select(row => row.RowNumber).ToList();

        var deliverBuilder = WebpayAdmin.DeliverOrderRows(Config)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetRowsToDeliver(rowIndexesToDeliver)
            .SetOrderId(long.Parse(orderId));

        var deliverResponse = await deliverBuilder.DeliverInvoiceOrderRows().DoRequestAsync();

        if (deliverResponse.ResultCode != 0)
            TempData["ErrorMessage"] = deliverResponse.ErrorMessage;
        else
        {
            var deliveryReferenceNumbers = deliverResponse.OrdersDelivered
                .Select(o => o.DeliveryReferenceNumber)
                .Select(refNum => refNum)
                .ToList();

            if (!SveaOrderDeliveryReferences.ContainsKey(orderId))
                SveaOrderDeliveryReferences[orderId] = new List<long>();

            SveaOrderDeliveryReferences[orderId].AddRange(deliveryReferenceNumbers);

            // Save delivered rows
            var referenceNumber = deliveryReferenceNumbers.FirstOrDefault();
            if (!DeliveryReferenceOrderRows.ContainsKey(referenceNumber.ToString()))
            {
                DeliveryReferenceOrderRows[referenceNumber.ToString()] = new List<long>();
            }
            DeliveryReferenceOrderRows[referenceNumber.ToString()].AddRange(rowIndexesToDeliver);
        }
    }

    private async Task HandleCancelOrder(string orderId, PaymentType paymentType)
    {
        var cancelOrderBuilder = WebpayAdmin.CancelOrder(Config)
            .SetOrderId(long.Parse(orderId))
            .SetCountryCode(CountryCode.SE);

        var cancelOrderResponse = await cancelOrderBuilder.CancelPaymentTypeOrder(paymentType).DoRequestAsync();

        if (cancelOrderResponse.ResultCode != 0)
            TempData["ErrorMessage"] = cancelOrderResponse.ErrorMessage;
    }

    private async Task HandleCreditRows(string orderId, PaymentType paymentType, List<AdminWS.OrderRow> newOrderRows, List<SelectableNumberedOrderRow> selectedRows, long deliveryReference, bool isCompany)
    {
        var newCreditOrderRows = newOrderRows
            .Select(row => row.ToOrderRowBuilder(isCompany, true))
            .ToList();

        var rowIndexesToCredit = selectedRows.Select(row => row.RowNumber).ToList();

        if (paymentType == PaymentType.INVOICE)
        {
            var creditBuilder = WebpayAdmin.CreditOrderRows(Config)
                .SetInvoiceId(deliveryReference)
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetCountryCode(CountryCode.SE)
                .AddCreditOrderRows(newCreditOrderRows)
                .SetRowsToCredit(rowIndexesToCredit)
                .CreditInvoiceOrderRows();

            var creditResponse = await creditBuilder.DoRequestAsync();

            if (creditResponse.ResultCode != 0)
                TempData["ErrorMessage"] = creditResponse.ErrorMessage;
            else
            {
                var creditReferenceNumber = creditResponse.OrdersDelivered.FirstOrDefault().DeliveryReferenceNumber;
                SveaOrderDeliveryReferences[orderId].Add(creditReferenceNumber);

                if (!CreditedDeliveryReferences.ContainsKey(orderId))
                {
                    CreditedDeliveryReferences[orderId] = new List<long>();
                }
                CreditedDeliveryReferences[orderId].Add(creditReferenceNumber);
            }
        }
        else if (paymentType == PaymentType.PAYMENTPLAN)
        {
            var creditBuilder = WebpayAdmin.CreditOrderRows(Config)
                .SetContractNumber(deliveryReference)
                .SetCountryCode(CountryCode.SE)
                .AddCreditOrderRows(newCreditOrderRows)
                .SetRowsToCredit(rowIndexesToCredit)
                .CreditPaymentPlanOrderRows();

            var creditResponse = await creditBuilder.DoRequestAsync();

            if (creditResponse.ResultCode != 0)
                TempData["ErrorMessage"] = creditResponse.ErrorMessage;
        }
        else if (paymentType == PaymentType.ACCOUNTCREDIT)
        {
            var creditBuilder = WebpayAdmin.CreditOrderRows(Config)
                .SetContractNumber(deliveryReference)
                .SetCountryCode(CountryCode.SE)
                .AddCreditOrderRows(newCreditOrderRows)
                .SetRowsToCredit(rowIndexesToCredit)
                .CreditAccountCreditOrderRows();

            var creditResponse = await creditBuilder.DoRequestAsync();

            if (creditResponse.ResultCode != 0)
                TempData["ErrorMessage"] = creditResponse.ErrorMessage;
        }

        if (string.IsNullOrWhiteSpace(TempData["ErrorMessage"] as string))
        {
            if (!CreditedDeliveryReferences.ContainsKey(orderId))
            {
                CreditedDeliveryReferences[orderId] = new List<long>();
            }
            CreditedDeliveryReferences[orderId].Add(deliveryReference);

            if (DeliveryReferenceOrderRows.ContainsKey(deliveryReference.ToString()))
            {
                var existingRows = DeliveryReferenceOrderRows[deliveryReference.ToString()];

                DeliveryReferenceOrderRows[deliveryReference.ToString()] =
                    existingRows.Except(rowIndexesToCredit).ToList();
            }
        }
    }

    private async Task HandleGetInvoicePdfLink(long invoiceDeliveryReference)
    {
        var getInvoicePdfLinkBuilder = WebpayAdmin.GetInvoicePdfLink(Config)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceId(invoiceDeliveryReference);

        var getInvoicePdfLinkRequest = getInvoicePdfLinkBuilder.Build();
        var pdfLinkResponse = await getInvoicePdfLinkRequest.DoRequestAsync();

        if (pdfLinkResponse.ResultCode != 0)
        {
            TempData["ErrorMessage"] = pdfLinkResponse.ErrorMessage;
        }
        else
        {
            var pdfLink = pdfLinkResponse.PdfLink;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(pdfLink);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var pdfData = JsonSerializer.Deserialize<PdfResponse>(jsonResponse);
                    //TempData["PdfBase64"] = pdfData.Pdf;
                    TempData["PdfBase64"] = "true";
                    pdfString = pdfData.Pdf;
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve PDF data.";
                }
            }

            TempData["SuccessMessage"] = $"Invoice PDF link retrieved successfully (displayed at bottom of the page).";
        }
    }

    #region Deprecated
    private async Task HandleGetContractPdfEu(string orderId, List<long> contractNumbers)
    {
        var contractNumber = contractNumbers.FirstOrDefault();

        var contractPdf = await WebpayConnection
            .GetContractPdf(Config)
            .SetCountryCode(CountryCode.SE)
            .SetContractNumber(contractNumber)
            .DoRequestAsync();

        if (contractPdf.ResultCode == 0)
        {
            TempData["SuccessMessage"] = $"Contract PDF link retrieved successfully: {contractPdf.PdfLink}";

            if (!string.IsNullOrEmpty(contractPdf.FileBinaryDataBase64))
            {
                var pdfBytes = Convert.FromBase64String(contractPdf.FileBinaryDataBase64);
                System.IO.File.WriteAllBytes("Contract.pdf", pdfBytes);
            }
            else
            {
                TempData["ErrorMessage"] = "No binary data available for the PDF.";
            }
        }
        else
        {
            TempData["ErrorMessage"] = contractPdf.ErrorMessage;
        }
    }

    private async Task HandleApproveInvoice(string orderId, List<long> invoiceDeliveryReferences)
    {
        // TODO: if multiple deliveries, allow user to select one
        var firstInvoiceDeliveryReference = invoiceDeliveryReferences.FirstOrDefault(); // Use first delivery reference for now

        var approveInvoiceBuilder = WebpayAdmin.ApproveInvoice(Config)
            .SetInvoiceId(firstInvoiceDeliveryReference)
            .SetClientId(Config.GetClientNumber(PaymentType.INVOICE, CountryCode.SE))
            .SetCountryCode(CountryCode.SE);

        var approveInvoiceResponse = await approveInvoiceBuilder.ApproveInvoice().DoRequestAsync();

        if (approveInvoiceResponse.ResultCode != 0)
            TempData["ErrorMessage"] = approveInvoiceResponse.ErrorMessage;
    }
    #endregion

    private async Task HandleDeliverOrders(string selectedPaymentType, List<string> selectedIds)
    {
        var paymentType = selectedPaymentType.ToLowerInvariant() switch
        {
            "invoice" => PaymentType.INVOICE,
            "paymentplan" => PaymentType.PAYMENTPLAN,
            "accountcredit" => PaymentType.ACCOUNTCREDIT,
            _ => throw new ArgumentException("Invalid payment type.")
        };

        var queriedOrders = new Dictionary<long, AdminWS.GetOrdersResponse>();
        foreach (var orderId in selectedIds.Select(long.Parse))
        {
            var queryOrderBuilder = WebpayAdmin.QueryOrder(Config)
                .SetOrderId(orderId)
                .SetCountryCode(CountryCode.SE);

            var response = await queryOrderBuilder.QueryPaymentTypeOrder(paymentType).DoRequestAsync();
            if (response?.Orders?.FirstOrDefault() is { } newOrder)
            {
                queriedOrders[orderId] = response;
            }
            else
            {
                var errorMessage = $"Failed to fetch order details for Order ID {orderId}.";
                TempData["ErrorMessage"] = errorMessage;
                throw new InvalidOperationException(errorMessage);
            }
        }

        var orderIdsToDeliver = selectedIds.Select(id => long.Parse(id)).ToList();

        var builder = WebpayAdmin.DeliverOrders(Config)
            .SetOrderIds(orderIdsToDeliver)
            .SetCountryCode(CountryCode.SE);

        if (paymentType == PaymentType.INVOICE)
        {
            builder.SetInvoiceDistributionType(DistributionType.POST);
        }

        var delivery = await builder.DeliverPaymentTypeOrders(paymentType).DoRequestAsync();

        if (delivery.ResultCode != 0)
            TempData["ErrorMessage"] = delivery.ErrorMessage;
        else
        {
            foreach (var deliveredOrder in delivery.OrdersDelivered)
            {
                var sveaOrderId = deliveredOrder.SveaOrderId;
                var deliveryReferenceNumber = deliveredOrder.DeliveryReferenceNumber;

                if (!SveaOrderDeliveryReferences.ContainsKey(sveaOrderId.ToString()))
                {
                    SveaOrderDeliveryReferences[sveaOrderId.ToString()] = new List<long>();
                }

                if (!SveaOrderDeliveryReferences[sveaOrderId.ToString()].Contains(deliveryReferenceNumber))
                {
                    SveaOrderDeliveryReferences[sveaOrderId.ToString()].Add(deliveryReferenceNumber);
                }

                // Save delivered rows
                if (!DeliveryReferenceOrderRows.ContainsKey(deliveryReferenceNumber.ToString()))
                {
                    DeliveryReferenceOrderRows[deliveryReferenceNumber.ToString()] = new List<long>();
                }

                if (queriedOrders.TryGetValue(sveaOrderId, out var queriedOrderResponse))
                {
                    var queriedOrder = queriedOrderResponse.Orders.FirstOrDefault();
                    var notDeliveredRowNumbers = queriedOrder?.OrderRows
                        .Where(row => row.Status == "NotDelivered")
                        .Select(row => row.RowNumber)
                        .ToList() ?? new List<long>();

                    DeliveryReferenceOrderRows[deliveryReferenceNumber.ToString()].AddRange(notDeliveredRowNumbers);
                }
            }

            TempData["SuccessMessage"] = $"Successfully delivered {delivery.OrdersDelivered.Count()} orders.";
        }
    }

    private async Task HandleGetInvoices(List<string> selectedIds)
    {
        var clientInvoiceIds = selectedIds.SelectMany(refs => refs.Split(',')).Select(long.Parse).ToList();

        var getInvoicesBuilder = WebpayAdmin.GetInvoices(Config)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceType(PaymentType.INVOICE)
            .SetInvoiceIds(clientInvoiceIds);

        var getInvoicesRequest = getInvoicesBuilder.Build();
        var getInvoicesResponse = await getInvoicesRequest.DoRequestAsync();

        if (getInvoicesResponse.ResultCode != 0)
            TempData["ErrorMessage"] = getInvoicesResponse.ErrorMessage;
        else
        {
            TempData["Invoices"] = JsonSerializer.Serialize(getInvoicesResponse.Invoices);
            TempData["SuccessMessage"] = "Invoices retrieved successfully (displayed at bottom of the page).";
        }
    }

    private async Task HandleGetFinancialReport()
    {
        var financialReportBuilder = WebpayAdmin.GetFinancialReport(Config)
            .SetCountryCode(CountryCode.SE)
            //.SetFromDate(new DateTime(2024, 1, 1))
            //.SetToDate(new DateTime(2024, 12, 31));
            .SetFromDate(DateTime.Now.AddDays(-120).Date)
            .SetToDate(DateTime.Now.Date);

        var financialReportRequest = financialReportBuilder.Build();
        var financialReportResponse = await financialReportRequest.DoRequestAsync();

        if (financialReportResponse.ResultCode != 0)
        {
            TempData["ErrorMessage"] = financialReportResponse.ErrorMessage;
        }
        else
        {
            TempData["FinancialReport"] = JsonSerializer.Serialize(financialReportResponse);
            TempData["SuccessMessage"] = "Financial report retrieved successfully (displayed at bottom of the page).";
        }
    }

    private async Task HandleGetInvoiceReport()
    {
        var invoiceReportBuilder = WebpayAdmin.GetInvoiceReport(Config)
            .SetCountryCode(CountryCode.SE)
            .SetFromDate(DateTime.Now.AddDays(-60).Date)
            .SetToDate(DateTime.Now.Date);

        var invoiceReportRequest = invoiceReportBuilder.Build();
        var invoiceReportResponse = await invoiceReportRequest.DoRequestAsync();

        if (invoiceReportResponse.ResultCode != 0)
        {
            TempData["ErrorMessage"] = invoiceReportResponse.ErrorMessage;
        }
        else
        {
            TempData["InvoiceReport"] = JsonSerializer.Serialize(invoiceReportResponse);
            TempData["SuccessMessage"] = "Invoice report retrieved successfully (displayed at bottom of the page).";
        }
    }

    private async Task HandleGetPaymentPlanReport()
    {
        var paymentPlanReportBuilder = WebpayAdmin.GetPaymentPlanReport(Config)
            .SetCountryCode(CountryCode.SE)
            .SetFromDate(DateTime.Now.AddDays(-60).Date)
            .SetToDate(DateTime.Now.Date);

        var paymentPlanReportRequest = paymentPlanReportBuilder.Build();
        var paymentPlanReportResponse = await paymentPlanReportRequest.DoRequestAsync();

        if (paymentPlanReportResponse.ResultCode != 0)
        {
            TempData["ErrorMessage"] = paymentPlanReportResponse.ErrorMessage;
        }
        else
        {
            TempData["PaymentPlanReport"] = JsonSerializer.Serialize(paymentPlanReportResponse);
            TempData["SuccessMessage"] = "PaymentPlan report retrieved successfully (displayed at bottom of the page).";
        }
    }

    private async Task HandleGetAccountingReport()
    {
        var accountingReportBuilder = WebpayAdmin.GetAccountingReport(Config)
            .SetCountryCode(CountryCode.SE)
            .SetFromDate(DateTime.Now.AddDays(-60).Date)
            .SetToDate(DateTime.Now.Date);

        var accountingReportRequest = accountingReportBuilder.Build();
        var accountingReportResponse = await accountingReportRequest.DoRequestAsync();

        if (accountingReportResponse.ResultCode != 0)
        {
            TempData["ErrorMessage"] = accountingReportResponse.ErrorMessage;
        }
        else
        {
            TempData["AccountingReport"] = JsonSerializer.Serialize(accountingReportResponse);
            TempData["SuccessMessage"] = "Accounting report retrieved successfully (displayed at bottom of the page).";
        }
    }

    private async Task HandleGetRegressionReport()
    {
        var regressionReportBuilder = WebpayAdmin.GetRegressionReport(Config)
            .SetCountryCode(CountryCode.SE)
            .SetFromDate(DateTime.Now.AddDays(-60).Date)
            .SetToDate(DateTime.Now.Date);

        var regressionReportRequest = regressionReportBuilder.Build();
        var regressionReportResponse = await regressionReportRequest.DoRequestAsync();

        if (regressionReportResponse.ResultCode != 0)
        {
            TempData["ErrorMessage"] = regressionReportResponse.ErrorMessage;
        }
        else
        {
            TempData["RegressionReport"] = JsonSerializer.Serialize(regressionReportResponse);
            TempData["SuccessMessage"] = "Regression report retrieved successfully (displayed at bottom of the page).";
        }
    }

    [HttpGet]
    public IActionResult GetPdf()
    {
        if (pdfString != null)
        {
            var pdfBytes = Convert.FromBase64String(pdfString);
            return File(pdfBytes, "application/pdf", "Invoice.pdf");
        }

        TempData["ErrorMessage"] = "No PDF data available.";
        return RedirectToAction("Details");
    }

    private void ClearTempData()
    {
        TempData["ErrorMessage"] = null;
        TempData["SuccessMessage"] = null;
        TempData["Invoices"] = null;
        TempData["FinancialReport"] = null;
        TempData["InvoiceReport"] = null;
        TempData["PaymentPlanReport"] = null;
        TempData["AccountingReport"] = null;
        TempData["RegressionReport"] = null;
        TempData["PdfBase64"] = null;
        pdfString = null;

        // Keep these
        TempData.Keep("SveaOrderDeliveryReferences");
        TempData.Keep("SveaInvoiceDeliveryReferences");
        TempData.Keep("CreditedDeliveryReferences");
        TempData.Keep("DeliveryReferenceOrderRows");
    }

    private bool OrderExists(int id)
    {
        return context.Orders.Any(e => e.OrderId == id);
    }
}

