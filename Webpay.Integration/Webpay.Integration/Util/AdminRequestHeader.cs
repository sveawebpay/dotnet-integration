namespace Webpay.Integration.Util;

public class AdminRequestHeader
{
    public AdminRequestHeader(string key, object value) {
        Header = new KeyValuePair<string, object>(key, value);
    }
   public KeyValuePair<string, object> Header { get; set; }
}
