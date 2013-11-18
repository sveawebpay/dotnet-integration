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
            Assert.That(Base64Util.DecodeBase64String(Encoded), Is.EqualTo(Plain));
        }

        [Test]
        public void TestEncodeBase64String()
        {
            Assert.That(Base64Util.EncodeBase64String(Plain), Is.EqualTo(Encoded));
        }
    }
}