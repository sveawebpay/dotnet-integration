using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.IntegrationTest.Config;

[TestFixture]
public class ConfigurationProviderTest
{
    private IConfigurationProvider _defaultConf;
    private const string ExpectedMessage = "A configuration must be provided. For testing purposes use SveaConfig.GetDefaultConfig()";

    [SetUp]
    public void SetUp()
    {
        _defaultConf = SveaConfig.GetDefaultConfig();
    }

    [Test]
    public void TestGetDefaultConfigSe()
    {
        const CountryCode countrycode = CountryCode.SE;

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode), Is.EqualTo("sverigetest"));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode), Is.EqualTo("sverigetest"));

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetUsername(PaymentType.PAYMENTPLAN, countrycode)));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetPassword(PaymentType.PAYMENTPLAN, countrycode)));

        Assert.That(_defaultConf.GetClientNumber(PaymentType.INVOICE, countrycode), Is.EqualTo(79021));
        Assert.That(_defaultConf.GetClientNumber(PaymentType.PAYMENTPLAN, countrycode), Is.EqualTo(59999));

        Assert.That(_defaultConf.GetMerchantId(PaymentType.HOSTED, countrycode), Is.EqualTo("1110"));

        Assert.That(_defaultConf.GetSecretWord(PaymentType.HOSTED, countrycode),
                    Is.EqualTo("1f8bcd8a564073f7156efd2522d5998f5487a1dcd19e1e120276fb1fb7e233a6059c45d6eb44a8d7342a4989bbb95acd4708051bbc145bda43ae0dd3503928db"));
    }

    [Test]
    public void TestGetDefaultConfigDk()
    {
        const CountryCode countrycode = CountryCode.DK;

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode), Is.EqualTo("danmarktest2"));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode), Is.EqualTo("danmarktest2"));

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetUsername(PaymentType.PAYMENTPLAN, countrycode)));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetPassword(PaymentType.PAYMENTPLAN, countrycode)));

        Assert.That(_defaultConf.GetClientNumber(PaymentType.INVOICE, countrycode), Is.EqualTo(62008));
        Assert.That(_defaultConf.GetClientNumber(PaymentType.PAYMENTPLAN, countrycode), Is.EqualTo(64008));
    }

    [Test]
    public void TestGetDefaultConfigDe()
    {
        const CountryCode countrycode = CountryCode.DE;

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode), Is.EqualTo("germanytest"));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode), Is.EqualTo("germanytest"));

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetUsername(PaymentType.PAYMENTPLAN, countrycode)));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetPassword(PaymentType.PAYMENTPLAN, countrycode)));

        Assert.That(_defaultConf.GetClientNumber(PaymentType.INVOICE, countrycode), Is.EqualTo(14997));
        Assert.That(_defaultConf.GetClientNumber(PaymentType.PAYMENTPLAN, countrycode), Is.EqualTo(16997));
    }

    [Test]
    public void TestGetDefaultConfigFi()
    {
        const CountryCode countrycode = CountryCode.FI;

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode), Is.EqualTo("finlandtest2"));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode), Is.EqualTo("finlandtest2"));

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetUsername(PaymentType.PAYMENTPLAN, countrycode)));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetPassword(PaymentType.PAYMENTPLAN, countrycode)));

        Assert.That(_defaultConf.GetClientNumber(PaymentType.INVOICE, countrycode), Is.EqualTo(26136));
        Assert.That(_defaultConf.GetClientNumber(PaymentType.PAYMENTPLAN, countrycode), Is.EqualTo(27136));
    }

    [Test]
    public void TestGetDefaultConfigNo()
    {
        const CountryCode countrycode = CountryCode.NO;

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode), Is.EqualTo("norgetest2"));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode), Is.EqualTo("norgetest2"));

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetUsername(PaymentType.PAYMENTPLAN, countrycode)));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetPassword(PaymentType.PAYMENTPLAN, countrycode)));

        Assert.That(_defaultConf.GetClientNumber(PaymentType.INVOICE, countrycode), Is.EqualTo(33308));
        Assert.That(_defaultConf.GetClientNumber(PaymentType.PAYMENTPLAN, countrycode), Is.EqualTo(32503));
    }

    [Test]
    public void TestGetDefaultConfigNl()
    {
        const CountryCode countrycode = CountryCode.NL;

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode), Is.EqualTo("hollandtest"));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode), Is.EqualTo("hollandtest"));

        Assert.That(_defaultConf.GetUsername(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetUsername(PaymentType.PAYMENTPLAN, countrycode)));
        Assert.That(_defaultConf.GetPassword(PaymentType.INVOICE, countrycode),
                    Is.EqualTo(_defaultConf.GetPassword(PaymentType.PAYMENTPLAN, countrycode)));

        Assert.That(_defaultConf.GetClientNumber(PaymentType.INVOICE, countrycode), Is.EqualTo(85997));
        Assert.That(_defaultConf.GetClientNumber(PaymentType.PAYMENTPLAN, countrycode), Is.EqualTo(86997));
    }

    [Test]
    public void TestProductionUrls()
    {
        Assert.That(SveaConfig.GetProdPayPageUrl(), Is.EqualTo("https://webpaypaymentgateway.svea.com/webpay/payment"));
        Assert.That(SveaConfig.GetProdWebserviceUrl(), Is.EqualTo("https://webpayws.svea.com/SveaWebPay.asmx?WSDL"));
    }

    [Test]
    public async Task TestConnectionWithTestConfiguration()
    {
        var conf = new ConfigurationProviderTestData();
        var response = await WebpayConnection.CreateOrder(conf)
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequestAsync();

        Assert.That(response.Accepted, Is.True);
    }

    [Test]
    public async Task TestConnectionCreateOrderFailsIfNoConfigurationIsProvided()
    {
        var exception = Assert.ThrowsAsync<SveaWebPayException>(async () =>
            await WebpayConnection.CreateOrder()
                                  .UseInvoicePayment()
                                  .DoRequestAsync());

        Assert.That(exception.Message, Is.EqualTo(ExpectedMessage));
    }

    [Test]
    public async Task TestConnectionDeliverOrderFailsIfNoConfigurationIsProvided()
    {
        var exception = Assert.ThrowsAsync<SveaWebPayException>(async () =>
            await WebpayConnection.DeliverOrder()
                                  .DeliverInvoiceOrder()
                                  .DoRequestAsync());

        Assert.That(exception.Message, Is.EqualTo(ExpectedMessage));
    }

    [Test]
    public async Task TestConnectionGetAddressFailsIfNoConfigurationIsProvided()
    {
        var exception = Assert.ThrowsAsync<SveaWebPayException>(async () =>
            await WebpayConnection.GetAddresses()
                                  .DoRequestAsync());

        Assert.That(exception.Message, Is.EqualTo(ExpectedMessage));
    }

    [Test]
    public async Task TestConnectionGetPaymentPlanFailsIfNoConfigurationIsProvided()
    {
        var exception = Assert.ThrowsAsync<SveaWebPayException>(async () =>
            await WebpayConnection.GetPaymentPlanParams()
                                  .DoRequestAsync());

        Assert.That(exception.Message, Is.EqualTo(ExpectedMessage));
    }
}