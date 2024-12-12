namespace Sample.AspNetCore.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.AspNetCore.Models;
using System.Threading.Tasks;

public class CurrencyController : Controller
{
    private readonly ILogger<CurrencyController> logger;
    private readonly Market marketService;

    public CurrencyController(ILogger<CurrencyController> logger, Market marketService)
    {
        this.logger = logger;
        this.marketService = marketService;
    }

    public async Task<IActionResult> SetCurrency(string currencyCode)
    {
        //this.marketService.SetCurrency(currencyCode);
        logger.LogInformation($"Language changed to {currencyCode}");
        marketService.Update();
        return await Task.FromResult(Redirect(Request.Headers["Referer"].ToString()));
    }
}
