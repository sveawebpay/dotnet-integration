using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sample.AspNetCore.Data;
using Sample.AspNetCore.Extensions;
using Sample.AspNetCore.Models;
using Sample.AspNetCore.Webpay;
using System;
using System.Linq;
using System.Text.Json;
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

            TempData["AddressData"] = JsonSerializer.Serialize(response.Addresses);

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
        //var orderItems = _cartService.CartLines.ToOrderItems().ToList();
        var orderItems = _cartService.CartLines.Select(line => line.ToOrderRowBuilder()).ToList();

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

        var addressDataJson = TempData["AddressData"] as string;
        if (string.IsNullOrEmpty(addressDataJson))
        {
            ViewBag.Error = "Address data is missing. Please re-enter your SSN.";
            ViewBag.ShowAdditionalFields = true;
            return View("Checkout");
        }

        var addressData = JsonSerializer.Deserialize<CustomerAddress[]>(addressDataJson);
        var correlationId = Guid.NewGuid();
        var clientOrderNumber = GenerateRandomOrderNumber();
        var customerAddressData = addressData[0]; // Use first address...
        var ipAddress = GetIpAddress();
        var individualCustomer = TestingTool.ConfigureCustomer(customerAddressData, ipAddress, PhoneNumber, EmailAddress);

        var createOrderBuilder = WebpayConnection.CreateOrder(Config)
            .AddOrderRows(orderItems)
            .AddCustomerDetails(individualCustomer)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(DateTime.Now)
            .SetClientOrderNumber(clientOrderNumber)
            .SetCorrelationId(correlationId)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        // TODO: AccountCredit?
        CreateOrderEuResponse order;
        // TODO: PaymentPlan requires another ClientID
        if (PaymentOption == "PaymentPlan")
        {
            Config.MyClientNumber = 59999; // TODO
            var paymentPlanParam = await WebpayConnection.GetPaymentPlanParams(Config)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DoRequestAsync();
            var code = paymentPlanParam.CampaignCodes[0].CampaignCode;

            order = await createOrderBuilder.UsePaymentPlanPayment(code).DoRequestAsync();
            Config.MyClientNumber = 79021; // TODO
        }
        else
        {
            order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();
        }

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
        }
        else
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

    // Helpers
    private string GetIpAddress()
    {
        var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
        return remoteIpAddress != null ? remoteIpAddress.ToString() : "0.0.0.0";
    }

    private string GenerateRandomOrderNumber(int randomLength = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var randomString = new string(Enumerable.Repeat(chars, randomLength)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        return $"WebpayIntegration-{randomString}";
    }
}
