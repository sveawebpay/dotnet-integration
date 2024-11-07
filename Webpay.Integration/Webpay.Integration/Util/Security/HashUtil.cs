using System.Security.Cryptography;
using System.Text;

namespace Webpay.Integration.Util.Security;

public static class HashUtil
{
    public static string CreateHash(string inputString)
    {
        var utfEncoding = new UTF8Encoding();
        byte[] message = utfEncoding.GetBytes(inputString);

        var hashString = new SHA512Managed();
        string hex = String.Empty;

        byte[] hashValue = hashString.ComputeHash(message);
        foreach (byte x in hashValue)
        {
            hex += String.Format("{0:x2}", x);
        }
        return hex;
    }

    public static byte[] GetBytesFromString(string str)
    {
        var bytes = new byte[str.Length * sizeof (char)];
        Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }
}