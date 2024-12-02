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

namespace Sample.AspNetCore.Controllers;

public class OrdersController : Controller
{
    private readonly StoreDbContext context;

    private static readonly WebpayConfig Config = new WebpayConfig
    {
        MyUserName = "sverigetest",
        MyPassword = "sverigetest",
        MyClientNumber = 79021,
        MyMerchantId = string.Empty,
        MySecretWord = string.Empty
    };
   
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

                    var response = await queryOrderBuilder.QueryInvoiceOrder().DoRequestAsync();
                    var newOrder = response.Orders.FirstOrDefault();

                    orderViewModel.Order = newOrder;
                    orderViewModel.IsLoaded = true;
                    orderViewModel.ShippingStatus = order.ShippingStatus;
                    orderViewModel.ShippingDescription = order.ShippingDescription;
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
    public async Task<IActionResult> ExecuteAction(string OrderId, string Action)
    {
        if (string.IsNullOrWhiteSpace(OrderId) || string.IsNullOrWhiteSpace(Action))
        {
            return BadRequest("Invalid parameters.");
        }

        // Fetch order again to get latest data
        var queryOrderBuilder = WebpayAdmin.QueryOrder(Config)
            .SetOrderId(long.Parse(OrderId))
            .SetCountryCode(CountryCode.SE);

        var response = await queryOrderBuilder.QueryInvoiceOrder().DoRequestAsync();
        var newOrder = response.Orders.FirstOrDefault();

        // TODO: validate order for action

        // Handle action
        switch (Action)
        {
            // WebpayService
            case "CloseOrderEu":
                var closeOrderResponse = await WebpayConnection.CloseOrder(Config)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE)
                    .CloseInvoiceOrder()
                    .DoRequestAsync();

                if (closeOrderResponse.ResultCode == 0)
                {
                    Console.WriteLine("Order delivered successfully!");
                }

                break;

            case "DeliverOrderEu":
                var orderRowBuilders = newOrder.OrderRows
                    .Select(row => row.ToOrderRowBuilder())
                    .ToList();

                var deliverOrderResponse = await WebpayConnection.DeliverOrder(Config)
                    .AddOrderRows(orderRowBuilders)
                    .SetOrderId(long.Parse(OrderId))
                    .SetCountryCode(CountryCode.SE)
                    .SetInvoiceDistributionType(DistributionType.POST)
                    .DeliverInvoiceOrder()
                    .DoRequestAsync();

                if (deliverOrderResponse.ResultCode == 0)
                {
                    Console.WriteLine("Order delivered successfully!");
                }

                break;

            case "GetContractPdfEu":

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
                Config.MyClientNumber = 58702; // TODO
                var accountCreditParams = await WebpayConnection
                    .GetAccountCreditParams(Config)
                    .SetCountryCode(TestingTool.DefaultTestCountryCode)
                    .DoRequestAsync();

                if (accountCreditParams.ResultCode == 0)
                {
                    Console.WriteLine("accountCreditParams fetched successfully!");
                }
                Config.MyClientNumber = 79021; // TODO

                break;

            case "GetPaymentPlanParamsEu":
                Config.MyClientNumber = 59999; // TODO
                var paymentPlanParams= await WebpayConnection
                    .GetPaymentPlanParams(Config)
                    .SetCountryCode(TestingTool.DefaultTestCountryCode)
                    .DoRequestAsync();

                if (paymentPlanParams.ResultCode == 0)
                {
                    Console.WriteLine("paymentPlanParams fetched successfully!");
                }
                Config.MyClientNumber = 79021; // TODO

                break;

            // AdminService
            case "AddOrderRows":
                // TODO
                //var builder = WebpayAdmin.AddOrderRows(SveaConfig.GetDefaultConfig())
                //    .SetOrderId(order.CreateOrderResult.SveaOrderId)
                //    .SetCountryCode(CountryCode.SE)
                //    .AddOrderRow(firstOrderRow)
                //    .AddOrderRow(secondOrderRow);

                //var addition = await builder.AddInvoiceOrderRows().DoRequestAsync();
                break;

            case "UpdateOrderRows":
                // TODO
                break;

            case "CancelOrderRows":
                // TODO
                break;

            case "UpdateOrder":
                // TODO
                break;

            case "ApproveInvoice":
                // TODO
                break;

            case "DeliverPartial":
                // TODO
                break;

            case "DeliverOrders":
                // TODO
                break;

            case "CancelOrder":
                // TODO
                break;

            case "CreditInvoiceRows":
                // TODO
                break;

            case "GetInvoices":
                // TODO
                //var getInvoicesBuilder = WebpayAdmin.GetInvoices(Config)
                //    .SetInvoiceIds(referenceNumbers.ToList())
                //    .SetCountryCode(CountryCode.SE);

                //var getInvoicesRequest = getInvoicesBuilder.Build();
                //var getInvoicesResponse = await getInvoicesRequest.DoRequestAsync();

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