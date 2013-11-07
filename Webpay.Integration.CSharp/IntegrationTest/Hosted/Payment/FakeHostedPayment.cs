using System.Xml;
using Webpay.Integration.CSharp.Hosted.Payment;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.IntegrationTest.Hosted.Payment
{
    internal class FakeHostedPayment : HostedPayment
    {
        public FakeHostedPayment(CreateOrderBuilder createOrderBuilder) : base(createOrderBuilder)
        {
        }

        public FakeHostedPayment SetCancelUrl(string cancelUrl)
        {
            CancelUrl = cancelUrl;
            return this;
        }

        public FakeHostedPayment SetReturnUrl(string returnUrl)
        {
            ReturnUrl = returnUrl;
            return this;
        }

        public FakeHostedPayment SetPayPageLanguageCode(LanguageCode languageCode)
        {
            LanguageCode = languageCode.ToString().ToLower();
            return this;
        }

        public FakeHostedPayment ConfigureExcludedPaymentMethod()
        {
            return this;
        }

        public override void CalculateRequestValues()
        {
            FormatRequestValues();
            ConfigureExcludedPaymentMethod();
        }

        public override void WritePaymentSpecificXml(XmlWriter xmlw)
        {
        }
    }
}