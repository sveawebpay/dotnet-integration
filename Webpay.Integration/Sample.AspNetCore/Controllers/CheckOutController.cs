using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Data;
using Sample.AspNetCore.Extensions;
using Sample.AspNetCore.Models;
using Sample.AspNetCore.Webpay;
using System;
using System.Collections.Generic;
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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Cart _cartService;
    private readonly Market _marketService;
    private readonly StoreDbContext _context;
    private static readonly WebpayConfig Config = new WebpayConfig();

    private static int SelectedAddressIndex { get; set; }
    private static bool IsTestCustomersVisible { get; set; }
    private static bool UseBankID { get; set; } = false;
    private static bool UseKalp { get; set; } = false;

    public CheckOutController(
        IHttpContextAccessor httpContextAccessor,
        Cart cartService,
        Market marketService,
        StoreDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _cartService = cartService;
        _marketService = marketService;
        _context = context;
    }

    public async Task<IActionResult> LoadPaymentMenu(bool requireBankId, bool isInternational)
    {
        UseBankID = false;
        UseKalp = false;
        IsTestCustomersVisible = true;
        TempData["UseBankID"] = UseBankID;
        TempData["UseKalp"] = UseKalp;
        TempData["CountryId"] = _marketService.CountryId;
        TempData["IsTestCustomersVisible"] = IsTestCustomersVisible;

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

        if (!_cartService.CartLines.Any())
        {
            ViewBag.Error = "Cart is empty.";
            return View("Checkout");
        }

        try
        {
            TempData["AddressData"] = null;

            var countryCode = _marketService.CountryId.GetCountryCode();

            if (!IsCompany && (countryCode is CountryCode.FI || countryCode is CountryCode.NO))
            {
                var isFI = countryCode is CountryCode.FI;
                var mockedAddress = new CustomerAddress
                {
                    LegalName = isFI ? "Matti Meikäläinen" : "Ola Normann",
                    SecurityNumber = SSN,
                    PhoneNumber = "112233",
                    AddressLine1 = isFI ? "Testitie 1" : "Testveien 2",
                    AddressLine2 = "1A",
                    Postcode = isFI ? 370 : 0359,
                    Zipcode = isFI ? "370" : "0359",
                    Postarea = isFI ? "Helsinki" : "Oslo",
                    BusinessType = BusinessTypeCode.Person,
                    FirstName = isFI ? "Matti" : "Ola",
                    LastName = isFI ? "Meikäläinen" : "Normann"
                };

                var mockedAddresses = new List<CustomerAddress> { mockedAddress }.ToArray();

                TempData["AddressData"] = JsonSerializer.Serialize(mockedAddresses);
                TempData["IsCompany"] = IsCompany;
                TempData.Keep("IsCompany");
                TempData.Keep("AddressData");

                ViewBag.Addresses = mockedAddresses;
                ViewBag.ShowAdditionalFields = true;
            }
            else
            {
                var request = WebpayConnection.GetAddresses(Config)
                    .SetCountryCode(countryCode)
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
            .SetCountryCode(_marketService.CountryId.GetCountryCode())
            .SetOrderDate(DateTime.Now)
            .SetClientOrderNumber(clientOrderNumber)
            .SetCorrelationId(correlationId)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        if (UseBankID)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var thankYouUrl = $"{baseUrl}/CheckOut/Thankyou";
            var rejectUrl = $"{baseUrl}/CheckOut/LoadPaymentMenu";

            createOrderBuilder.AddNavigationUrls(thankYouUrl, rejectUrl);
        }

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
                .SetCountryCode(_marketService.CountryId.GetCountryCode())
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
                .SetCountryCode(_marketService.CountryId.GetCountryCode())
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

            if (UseBankID && !string.IsNullOrEmpty(order?.NavigationResult.RedirectUrl))
            {
                return Redirect(order.NavigationResult.RedirectUrl);
            }

            // temp
            if (UseKalp)
                return Redirect("https://www.svea.com/sv-se/logga-in");

            return RedirectToAction("Thankyou");
        }
        else
        {
            if (order.ErrorMessage != null)
                ViewBag.Error = order.ErrorMessage;
            else
                ViewBag.Error = "Something went wrong. Please try again.";

            ViewBag.ShowAdditionalFields = false;
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
                        .SetCountryCode(_marketService.CountryId.GetCountryCode())
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
                        .SetCountryCode(_marketService.CountryId.GetCountryCode())
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

    [HttpPost]
    public IActionResult UpdateUseBankID(bool useBankIDInput)
    {
        UseBankID = useBankIDInput;
        TempData["UseBankID"] = useBankIDInput.ToString();
        return NoContent();
    }

    [HttpPost]
    public IActionResult UpdateUseKalp(bool useKalpInput)
    {
        UseKalp = useKalpInput;
        TempData["UseKalp"] = useKalpInput.ToString();
        return NoContent();
    }

    public ViewResult Thankyou()
    {
        _cartService.Clear();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoiceCreditAgreementPdf(string paymentOption, string country)
    {
        if (!country.Equals("NO", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Unsupported country for credit agreement PDF.");

        var countryCode = _marketService.CountryId.GetCountryCode();

        var addressDataJson = TempData.Peek("AddressData") as string;
        if (string.IsNullOrWhiteSpace(addressDataJson))
            return BadRequest("Address data is missing.");

        var addresses = JsonSerializer.Deserialize<CustomerAddress[]>(addressDataJson);
        if (addresses == null || addresses.Length == 0)
            return BadRequest("No addresses available.");

        if (SelectedAddressIndex < 0 || SelectedAddressIndex >= addresses.Length)
            return BadRequest("Invalid address selection.");

        var a = addresses[SelectedAddressIndex];
        var fullName = !string.IsNullOrWhiteSpace(a.LegalName)
            ? a.LegalName
            : $"{a.FirstName?.Trim()} {a.LastName?.Trim()}".Trim();

        var street = $"{a.AddressLine1} {a.AddressLine2 ?? ""}".Trim();
        var postal = !string.IsNullOrWhiteSpace(a.Zipcode) ? a.Zipcode : a.Postcode.ToString();
        var city = a.Postarea;
        var nationalId = a.SecurityNumber ?? "";
        var totalAmount = _cartService.CartLines.Sum(line => line.CalculateTotal());

        var response = await WebpayConnection
            .GetInvoiceCreditAgreementPdf(Config)
            .SetCountryCode(countryCode)
            .SetFullName(fullName)
            .SetStreetAddress(street)
            .SetPostalCode(postal)
            .SetCity(city)
            .SetOrderCreatedDate(DateTime.UtcNow)
            .SetTotalAmount(totalAmount)
            .SetNationalId(nationalId)
            .DoRequestAsync();

        if (response == null)
            return StatusCode(500, "No response returned from service.");

        if (string.IsNullOrWhiteSpace(response.FileBinaryDataBase64))
            return NotFound("No PDF returned from service.");

        byte[] pdfBytes;
        try
        {
            pdfBytes = Convert.FromBase64String(response.FileBinaryDataBase64);
        }
        catch
        {
            return BadRequest("Invalid base64 PDF returned from service.");
        }

        return File(pdfBytes, "application/pdf", "kredittavtale.pdf");
    }

    private void SaveTempData()
    {
        TempData["IsTestCustomersVisible"] = IsTestCustomersVisible;
        TempData["CountryId"] = _marketService.CountryId;

        TempData.Keep("IsTestCustomersVisible");
        TempData.Keep("CountryId");
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
