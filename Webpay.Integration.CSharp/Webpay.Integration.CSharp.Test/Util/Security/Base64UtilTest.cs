using NUnit.Framework;
using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Test.Util.Security
{
    [TestFixture]
    public class Base64UtilTest
    {
        private const string Plain = "JAs dkjhas djha sdjha jsdh ajhsd jash";
        private const string Encoded = "SkFzIGRramhhcyBkamhhIHNkamhhIGpzZGggYWpoc2QgamFzaA==";

        [Test]
        public void TestDecodeBase64String()
        {
            Assert.AreEqual(Plain, Base64Util.DecodeBase64String(Encoded));
        }

        [Test]
        public void TestEncodeBase64String()
        {
            Assert.AreEqual(Encoded, Base64Util.EncodeBase64String(Plain));
        }
    }
}
