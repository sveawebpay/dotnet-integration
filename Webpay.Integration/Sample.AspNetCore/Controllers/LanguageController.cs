namespace Sample.AspNetCore.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sample.AspNetCore.Models;

using System.Threading.Tasks;

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
        this.logger.LogInformation($"Language changed to {languageId}");
        this.marketService.Update();
        return await Task.FromResult(Redirect(Request.Headers["Referer"].ToString()));
    }
}
