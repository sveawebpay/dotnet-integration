using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.AspNetCore.Models;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Controllers;

public class LanguageController : Controller
{
    private readonly ILogger<LanguageController> logger;
    private readonly Market marketService;

    public LanguageController(ILogger<LanguageController> logger, Market marketService)
    {
        this.logger = logger;
        this.marketService = marketService;
    }

    public async Task<IActionResult> SetLanguage(string languageId)
    {
        //this.marketService.SetLanguage(languageId);
        logger.LogInformation($"Language changed to {languageId}");
        marketService.Update();
        return await Task.FromResult(Redirect(Request.Headers["Referer"].ToString()));
    }
}
