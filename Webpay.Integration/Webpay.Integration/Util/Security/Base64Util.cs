namespace Webpay.Integration.Util.Security;

public abstract class Base64Util
{
    public static string EncodeBase64String(string toEncode)
    {
        var toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);
        return Convert.ToBase64String(toEncodeAsBytes);
    }

    public static string DecodeBase64String(string encodedData)
    {
        var encodedDataAsBytes = Convert.FromBase64String(encodedData);
        return System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
    }
}