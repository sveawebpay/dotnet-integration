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
using Order = Sample.AspNetCore.Models.Order;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Order.Row;
using Sample.AspNetCore.Models;

namespace Sample.AspNetCore.Controllers;

public class OrdersController : Controller
{
    private readonly StoreDbContext context;
    private static readonly WebpayConfig Config = new WebpayConfig();
    private static List<OrderViewModel> orderViewModels = new List<OrderViewModel>();
    private static readonly Dictionary<string, List<long>> SveaOrderDeliveryReferences = new();

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
    public async Task<IActionResult> ExecuteAction(string OrderId, string Action, List<SelectableNumberedOrderRow> OrderRows, List<AdminWS.OrderRow> NewOrderRows)
    {
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
            // WebpayService
            case "CloseOrderEu":
                var closeOrderRequest = WebpayConnection.CloseOrder(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE);

                var closeOrderResponse = await closeOrderRequest.CloseOrderByOrderType(paymentType.ToString()).DoRequestAsync();

                if (closeOrderResponse.ResultCode != 0)
                    TempData["ErrorMessage"] = closeOrderResponse.ErrorMessage;

                break;

            case "DeliverOrderEu":
                var orderRowBuilders = newOrder.OrderRows
                    .Where(row => row.Status == "NotDelivered")
                    .Select(row => row.ToOrderRowBuilder(isCompany))
                    .ToList();

                var deliverOrderRequest = WebpayConnection.DeliverOrder(Config)
                    .AddOrderRows(orderRowBuilders)
                    .SetOrderId(long.Parse(OrderId))
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

                    if (!SveaOrderDeliveryReferences.ContainsKey(OrderId))
                        SveaOrderDeliveryReferences[OrderId] = new List<long>();

                    SveaOrderDeliveryReferences[OrderId].Add(deliveryReferenceNumber);
                }

                break;

            #region Deprecated
            case "GetContractPdfEu":
                // TODO: deprecated...
                if (!SveaOrderDeliveryReferences.TryGetValue(OrderId, out var contractNumbers) || !contractNumbers.Any())
                {
                    TempData["ErrorMessage"] = "No delivery references found for this order.";
                    return View("Details", new OrderListViewModel
                    {
                        PaymentOrders = orderViewModels
                    });
                }

                var contractNumber = contractNumbers.FirstOrDefault();

                var contractPdf = await WebpayConnection
                    .GetContractPdf(Config)
                    .SetCountryCode(CountryCode.SE)
                    .SetContractNumber(contractNumber)
                    .DoRequestAsync();

