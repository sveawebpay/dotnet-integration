namespace Sample.AspNetCore.Models;

using System.Text.Json.Serialization;

public class PdfResponse
{
    [JsonPropertyName("pdf")]
    public string Pdf { get; set; }

    [JsonPropertyName("resultCode")]
    public int ResultCode { get; set; }
}
