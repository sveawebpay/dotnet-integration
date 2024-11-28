namespace Sample.AspNetCore.Components;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Sample.AspNetCore.Models;

using System;
using System.Collections.Generic;
using System.Linq;

public class MarketSelectorViewComponent : ViewComponent
{
    private readonly Market marketService;
    private readonly List<MarketSettings> markets;

    public MarketSelectorViewComponent(Market marketService, IOptions<List<MarketSettings>> markets)
    {
        this.marketService = marketService;
        this.markets = markets.Value;
    }

    public IViewComponentResult Invoke()
    {
        var countries = markets.Select(x => x.Countries).Distinct();

        var market = markets.FirstOrDefault(x => x.Id.Equals(marketService.MarketId, StringComparison.InvariantCultureIgnoreCase));
        var model = new MarketSelectorViewModel
        {
            AvailableMarkets = markets.Select(x => x.Id),
            AvailableCountries = markets.SelectMany(x => x.Countries).Distinct(),
            SelectedMarket = market,
            SelectedCurrencyCode = marketService.CurrencyCode,
            SelectedLanguage = marketService.LanguageId,
            SelectedCountry = marketService.CountryId
        };
        return View(model);
    }
}

public class MarketSelectorViewModel
{
    public MarketSettings SelectedMarket { get; set; }
    public string SelectedLanguage { get; set; }
    public string SelectedCurrencyCode { get; set; }
    public string SelectedCountry { get; set; }
    public IEnumerable<string> AvailableMarkets { get; set; }
    public IEnumerable<string> AvailableCountries { get; set; }
}
