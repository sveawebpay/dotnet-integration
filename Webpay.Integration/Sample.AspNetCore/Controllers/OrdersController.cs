﻿using System;
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
using WebpayWS;
using AdminWS;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Order.Row;
using Sample.AspNetCore.Models;

namespace Sample.AspNetCore.Controllers;

public class OrdersController : Controller
{
    private readonly StoreDbContext context;

    private static readonly WebpayConfig Config = new WebpayConfig();
   
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
        
        var orderViewModels = new List<OrderViewModel>();
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
                        // TODO
                        //PaymentType.ACCOUNTCREDIT => await queryOrderBuilder.QueryAccountCreditOrder().DoRequestAsync(),
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
    //public async Task<IActionResult> ExecuteAction(string OrderId, string Action)
    public async Task<IActionResult> ExecuteAction(string OrderId, string Action, List<SelectableNumberedOrderRow> OrderRows, List<AdminWS.OrderRow> NewOrderRows)
    {
        if (string.IsNullOrWhiteSpace(OrderId) || string.IsNullOrWhiteSpace(Action))
        {
            return BadRequest("Invalid parameters.");
        }

        // Fetch order again to get latest data
        var queryOrderBuilder = WebpayAdmin.QueryOrder(Config)
            .SetOrderId(long.Parse(OrderId))
            .SetCountryCode(CountryCode.SE);

        var order = await context.Orders.FirstOrDefaultAsync(o => o.SveaOrderId == OrderId);

        if (order == null)
        {
            return NotFound("Order not found.");
        }
        var paymentType = order.PaymentType;

        var response = paymentType switch
        {
            PaymentType.INVOICE => await queryOrderBuilder.QueryInvoiceOrder().DoRequestAsync(),
            PaymentType.PAYMENTPLAN => await queryOrderBuilder.QueryPaymentPlanOrder().DoRequestAsync(),
            // TODO
            //PaymentType.ACCOUNTCREDIT => await queryOrderBuilder.QueryAccountCreditOrder().DoRequestAsync(),
            _ => throw new InvalidOperationException("Unsupported PaymentType")
        };
        var newOrder = response.Orders.FirstOrDefault();

        // TODO: validate order for action

        var selectedRows = OrderRows?.Where(row => row.IsSelected).ToList();

        // Handle action
        switch (Action)
        {
            // WebpayService
            case "CloseOrderEu":
                var closeOrderRequest = WebpayConnection.CloseOrder(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE);

                var closeOrderResponse = await (paymentType switch
                {
                    PaymentType.INVOICE => closeOrderRequest.CloseInvoiceOrder(),
                    PaymentType.PAYMENTPLAN => closeOrderRequest.ClosePaymentPlanOrder(),
                    // TODO
                    //PaymentType.ACCOUNTCREDIT => closeOrderRequest.CloseAccountCreditOrder(),
                    _ => throw new InvalidOperationException("Unsupported PaymentType for closing order.")
                }).DoRequestAsync();

                if (closeOrderResponse.ResultCode == 0)
                {
                    Console.WriteLine("Order delivered successfully!");
                }

                break;

            case "DeliverOrderEu":
                var orderRowBuilders = newOrder.OrderRows
                    .Select(row => row.ToOrderRowBuilder())
                    .ToList();

                //var deliverOrderResponse = await WebpayConnection.DeliverOrder(Config)
                //    .AddOrderRows(orderRowBuilders)
                //    .SetOrderId(long.Parse(OrderId))
                //    .SetCountryCode(CountryCode.SE)
                //    .SetInvoiceDistributionType(DistributionType.POST)
                //    .DeliverInvoiceOrder()
                //    .DoRequestAsync();

                var deliverOrderRequest = WebpayConnection.DeliverOrder(Config)
                    .AddOrderRows(orderRowBuilders)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE);

                var deliverOrderResponse = await (paymentType switch
                {
                    PaymentType.INVOICE => deliverOrderRequest.SetInvoiceDistributionType(DistributionType.POST).DeliverInvoiceOrder(),
                    PaymentType.PAYMENTPLAN => deliverOrderRequest.DeliverPaymentPlanOrder(),
                    // TODO
                    //PaymentType.ACCOUNTCREDIT => deliverOrderRequest.DeliverAccountCreditOrder(),
                    _ => throw new InvalidOperationException("Unsupported PaymentType for closing order.")
                }).DoRequestAsync();

                if (deliverOrderResponse.ResultCode == 0)
                {
                    Console.WriteLine("Order delivered successfully!");
                }

                break;

            case "GetContractPdfEu":
                // TODO: deprecated...

                var contractPdf = await WebpayConnection
                    .GetContractPdf(Config)
                    .SetCountryCode(CountryCode.SE)
                    .SetContractNumber(123)
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
                    Console.WriteLine($"Failed to fetch contract PDF. ResultCode: {contractPdf.ResultCode}, Error: {contractPdf.ErrorMessage}");
                }

                break;

