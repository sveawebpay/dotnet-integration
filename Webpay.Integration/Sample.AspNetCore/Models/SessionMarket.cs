using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sample.AspNetCore.Extensions;
using System;
using System.Text.Json.Serialization;

namespace Sample.AspNetCore.Models;

public class SessionMarket : Market
{
    public const string MarketSessionKey = "_Market";
    [JsonIgnore] public ISession Session { get; set; }

    public override void SetMarket(string marketId)
    {
        base.SetMarket(marketId);
        Session.SetJson(MarketSessionKey, this);
    }

    //public override void SetLanguage(string languageId)
    //{
    //    base.SetLanguage(languageId);
    //    Session.SetJson(MarketSessionKey, this);
    //}

    //public override void SetCurrency(string currencyCode)
    //{
    //    base.SetCurrency(currencyCode);
    //    Session.SetJson(MarketSessionKey, this);
    //}

    public override void SetCountry(string countryId)
    {
        base.SetCountry(countryId);
        Session.SetJson(MarketSessionKey, this);
    }

    public static Market GetMarket(IServiceProvider services)
    {
        var session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
        var market = session?.GetJson<SessionMarket>(MarketSessionKey) ?? new SessionMarket();

        market.Session = session;
        return market;
    }
}