                if (contractPdf.ResultCode == 0)
                {
                    Console.WriteLine("Contract PDF fetched successfully!");
                    Console.WriteLine($"PDF Link: {contractPdf.PdfLink}");

                    if (!string.IsNullOrEmpty(contractPdf.FileBinaryDataBase64))
                    {
                        var pdfBytes = Convert.FromBase64String(contractPdf.FileBinaryDataBase64);
                        System.IO.File.WriteAllBytes("Contract.pdf", pdfBytes);
                        Console.WriteLine($"PDF saved to Contract.pdf (Size: {contractPdf.FileLengthInBytes} bytes)");
                    }
                    else
                    {
                        Console.WriteLine("No binary data available for the PDF.");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = contractPdf.ErrorMessage;
                }

                break;
            #endregion

            // AdminService
            case "AddOrderRows":
                if (NewOrderRows != null && NewOrderRows.Any())
                {
                    var newOrderRowBuilders = NewOrderRows
                        .Select(row => row.ToOrderRowBuilder(isCompany, true))
                        .ToList();

                    var addOrderRowsBuilder = WebpayAdmin.AddOrderRows(Config)
                        .SetOrderId(long.Parse(OrderId))
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
                break;

            case "UpdateOrderRows":
                if (OrderRows != null)
                {
                    var newOrderRowBuilders = OrderRows
                        .Where(row => row.Status == "NotDelivered")
                        .Select(row => row.ToNumberedOrderRowBuilder(isCompany))
                        .ToList();
                    var updateOrderRowsBuilder = WebpayAdmin.UpdateOrderRows(Config)
                        .SetOrderId(long.Parse(OrderId))
                        .SetCountryCode(CountryCode.SE)
                        .AddUpdateOrderRows(newOrderRowBuilders);

                    var addition = await updateOrderRowsBuilder.UpdateOrderRowsByPaymentType(paymentType).DoRequestAsync();

                    if (addition.ResultCode != 0)
                        TempData["ErrorMessage"] = addition.ErrorMessage;
                }
                break;

            case "CancelOrderRows":
                var rowIndexesToCancel = selectedRows.Select(row => row.RowNumber).ToList();

                var cancellation = WebpayAdmin.CancelOrderRows(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetRowsToCancel(rowIndexesToCancel)
                    .SetCountryCode(CountryCode.SE);

                var cancellationResponse = await cancellation.CancelPaymentTypeOrderRows(paymentType).DoRequestAsync();

                if (cancellationResponse.ResultCode != 0)
                    TempData["ErrorMessage"] = cancellationResponse.ErrorMessage;

                break;

            case "UpdateOrder":
                var clientOrderIdText = "Updated clientOrderId";
                var notesText = "Updated notes";

                var updateBuilder = new UpdateOrderBuilder(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE)
                    .SetClientOrderNumber(clientOrderIdText)
                    .SetNotes(notesText);

                var updateResponse = await updateBuilder.UpdatePaymentTypeOrder(paymentType).DoRequestAsync();

                if (updateResponse.ResultCode != 0)
                    TempData["ErrorMessage"] = updateResponse.ErrorMessage;

                break;

            case "ApproveInvoice":
                // TODO: if multiple deliveries, allow user to select one
                if (!SveaOrderDeliveryReferences.TryGetValue(OrderId, out var invoiceDeliveryReference) || !invoiceDeliveryReference.Any())
                {
                    TempData["ErrorMessage"] = "No delivery references found for this order.";
                    return View("Details", new OrderListViewModel
                    {
                        PaymentOrders = orderViewModels
                    });
                }

                var firstInvoiceDeliveryReference = invoiceDeliveryReference.FirstOrDefault(); // Use first delivery reference for now

                var approveInvoiceBuilder = WebpayAdmin.ApproveInvoice(Config)
                    .SetInvoiceId(firstInvoiceDeliveryReference)
                    .SetClientId(Config.GetClientNumber(PaymentType.INVOICE, CountryCode.SE))
                    .SetCountryCode(CountryCode.SE);

                var approveInvoiceResponse = await approveInvoiceBuilder.ApproveInvoice().DoRequestAsync();

                if (approveInvoiceResponse.ResultCode != 0)
                    TempData["ErrorMessage"] = approveInvoiceResponse.ErrorMessage;

                break;

            case "DeliverPartial":
                var rowIndexesToDeliver = selectedRows.Select(row => row.RowNumber).ToList();

                var deliverBuilder = WebpayAdmin.DeliverOrderRows(Config)
                    .SetCountryCode(CountryCode.SE)
                    .SetInvoiceDistributionType(DistributionType.POST)
                    .SetRowsToDeliver(rowIndexesToDeliver)
                    .SetOrderId(long.Parse(OrderId));

                var deliverResponse = await deliverBuilder.DeliverInvoiceOrderRows().DoRequestAsync();

                if (deliverResponse.ResultCode != 0)
                    TempData["ErrorMessage"] = deliverResponse.ErrorMessage;
                else
                {
                    var deliveryReferenceNumbers = deliverResponse.OrdersDelivered
                        .Select(o => o.DeliveryReferenceNumber)
                        .Select(refNum => refNum)
                        .ToList();

                    if (!SveaOrderDeliveryReferences.ContainsKey(OrderId))
                        SveaOrderDeliveryReferences[OrderId] = new List<long>();

                    SveaOrderDeliveryReferences[OrderId].AddRange(deliveryReferenceNumbers);
                }

                break;

            case "CancelOrder":
                var cancelOrderBuilder = WebpayAdmin.CancelOrder(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE);

                var cancelOrderResponse = await cancelOrderBuilder.CancelPaymentTypeOrder(paymentType).DoRequestAsync();

                if (cancelOrderResponse.ResultCode != 0)
                    TempData["ErrorMessage"] = cancelOrderResponse.ErrorMessage;

                break;

            case "CreditInvoiceRows":
                // TODO
                // Provide popup for selecing a row to credit, or create a new row?

                var newIncVatCreditOrderRow = new OrderRowBuilder()
                    .SetName("NewCreditOrderRow")
                    .SetAmountIncVat(-10.0M)
                    .SetVatPercent(25)
                    .SetQuantity(1M);

                var newCreditOrderRows = new List<OrderRowBuilder> { newIncVatCreditOrderRow };

                // TODO: if multiple deliveries, allow user to select one
                if (!SveaOrderDeliveryReferences.TryGetValue(OrderId, out var deliveryReference) || !deliveryReference.Any())
                {
                    TempData["ErrorMessage"] = "No delivery references found for this order.";
                    return View("Details", new OrderListViewModel
                    {
                        PaymentOrders = orderViewModels
                    });
                }

                var firstDeliveryReference = deliveryReference.FirstOrDefault(); // Use first delivery reference for now
                var rowIndexesToCredit = selectedRows.Select(row => row.RowNumber).ToList();

                // TODO: fix delivery IDs and AccountCredit
                if (paymentType == PaymentType.INVOICE)
                {
                    var creditBuilder = WebpayAdmin.CreditOrderRows(Config)
                        .SetInvoiceId(firstDeliveryReference)
                        .SetInvoiceDistributionType(DistributionType.POST)
                        .SetCountryCode(CountryCode.SE)
                        //.AddCreditOrderRows(newCreditOrderRows)
                        .SetRowsToCredit(rowIndexesToCredit)
                        .CreditInvoiceOrderRows();

                    var creditResponse = await creditBuilder.DoRequestAsync();

                    if (creditResponse.ResultCode != 0)
                        TempData["ErrorMessage"] = creditResponse.ErrorMessage;
                }
                else if (paymentType == PaymentType.PAYMENTPLAN)
                {
                    var creditBuilder = WebpayAdmin.CreditOrderRows(Config)
                        .SetContractNumber(firstDeliveryReference)
                        .SetCountryCode(CountryCode.SE)
                        //.AddCreditOrderRows(newCreditOrderRows)
                        .SetRowsToCredit(rowIndexesToCredit)
                        .CreditPaymentPlanOrderRows();

                    var creditResponse = await creditBuilder.DoRequestAsync();

                    if (creditResponse.ResultCode != 0)
                        TempData["ErrorMessage"] = creditResponse.ErrorMessage;
                }
                else if (paymentType == PaymentType.ACCOUNTCREDIT)
                {
                    var creditBuilder = WebpayAdmin.CreditOrderRows(Config)
                        .SetContractNumber(firstDeliveryReference)
                        .SetCountryCode(CountryCode.SE)
                        //.AddCreditOrderRows(newCreditOrderRows)
                        .SetRowsToCredit(rowIndexesToCredit)
                        .CreditAccountCreditOrderRows();

                    var creditResponse = await creditBuilder.DoRequestAsync();

                    if (creditResponse.ResultCode != 0)
                        TempData["ErrorMessage"] = creditResponse.ErrorMessage;
                }

                break;

            default:
                return BadRequest("Invalid action.");
        }

        if (string.IsNullOrWhiteSpace(TempData["ErrorMessage"] as string))
            TempData["DeliverMessage"] = "Action successfully executed!";

        return RedirectToAction("Details");
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteGeneralAction(string Action)
    {
        var OrderId = "123"; // TODO
        try
        {
            switch (Action)
            {
                case "DeliverOrders":
                    // TODO
                    // Present option for selecting multiple orders to deliver
                    //var orderIdsToDeliver = new List<long> { long.Parse(OrderId) };

                    //var builder = WebpayAdmin.DeliverOrders(Config)
                    //    .SetOrderIds(orderIdsToDeliver)
                    //    .SetCountryCode(CountryCode.SE);

                    //var delivery = await builder.DeliverPaymentTypeOrders(paymentType).DoRequestAsync();

                    //if (delivery.ResultCode != 0)
                    //    TempData["ErrorMessage"] = delivery.ErrorMessage;
                    //else
                    //{
                    //    var deliveryReferenceNumbers = delivery.OrdersDelivered
                    //        .Select(o => o.DeliveryReferenceNumber)
                    //        .Select(refNum => refNum)
                    //        .ToList();

                    //    if (!SveaOrderDeliveryReferences.ContainsKey(OrderId))
                    //        SveaOrderDeliveryReferences[OrderId] = new List<long>();

                    //    SveaOrderDeliveryReferences[OrderId].AddRange(deliveryReferenceNumbers);
                    //}

                    break;

                case "GetInvoices":
                if (!SveaOrderDeliveryReferences.TryGetValue(OrderId, out var clientInvoiceIds) || !clientInvoiceIds.Any())
                {
                    TempData["ErrorMessage"] = "No delivery references found for this order.";
                    return View("Details", new OrderListViewModel
                    {
                        PaymentOrders = orderViewModels
                    });
                }

                // Get ALL invoice delivery references (use this for general action button...)
                //var invoiceOrderIds = await context.Orders
                //    .Where(o => o.PaymentType == PaymentType.INVOICE)
                //    .Select(o => o.SveaOrderId)
                //    .ToListAsync();

                //var invoiceDeliveryReferences = SveaOrderDeliveryReferences
                //    .Where(kv => invoiceOrderIds.Contains(kv.Key) && kv.Value.Any())
                //    .ToDictionary(kv => kv.Key, kv => kv.Value);

                var getInvoicesBuilder = WebpayAdmin.GetInvoices(Config)
                    .SetCountryCode(CountryCode.SE)
                    .SetInvoiceType(PaymentType.INVOICE)
                    .SetInvoiceIds(clientInvoiceIds);

                var getInvoicesRequest = getInvoicesBuilder.Build();
                var getInvoicesResponse = await getInvoicesRequest.DoRequestAsync();

                if (getInvoicesResponse.ResultCode != 0)
                    TempData["ErrorMessage"] = getInvoicesResponse.ErrorMessage;

                break;

                //case "GetFinancialReport":
                //    await GetFinancialReport(OrderId);
                //    TempData["ReportMessage"] = "Financial report fetched successfully.";
                //    break;

                //case "GetInvoiceReport":
                //    await GetInvoiceReport(OrderId);
                //    TempData["ReportMessage"] = "Invoice report fetched successfully.";
                //    break;

                //case "GetAccountingReport":
                //    await GetAccountingReport(OrderId);
                //    TempData["ReportMessage"] = "Accounting report fetched successfully.";
                //    break;

                //case "GetRegressionReport":
                //    await GetRegressionReport(OrderId);
                //    TempData["ReportMessage"] = "Regression report fetched successfully.";
                //    break;

                //case "GetPaymentPlanReport":
                //    await GetPaymentPlanReport(OrderId);
                //    TempData["ReportMessage"] = "Payment plan report fetched successfully.";
                //    break;

                default:
                    TempData["ErrorMessage"] = "Invalid action selected.";
                    break;
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction("OrderDetails");
    }

    // GET: Orders
    public IActionResult Index()
    {
        return View();
    }

    private bool OrderExists(int id)
    {
        return context.Orders.Any(e => e.OrderId == id);
    }
}