            case "GetAccountCreditParamsEu":
                var accountCreditParams = await WebpayConnection
                    .GetAccountCreditParams(Config)
                    .SetCountryCode(TestingTool.DefaultTestCountryCode)
                    .DoRequestAsync();

                if (accountCreditParams.ResultCode == 0)
                {
                    Console.WriteLine("accountCreditParams fetched successfully!");
                }

                break;

            case "GetPaymentPlanParamsEu":
                var paymentPlanParams= await WebpayConnection
                    .GetPaymentPlanParams(Config)
                    .SetCountryCode(TestingTool.DefaultTestCountryCode)
                    .DoRequestAsync();

                if (paymentPlanParams.ResultCode == 0)
                {
                    Console.WriteLine("paymentPlanParams fetched successfully!");
                }

                break;

            // AdminService
            case "AddOrderRows":
                if (NewOrderRows != null && NewOrderRows.Any())
                {
                    var newOrderRowBuilders = NewOrderRows
                        .Select(row => row.ToOrderRowBuilder())
                        .ToList();

                    var addOrderRowsBuilder = WebpayAdmin.AddOrderRows(Config)
                        .SetOrderId(long.Parse(OrderId))
                        .SetCountryCode(CountryCode.SE)
                        .AddOrderRows(newOrderRowBuilders);

                    var addition = await (paymentType switch
                    {
                        PaymentType.INVOICE => addOrderRowsBuilder.AddInvoiceOrderRows(),
                        PaymentType.PAYMENTPLAN => addOrderRowsBuilder.AddPaymentPlanOrderRows(),
                        // TODO
                        //PaymentType.ACCOUNTCREDIT => addOrderRowsBuilder.AddAccountCreditOrderRows(),
                        _ => throw new InvalidOperationException("Unsupported PaymentType for closing order.")
                    }).DoRequestAsync();
                }
                break;

            case "UpdateOrderRows":
                if (OrderRows != null)
                {
                    var newOrderRowBuilders = OrderRows
                        .Select(row => row.ToNumberedOrderRowBuilder())
                        .ToList();
                    var updateOrderRowsBuilder = WebpayAdmin.UpdateOrderRows(Config)
                        .SetOrderId(long.Parse(OrderId))
                        .SetCountryCode(CountryCode.SE)
                        .AddUpdateOrderRows(newOrderRowBuilders);

                    var addition = await (paymentType switch
                    {
                        PaymentType.INVOICE => updateOrderRowsBuilder.UpdateInvoiceOrderRows(),
                        PaymentType.PAYMENTPLAN => updateOrderRowsBuilder.UpdatePaymentPlanOrderRows(),
                        // TODO
                        //PaymentType.ACCOUNTCREDIT => updateOrderRowsBuilder.UpdateAccountCreditOrderRows(),
                        _ => throw new InvalidOperationException("Unsupported PaymentType for closing order.")
                    }).DoRequestAsync();
                }
                break;

            case "CancelOrderRows":
                var rowIndexesToCancel = selectedRows.Select(row => row.RowNumber).ToList();

