using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Sample.AspNetCore.Pages;

public class IndexModel : PageModel
{
    public IndexModel(ILogger<IndexModel> logger)
    {
    }

    public void OnGet()
    {
        
    }
}
