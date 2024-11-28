using System.Globalization;

namespace Sample.AspNetCore.Models;

public class SveaCheckoutSource
{
    public CultureInfo Culture { get; set; }
    public string Snippet { get; set; }
}