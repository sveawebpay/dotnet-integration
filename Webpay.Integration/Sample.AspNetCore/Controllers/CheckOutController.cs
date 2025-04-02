using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Data;
using Sample.AspNetCore.Extensions;
using Sample.AspNetCore.Models;
using Sample.AspNetCore.Webpay;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Webpay.Integration;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using WebpayWS;
using Cart = Sample.AspNetCore.Models.Cart;

namespace Sample.AspNetCore.Controllers;

public class CheckOutController : Controller
{
    private readonly Cart _cartService;
    private readonly Market _marketService;
    private readonly StoreDbContext _context;
    private static readonly WebpayConfig Config = new WebpayConfig();
    private static int SelectedAddressIndex { get; set; }
    private static bool IsTestCustomersVisible { get; set; }

    public CheckOutController(
        Cart cartService,
        Market marketService,
        StoreDbContext context)
    {
        _cartService = cartService;
        _marketService = marketService;
        _context = context;
    }

    public async Task<IActionResult> LoadPaymentMenu(bool requireBankId, bool isInternational)
    {
        return View("Checkout");
    }

    [HttpPost]
    public async Task<IActionResult> SubmitForm(string SSN, bool IsCompany)
    {
        SaveTempData();
        if (string.IsNullOrWhiteSpace(SSN))
        {
            ViewBag.Error = "SSN is required.";
            return View("Checkout");
        }

        try
        {
            TempData["AddressData"] = null;
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
            TempData["IsCompany"] = IsCompany;
            TempData.Keep("IsCompany");

            ViewBag.Addresses = response.Addresses;
            ViewBag.ShowAdditionalFields = true;
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"An error occurred: {ex.Message}";
        }

        SelectedAddressIndex = 0;

        return View("Checkout");
    }