                var cancellation = WebpayAdmin.CancelOrderRows(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetRowsToCancel(rowIndexesToCancel)
                    .SetCountryCode(CountryCode.SE);

                var cancellationResponse = await (paymentType switch
                {
                    PaymentType.INVOICE => cancellation.CancelInvoiceOrderRows(),
                    PaymentType.PAYMENTPLAN => cancellation.CancelPaymentPlanOrderRows(),
                    // TODO
                    //PaymentType.ACCOUNTCREDIT => cancellation.CancelAccountCreditOrderRows(),
                    _ => throw new InvalidOperationException("Unsupported PaymentType for closing order.")
                }).DoRequestAsync();

                break;

            case "UpdateOrder":
                var clientOrderNumberText = "Updated clientOrderNumber";
                var notesText = "Updated notes";

                var updateBuilder = new UpdateOrderBuilder(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE)
                    .SetClientOrderNumber(clientOrderNumberText)
                    .SetNotes(notesText);

                var updateResponse = await (paymentType switch
                {
                    PaymentType.INVOICE => updateBuilder.UpdateInvoiceOrder(),
                    PaymentType.PAYMENTPLAN => updateBuilder.UpdatePaymentPlanOrder(),
                    // TODO
                    //PaymentType.ACCOUNTCREDIT => updateBuilder.UpdateAccountCreditOrder(),
                    _ => throw new InvalidOperationException("Unsupported PaymentType for closing order.")
                }).DoRequestAsync();

                break;

            case "ApproveInvoice":
                // TODO (test)
                // Order needs to be delivered...
                var approveInvoiceBuilder = WebpayAdmin.ApproveInvoice(Config)
                    .SetInvoiceId(123456789) // TODO
                    .SetClientId(Config.GetClientNumber(PaymentType.INVOICE, CountryCode.SE))
                    .SetCountryCode(CountryCode.SE);

                var approveInvoiceResponse= await approveInvoiceBuilder.ApproveInvoice().DoRequestAsync();

                if (response.ResultCode == 0)
                {
                    Console.WriteLine("Invoice approved successfully!");
                }
                else
                {
                    Console.WriteLine($"Failed to approve invoice: {response.ErrorMessage}");
                }

                break;

            case "DeliverPartial":
                var rowIndexesToDeliver = selectedRows.Select(row => row.RowNumber).ToList();

                var deliverBuilder = WebpayAdmin.DeliverOrderRows(Config)
                    .SetCountryCode(CountryCode.SE)
                    .SetInvoiceDistributionType(DistributionType.POST)
                    .SetRowsToDeliver(rowIndexesToDeliver)
                    .SetOrderId(long.Parse(OrderId));

                var deliverResponse = await deliverBuilder.DeliverInvoiceOrderRows().DoRequestAsync();

                break;

            case "DeliverOrders":
                // TODO
                // Present option for selecting multiple orders to deliver

                var orderIdsToDeliver = new List<long> { long.Parse(OrderId) };

                var builder = WebpayAdmin.DeliverOrders(Config)
                    .SetOrderIds(orderIdsToDeliver)
                    .SetCountryCode(CountryCode.SE);

                var delivery = await (paymentType switch
                {
                    PaymentType.INVOICE => builder.SetInvoiceDistributionType(DistributionType.POST).DeliverInvoiceOrders(),
                    PaymentType.PAYMENTPLAN => builder.DeliverPaymentPlanOrders(),
                    // TODO
                    //PaymentType.ACCOUNTCREDIT => builder.DeliverAccountCreditOrders(),
                    _ => throw new InvalidOperationException("Unsupported PaymentType for closing order.")
                }).DoRequestAsync();

                break;

            case "CancelOrder":
                var cancelOrderBuilder = WebpayAdmin.CancelOrder(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE);

                var cancelInvoiceResponse = await (paymentType switch
                {
                    PaymentType.INVOICE => cancelOrderBuilder.CancelInvoiceOrder(),
                    PaymentType.PAYMENTPLAN => cancelOrderBuilder.CancelPaymentPlanOrder(),
                    // TODO
                    //PaymentType.ACCOUNTCREDIT => builder.DeliverAccountCreditOrders(),
                    _ => throw new InvalidOperationException("Unsupported PaymentType for closing order.")
                }).DoRequestAsync();

                break;

            case "CreditInvoiceRows":
                // TODO
                // Provide popup for selecing a row to credit, or create a new row?

                // Order needs to be delivered...
                var newIncVatCreditOrderRow = new OrderRowBuilder()
                    .SetName("NewCreditOrderRow")
                    .SetAmountIncVat(10.0M)
                    .SetVatPercent(25)
                    .SetQuantity(1M);

                var newCreditOrderRows = new List<OrderRowBuilder> { newIncVatCreditOrderRow };

                var creditResponse = await (paymentType switch
                {
                    // TODO: fix delivery IDs
                    PaymentType.INVOICE => WebpayAdmin.CreditOrderRows(Config)
                        //.SetInvoiceId(deliverResponse.OrdersDelivered.FirstOrDefault()?.DeliveryReferenceNumber ?? throw new InvalidOperationException("DeliveryReferenceNumber is required for Invoice."))
                        .SetInvoiceId(1226007L)
                        .SetInvoiceDistributionType(DistributionType.POST)
                        .SetCountryCode(CountryCode.SE)
                        .AddCreditOrderRows(newCreditOrderRows)
                        .CreditInvoiceOrderRows(),

                    PaymentType.PAYMENTPLAN => WebpayAdmin.CreditOrderRows(Config)
                        //.SetContractNumber(deliverResponse.DeliverOrderResult.PaymentPlanResultDetails?.ContractNumber ?? throw new InvalidOperationException("ContractNumber is required for PaymentPlan."))
                        .SetContractNumber(91067320L)
                        .SetCountryCode(CountryCode.SE)
                        .AddCreditOrderRows(newCreditOrderRows)
                        .CreditPaymentPlanOrderRows(),

                    PaymentType.ACCOUNTCREDIT => throw new NotImplementedException("Credit for AccountCredit is not yet implemented."),

                    _ => throw new InvalidOperationException("Unsupported PaymentType for crediting order rows.")
                }).DoRequestAsync();

                break;

            case "GetInvoices":
                // TODO
                // Present option for retrieving delivered invoices
                var invoiceIdsToFetch = new List<long> { 1226007L }; // ClientInvoiceId

                var getInvoicesBuilder = WebpayAdmin.GetInvoices(Config)
                    .SetInvoiceIds(invoiceIdsToFetch)
                    .SetCountryCode(CountryCode.SE);

                var getInvoicesRequest = getInvoicesBuilder.Build();
                var getInvoicesResponse = await getInvoicesRequest.DoRequestAsync();

                break;

            default:
                return BadRequest("Invalid action.");
        }

        return RedirectToAction("Details");
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