using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.AspNetCore.Models;
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

        return await Task.FromResult(Redirect(Request.Headers["Referer"].ToString()));
    }
}
