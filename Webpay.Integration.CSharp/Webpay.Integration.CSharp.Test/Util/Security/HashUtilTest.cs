using NUnit.Framework;
using Webpay.Integration.CSharp.Util.Security;

namespace Webpay.Integration.CSharp.Test.Util.Security
{
    [TestFixture]
    public class HashUtilTest
    {
        [Test]
        public void TestCreateHash()
        {
            const string hash =
                "fe54c6e8727e9f8bf5f4f8e47a05567d694f68049cd1f116c19d9a6fbd066a742305d23da164291bca8869c34e7b8ff3bee15ab2da011d4ddc57adc736bc12ba";
            Assert.AreEqual(hash, HashUtil.CreateHash("Hsjhasj djahs djahs d"));
        }
    }
}