    [HttpPost]
    public async Task<IActionResult> FinalizeForm(string PhoneNumber, string EmailAddress, string PaymentOption, long? CampaignCode)
    {
        SaveTempData();
        var isCompany = TempData["IsCompany"] != null && Convert.ToBoolean(TempData["IsCompany"]);
        var orderItems = _cartService.CartLines.Select(line => line.ToOrderRowBuilder(isCompany)).ToList();

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
        else if (PaymentOption != "Invoice" && !CampaignCode.HasValue)
        {
            ViewBag.Error = "Campaign Code is required for this Payment Option.";
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
        if (SelectedAddressIndex < 0 || SelectedAddressIndex >= addressData.Length)
        {
            ViewBag.Error = "Invalid address selection.";
            ViewBag.ShowAdditionalFields = true;
            return View("Checkout");
        }

        var customerAddressData = addressData[SelectedAddressIndex];
        var correlationId = Guid.NewGuid();
        var clientOrderNumber = GenerateRandomOrderNumber();
        var ipAddress = GetIpAddress();

        var createOrderBuilder = WebpayConnection.CreateOrder(Config)
            .AddOrderRows(orderItems)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(DateTime.Now)
            .SetClientOrderNumber(clientOrderNumber)
            .SetCorrelationId(correlationId)
            .AddNavigationUrls("https://localhost:44345/CheckOut/Thankyou", "https://svea.com/reject") // TODO: only do this if using bankid. Also maybe set localhost uri dynamically?
            .SetCurrency(TestingTool.DefaultTestCurrency);

        if (isCompany)
        {
            var companyCustomer = TestingTool.ConfigureCompanyCustomer(customerAddressData, ipAddress, PhoneNumber, EmailAddress);
            createOrderBuilder.AddCustomerDetails(companyCustomer);
        }
        else
        {
            var individualCustomer = TestingTool.ConfigureCustomer(customerAddressData, ipAddress, PhoneNumber, EmailAddress);
            createOrderBuilder.AddCustomerDetails(individualCustomer);
        }

        CreateOrderEuResponse order;
        if (PaymentOption == "PaymentPlan")
        {
            var paymentPlanParam = await WebpayConnection.GetPaymentPlanParams(Config)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DoRequestAsync();

            var selectedCampaign = paymentPlanParam.CampaignCodes
                .FirstOrDefault(c => c.CampaignCode == CampaignCode.Value);

            if (selectedCampaign == null)
            {
                ViewBag.Error = "Invalid campaign selected.";
                ViewBag.ShowAdditionalFields = true;
                return View("Checkout");
            }

            order = await createOrderBuilder.UsePaymentPlanPayment(selectedCampaign.CampaignCode).DoRequestAsync();
        }
        else if (PaymentOption == "AccountCredit")
        {
            var accountCreditParam = await WebpayConnection.GetAccountCreditParams(Config)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DoRequestAsync();

            var selectedCampaign = accountCreditParam.AccountCreditCampaignCodes
                .FirstOrDefault(c => c.CampaignCode == CampaignCode.Value);

            if (selectedCampaign == null)
            {
                ViewBag.Error = "Invalid campaign selected.";
                ViewBag.ShowAdditionalFields = true;
                return View("Checkout");
            }

            order = await createOrderBuilder.UseAccountCreditPayment(selectedCampaign.CampaignCode).DoRequestAsync();
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

            PaymentType paymentType = PaymentOption.ToLowerInvariant() switch
            {
                "invoice" => PaymentType.INVOICE,
                "paymentplan" => PaymentType.PAYMENTPLAN,
                "accountcredit" => PaymentType.ACCOUNTCREDIT,
                _ => throw new ArgumentException("Invalid PaymentOption value.")
            };

            _context.Orders.Add(new Order
            {
                SveaOrderId = _cartService.SveaOrderId,
                Lines = _cartService.CartLines.ToList(),
                PaymentType = paymentType
            });

            await _context.SaveChangesAsync(true);

            if (!string.IsNullOrEmpty(order.NavigationResult.RedirectUrl))
            {
                return Redirect(order.NavigationResult.RedirectUrl);
            }
            return RedirectToAction("Thankyou");
        }
        else
        {
            ViewBag.Error = "Something went wrong. Please try again.";
            ViewBag.ShowAdditionalFields = true;
            return View("Checkout");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCampaigns(string paymentOption)
    {
        var totalAmount = _cartService.CartLines.Sum(line => line.CalculateTotal());

        try
        {
            switch (paymentOption)
            {
                case "PaymentPlan":
                    var paymentPlanParams = await WebpayConnection
                        .GetPaymentPlanParams(Config)
                        .SetCountryCode(TestingTool.DefaultTestCountryCode)
                        .DoRequestAsync();

                    if (paymentPlanParams.ResultCode != 0)
                        return BadRequest(paymentPlanParams.ErrorMessage);

                    var filteredPaymentPlanCampaigns = paymentPlanParams.CampaignCodes
                        .Where(c => totalAmount >= c.FromAmount && totalAmount <= c.ToAmount)
                        .ToList();

                    if (!filteredPaymentPlanCampaigns.Any())
                    {
                        ViewBag.Error = "No campaigns found! Please try another amount or payment method.";
                        return View("Checkout");
                    }

                    return Json(filteredPaymentPlanCampaigns);

                case "AccountCredit":
                    var accountCreditParams = await WebpayConnection
                        .GetAccountCreditParams(Config)
                        .SetCountryCode(TestingTool.DefaultTestCountryCode)
                        .DoRequestAsync();

                    if (accountCreditParams.ResultCode != 0)
                        return BadRequest(accountCreditParams.ErrorMessage);

                    var filteredAccountCreditCampaigns = accountCreditParams.AccountCreditCampaignCodes
                        .Where(c => totalAmount >= c.LowestOrderAmount)
                        .ToList();

                    if (!filteredAccountCreditCampaigns.Any())
                    {
                        ViewBag.Error = "No campaigns found! Please try another amount or payment method.";
                        return View("Checkout");
                    }

                    return Json(filteredAccountCreditCampaigns);

                default:
                    return BadRequest("Unsupported payment method.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public IActionResult UpdateTestCustomersVisibility(bool IsTestCustomersVisibleInput)
    {
        IsTestCustomersVisible = IsTestCustomersVisibleInput;
        TempData["IsTestCustomersVisible"] = IsTestCustomersVisibleInput.ToString();
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> SaveSelectedAddress(int selectedAddressIndex)
    {
        SelectedAddressIndex = selectedAddressIndex;
        return NoContent();
    }

    public ViewResult Thankyou()
    {
        _cartService.Clear();
        return View();
    }

    // Helpers
    private void SaveTempData()
    {
        TempData["IsTestCustomersVisible"] = IsTestCustomersVisible;
        TempData.Keep("IsTestCustomersVisible");
    }

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
