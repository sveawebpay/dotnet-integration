namespace Webpay.Integration.CSharp.Util.Security
{
    public abstract class Base64Util
    {
        public static string EncodeBase64String(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string DecodeBase64String(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            return System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
        }
    }
}
