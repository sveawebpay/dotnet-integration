using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sample.AspNetCore.Data;
using Sample.AspNetCore.Extensions;
using Sample.AspNetCore.Models;
using Sample.AspNetCore.Webpay;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webpay.Integration;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Testing;
using WebpayWS;
using Cart = Sample.AspNetCore.Models.Cart;

namespace Sample.AspNetCore.Controllers;

public class CheckOutController : Controller
{
    private readonly Cart _cartService;
    private readonly Market _marketService;
    private readonly Models.MerchantSettings _merchantSettings;
    private readonly StoreDbContext _context;

    private static readonly WebpayConfig Config = new WebpayConfig
    {
        MyUserName = "sverigetest",
        MyPassword = "sverigetest",
        MyClientNumber = 79021,
        MyMerchantId = string.Empty,
        MySecretWord = string.Empty
    };

    public CheckOutController(
        IOptionsSnapshot<Models.MerchantSettings> merchantsAccessor,
        Cart cartService,
        Market marketService,
        StoreDbContext context
    )
    {
        _merchantSettings = merchantsAccessor.Value;
        _cartService = cartService;
        _marketService = marketService;
        _context = context;
    }

    public async Task<IActionResult> LoadPaymentMenu(bool requireBankId, bool isInternational, bool enableShipping = false)
    {
        return View("Checkout");
    }

    [HttpPost]
    public async Task<IActionResult> SubmitForm(string SSN, bool IsCompany)
    {
        if (string.IsNullOrWhiteSpace(SSN))
        {
            ViewBag.Error = "SSN is required.";
            return View("Checkout");
        }

        try
        {
            // TODO: ordertype?
            var request = WebpayConnection.GetAddresses(Config)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetOrderTypeInvoice();

            if (IsCompany)
            {
                request.SetCompany(SSN);
            }
            else
            {
                request.SetIndividual(SSN);
            }

            var response = await request.DoRequestAsync();

            if (response.RejectionCode != GetCustomerAddressesRejectionCode.Accepted)
            {
                ViewBag.Error = "Failed to fetch address. Please verify the SSN.";
                return View("Checkout");
            }

            ViewBag.Addresses = response.Addresses;
            ViewBag.ShowAdditionalFields = true;
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"An error occurred: {ex.Message}";
        }

        return View("Checkout");
    }

    [HttpPost]
    public async Task<IActionResult> FinalizeForm(string PhoneNumber, string EmailAddress, string PaymentOption)
    {
        var orderItems = _cartService.CartLines.ToOrderItems().ToList();

        if (string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(EmailAddress))
        {
            ViewBag.Error = "Phone Number and Email Address are required.";
            ViewBag.ShowAdditionalFields = true;
            return View("Checkout");
        }

        if (PaymentOption == null)
        {
            ViewBag.Error = "Payment Option is required.";
            ViewBag.ShowAdditionalFields = true;
            return View("Checkout");
        }

        var createOrderBuilder = WebpayConnection.CreateOrder(Config)
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        // TODO: AccountCredit?
        CreateOrderEuResponse order;
        // TODO: PaymentPlan requires another ClientID
        if (PaymentOption == "PaymentPlan")
        {
            var paymentPlanParam = await WebpayConnection.GetPaymentPlanParams(Config)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DoRequestAsync();
            var code = paymentPlanParam.CampaignCodes[0].CampaignCode;

            //var request = createOrderBuilder.UsePaymentPlanPayment(code).PrepareRequest();
            order = await createOrderBuilder.UsePaymentPlanPayment(code).DoRequestAsync();
        }
        else
        {
            //var request = createOrderBuilder.UseInvoicePayment().PrepareRequest();
            order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();
        }

        PrintCreateOrderEuResponse(order);

        if (order.Accepted)
        {
            _cartService.SveaOrderId = order.CreateOrderResult.SveaOrderId.ToString();
            _cartService.Update();

            var products = _cartService.CartLines.Select(p => p.Product);

            _context.Products.AttachRange(products);

            _context.Orders.Add(new Order
            {
                SveaOrderId = _cartService.SveaOrderId,
                Lines = _cartService.CartLines.ToList(),
                ShippingStatus = _cartService.ShippingStatus
            });

            await _context.SaveChangesAsync(true);
            return RedirectToAction("Thankyou");
        } else
        {
            ViewBag.Error = "Something went wrong. Please try again.";
            ViewBag.ShowAdditionalFields = true;
            return View("Checkout");
        }
    }

    public ViewResult Thankyou()
    {
        _cartService.Clear();
        return View();
    }

    private void PrintCreateOrderEuResponse(CreateOrderEuResponse response)
    {
        Console.WriteLine("CreateOrderEuResponse:");
        Console.WriteLine($"  ResultCode: {response.ResultCode}");

        if (response.CreateOrderResult != null)
        {
            Console.WriteLine("  CreateOrderResult:");
            Console.WriteLine($"    SveaOrderId: {response.CreateOrderResult.SveaOrderId}");
            Console.WriteLine($"    OrderType: {response.CreateOrderResult.OrderType}");
            Console.WriteLine($"    SveaWillBuyOrder: {response.CreateOrderResult.SveaWillBuyOrder}");
            Console.WriteLine($"    Amount: {response.CreateOrderResult.Amount}");

            if (response.CreateOrderResult.CustomerIdentity != null)
            {
                Console.WriteLine($"    CustomerIdentity: {response.CreateOrderResult.CustomerIdentity}");
                // Etc...
            }
            else
            {
                Console.WriteLine("    CustomerIdentity: null");
            }

            if (response.CreateOrderResult.ExpirationDate.HasValue)
            {
                Console.WriteLine($"    ExpirationDate: {response.CreateOrderResult.ExpirationDate.Value}");
            }
            else
            {
                Console.WriteLine("    ExpirationDate: null");
            }

            Console.WriteLine($"    ClientOrderNumber: {response.CreateOrderResult.ClientOrderNumber}");

            if (response.CreateOrderResult.PendingReasons != null && response.CreateOrderResult.PendingReasons.Length > 0)
            {
                Console.WriteLine("    PendingReasons:");
                foreach (var reason in response.CreateOrderResult.PendingReasons)
                {
                    Console.WriteLine($"      - {reason}");
                }
            }
            else
            {
                Console.WriteLine("    PendingReasons: none");
            }
        }
        else
        {
            Console.WriteLine("  CreateOrderResult: null");
        }

        if (response.NavigationResult != null)
        {
            Console.WriteLine("  NavigationResult:");
            Console.WriteLine($"    RedirectUrl: {response.NavigationResult.RedirectUrl}");
        }
        else
        {
            Console.WriteLine("  NavigationResult: null");
        }
    }
}
