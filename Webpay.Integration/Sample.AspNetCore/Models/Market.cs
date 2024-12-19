using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Sample.AspNetCore.Models;

public class Market
{
    private string _marketId;
    public string MarketId
    {
        get => !string.IsNullOrWhiteSpace(_marketId) ? _marketId : "SE";
        set => _marketId = value;
    }

    private string _languageId;
    public string LanguageId
    {
        get => !string.IsNullOrWhiteSpace(_languageId) ? _languageId : "sv-SE";
        set => _languageId = value;
    }

    private string _currencyCode;

    public string CurrencyCode
    {
        get => !string.IsNullOrWhiteSpace(_currencyCode) ? _currencyCode : "SEK";
        set => _currencyCode = value;
    }

    private string _countryId;
    public string CountryId
    {
        get => !string.IsNullOrWhiteSpace(_countryId) ? _countryId : "SE";
        set => _countryId = value;
    }

    public virtual void Update()
    {
    }

    public virtual void SetMarket(string marketId)
    {
        MarketId = marketId;
    }

    //public virtual void SetLanguage(string languageId)
    //{
    //    var language = new Language(languageId);
    //    LanguageId = languageId;
    //}

    //public virtual void SetCurrency(string currencyCode)
    //{
    //    var currency = new CurrencyCode(currencyCode);
    //    CurrencyCode = currency.ToString();
    //}

    public virtual void SetCountry(string countryId)
    {
        CountryId = countryId;
    }

    public void ChangeMarket(MarketSettings market)
    {
        SetMarket(market.Id);

        //if (!market.Languages.ToList().Contains(LanguageId))
        //{
        //    SetLanguage(market.Languages.FirstOrDefault());
        //}

        //if (!market.Currencies.Contains(CurrencyCode))
        //{
        //    SetCurrency(market.Currencies.FirstOrDefault());
        //}

        Update();
    }
}
