using Webpay.Integration.Webservice.Helper;

namespace Webpay.Integration.Test.Webservice.Helper;

[TestFixture]
public class PeppolIdTest
{
    [Test]
    public void TestValidPeppolId()
    {
        bool validationResult = PeppolId.IsValidPeppolId("1234:asdf");
        Assert.IsTrue(validationResult);
    }

    [Test]
    public void TestInvalidPeppolId()
    {
        bool validationResult = PeppolId.IsValidPeppolId("1:1");
        Assert.IsTrue(!validationResult);
    }
}
