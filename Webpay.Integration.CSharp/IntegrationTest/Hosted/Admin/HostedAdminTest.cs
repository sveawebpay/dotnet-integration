using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Security;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Payment;

namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Admin
{
    [TestFixture]
    public class HostedAdminTest
    {
        /// <summary>
        /// Here or in the othere tests?
        /// </summary>
        [Test]
        public void TestPreparedPaymentRequest()
        {
            Uri uri = PreparePayment(PaymentMethod.NORDEASE);

            Assert.That(uri.AbsoluteUri, Is.StringMatching(".*\\/preparedpayment\\/[0-9]+"));
        }

        private static Uri PreparePayment(PaymentMethod paymentMethod)
        {
            return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetClientOrderNumber("1" + Guid.NewGuid().ToString().Replace("-", ""))
                .SetCurrency(TestingTool.DefaultTestCurrency)
                .UsePaymentMethod(paymentMethod)
                .___SetSimulatorCode_ForTestingOnly("0")
                .SetReturnUrl(
                    "https://test.sveaekonomi.se/webpay/public/static/testlandingpage.html")
                .PreparePayment("127.0.0.1");
        }

        [Test]
        public void TestAnnulPaymentRequest()
        {
            var payment = MakePreparedPayment(PreparePayment(PaymentMethod.KORTCERT));

            var preparedHostedAdminRequest = WebpayAdmin
                .Hosted(SveaConfig.GetDefaultConfig(), "1130", CountryCode.SE)
                .Annul(new Annul(
                    transactionId: payment.TransactionId
                ));

            HostedAdminRequest hostedAdminRequest = preparedHostedAdminRequest.Request();
            Assert.That(hostedAdminRequest.XmlDoc.SelectSingleNode("/annul/transactionid").InnerText, Is.EqualTo(payment.TransactionId + ""));
            var hostedAdminResponse = preparedHostedAdminRequest.DoRequest();
            Assert.That(hostedAdminResponse.MessageDocument.SelectSingleNode("//statuscode").InnerText, Is.EqualTo("0"));
        }

        private PaymentResponse MakePreparedPayment(Uri preparePayment)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(preparePayment);
            webRequest.AllowAutoRedirect = false;
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var location = new Uri(webResponse.Headers["Location"]);
            var nameValueCollection = HttpUtility.ParseQueryString(location.Query);
            var messageBase64 = nameValueCollection["response"];
            var merchantId = nameValueCollection["merchantid"];
            var mac = nameValueCollection["mac"];

            return new PaymentResponse(messageBase64, mac, merchantId);
        }
    }

    public class PaymentResponse
    {
        public string MerchantId { get; private set; }
        public string Mac { get; private set; }
        public string MessageBase64 { get; private set; }
        public string Message { get; private set; }
        public XmlDocument MessageXmlDoc { get; private set; }
        public long TransactionId { get; private set; }

        public PaymentResponse(string messageBase64, string mac, string merchantId)
        {
            Mac = mac;
            MessageBase64 = messageBase64;
            MerchantId = merchantId;

            Message = Base64Util.DecodeBase64String(messageBase64);
            MessageXmlDoc = new XmlDocument();
            MessageXmlDoc.LoadXml(Message);

            TransactionId = long.Parse(MessageXmlDoc.SelectSingleNode("//transaction").Attributes["id"].Value);
        }

    }

    public class Annul
    {
        public long TransactionId { get; private set; }

        public Annul(long transactionId)
        {
            TransactionId = transactionId;
        }
    }

    public class WebpayAdmin
    {
        public static HostedAdmin Hosted(IConfigurationProvider configurationProvider, string merchantId, CountryCode countryCode)
        {
            return new HostedAdmin(configurationProvider, merchantId, countryCode);
        }
    }

    public class HostedAdmin
    {
        public IConfigurationProvider ConfigurationProvider { get; private set; }
        public string MerchantId { get; private set; }
        public CountryCode CountryCode { get; private set; }

        public HostedAdmin(IConfigurationProvider configurationProvider, string merchantId, CountryCode countryCode)
        {
            ConfigurationProvider = configurationProvider;
            MerchantId = merchantId;
            CountryCode = countryCode;
        }

        public PreparedHostedAdminRequest Annul(Annul annul)
        {
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <annul>
                <transactionid>{0}</transactionid>
                </annul>", annul.TransactionId);

            return new PreparedHostedAdminRequest(xml, CountryCode, MerchantId, ConfigurationProvider, "/annul");
        }
    }

    public class PreparedHostedAdminRequest
    {
        public readonly string Xml;
        public readonly CountryCode CountryCode;
        public readonly string MerchantId;
        public readonly IConfigurationProvider ConfigurationProvider;
        public readonly string ServicePath;

        public PreparedHostedAdminRequest(string xml, CountryCode countryCode, string merchantId, IConfigurationProvider configurationProvider, string servicePath)
        {
            Xml = xml;
            CountryCode = countryCode;
            MerchantId = merchantId;
            ConfigurationProvider = configurationProvider;
            ServicePath = servicePath;
        }

        public HostedAdminResponse DoRequest()
        {
            return HostedAdminRequest.HostedAdminCall(GetEndPointBase(), Request());
        }

        private string GetEndPointBase()
        {
            var endPoint = ConfigurationProvider.GetEndPoint(PaymentType.HOSTED);
            var baseUrl = endPoint.Replace("/payment", "");

            var targetAddress = baseUrl + "/rest" + ServicePath;
            return targetAddress;
        }

        public HostedAdminRequest Request()
        {
            return new HostedAdminRequest(Xml, ConfigurationProvider.GetSecretWord(PaymentType.HOSTED, CountryCode), MerchantId);
        }
    }
}