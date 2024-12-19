namespace Sample.AspNetCore.Models;

using System.Collections.Generic;

public class MarketSettings
{
    public string Id { get; set; }
    public IEnumerable<string> Languages { get; set; }
    public IEnumerable<string> Currencies { get; set; }
    public IEnumerable<string> Countries { get; set; }
}
