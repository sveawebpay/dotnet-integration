using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Controllers;

public class CountryController : Controller
{
    private readonly ILogger<CountryController> logger;
    private readonly Market marketService;
    private readonly List<MarketSettings> markets;

    public CountryController(ILogger<CountryController> logger, Market marketService, IOptions<List<MarketSettings>> markets)
    {
        this.logger = logger;
        this.marketService = marketService;
        this.markets = markets.Value;
    }

    public async Task<IActionResult> SetCountry(string countryId)
    {
        marketService.SetCountry(countryId);
        var selectedMarket = markets.FirstOrDefault(x => x.Countries.ToList().Contains(countryId));
        if (selectedMarket != null)
        {
            marketService.ChangeMarket(selectedMarket);
            logger.LogInformation($"Market changed to {selectedMarket.Id}");
        }

        var requestRef = Request.Headers["Referer"].ToString();

        int checkoutIndex = requestRef.IndexOf("/checkout", StringComparison.OrdinalIgnoreCase);
        if (checkoutIndex >= 0)
        {
            string baseUrl = requestRef.Substring(0, checkoutIndex + "/Checkout".Length);
            if (baseUrl.EndsWith("/"))
                baseUrl = baseUrl.TrimEnd('/');

            string newUrl = baseUrl + "/LoadPaymentMenu";
            return await Task.FromResult(Redirect(newUrl));
        }

        return await Task.FromResult(Redirect(Request.Headers["Referer"].ToString()));
    }
}
