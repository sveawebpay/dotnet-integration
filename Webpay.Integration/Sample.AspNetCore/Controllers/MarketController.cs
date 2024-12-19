using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Sample.AspNetCore.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Controllers;

public class MarketController : Controller
{
    private readonly ILogger<MarketController> logger;
    private readonly Market marketService;
    private readonly List<MarketSettings> markets;

    public MarketController(ILogger<MarketController> logger, Market marketService, IOptions<List<MarketSettings>> markets)
    {
        this.logger = logger;
        this.marketService = marketService;
        this.markets = markets.Value;
    }

    public async Task<IActionResult> SetMarket(string marketId)
    {
        var selectedMarket = markets.FirstOrDefault(x => x.Id.Equals(marketId, StringComparison.InvariantCultureIgnoreCase));
        if (selectedMarket != null)
        {
            marketService.ChangeMarket(selectedMarket);
            logger.LogInformation($"Market changed to {selectedMarket.Id}");
        }

        return await Task.FromResult(Redirect(Request.Headers["Referer"].ToString()));
    }
}
