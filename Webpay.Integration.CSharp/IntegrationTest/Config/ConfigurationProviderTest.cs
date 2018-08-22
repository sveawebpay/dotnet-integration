using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.IntegrationTest.Config
{
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

            Assert.That(_defaultConf.GetMerchantId(PaymentType.HOSTED, countrycode), Is.EqualTo("1130"));

            Assert.That(_defaultConf.GetSecretWord(PaymentType.HOSTED, countrycode),
                        Is.EqualTo("8a9cece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3"));

            Assert.That(_defaultConf.GetEndPoint(PaymentType.HOSTED),
                        Is.EqualTo("https://webpaypaymentgatewaystage.svea.com/webpay/payment"));
            Assert.That(_defaultConf.GetEndPoint(PaymentType.INVOICE),
                        Is.EqualTo("https://webpaywsstage.svea.com/SveaWebPay.asmx?WSDL"));
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
        public void TestConnectionWithTestConfiguration()
        {
            var conf = new ConfigurationProviderTestData();
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(conf)
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                             .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.That(response.Accepted, Is.True);
        }

        [Test]
        public void TestConnectionCreateOrderFailsIfNoConfigurationIsProvided()
        {
            var exception = Assert.Throws<SveaWebPayException>(() => WebpayConnection.CreateOrder()
                                                                                     .UseInvoicePayment()
                                                                                     .DoRequest());

            Assert.That(exception.Message, Is.EqualTo(ExpectedMessage));
        }

        [Test]
        public void TestConnectionDeliverOrderFailsIfNoConfigurationIsProvided()
        {
            var exception = Assert.Throws<SveaWebPayException>(() => WebpayConnection.DeliverOrder()
                                                                                     .DeliverInvoiceOrder()
                                                                                     .DoRequest());

            Assert.That(exception.Message, Is.EqualTo(ExpectedMessage));
        }

        [Test]
        public void TestConnectionGetAddressFailsIfNoConfigurationIsProvided()
        {
            var exception = Assert.Throws<SveaWebPayException>(() => WebpayConnection.GetAddresses()
                                                                                     .DoRequest());

            Assert.That(exception.Message, Is.EqualTo(ExpectedMessage));
        }

        [Test]
        public void TestConnectionGetPaymentPlanFailsIfNoConfigurationIsProvided()
        {
            var exception = Assert.Throws<SveaWebPayException>(() => WebpayConnection.GetPaymentPlanParams()
                                                                                     .DoRequest());

            Assert.That(exception.Message, Is.EqualTo(ExpectedMessage));
        }
    }
}