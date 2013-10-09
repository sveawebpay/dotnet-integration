using System.Xml;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Payment
{
    /// <summary>
    /// Defines all card payments viewable in PayPage
    /// </summary>
    public class CardPayment : HostedPayment
    {
        public CardPayment(CreateOrderBuilder orderBuilder) : base(orderBuilder)
        {
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>CardPayment</returns>
        public CardPayment SetReturnUrl(string returnUrl)
        {
            ReturnUrl = returnUrl;
            return this;
        }

        public CardPayment SetCancelUrl(string returnUrl)
        {
            CancelUrl = returnUrl;
            return this;
        }

        public CardPayment SetPayPageLanguageCode(LanguageCode languageCode)
        {
            LanguageCode = languageCode.ToString().ToLower();
            return this;
        }

        public CardPayment ConfigureExcludedPaymentMethod()
        {
            //Payment service providers
            ExcludedPaymentMethod.Add(PaymentMethod.PAYPAL.Value);

            //Direct bank payment methods
            ExcludedPaymentMethod.Add(PaymentMethod.NORDEASE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SEBSE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SEBFTGSE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SHBSE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SWEDBANKSE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.BANKAXESS.Value);

            //Invoices and payment plan      
            ExcludedPaymentMethod.AddRange(Excluded.ExcludeInvoicesAndPaymentPlan());

            return this;
        }

        /// <summary>
        /// CalculateRequestValues
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        public override void CalculateRequestValues()
        {
            FormatRequestValues();
            ConfigureExcludedPaymentMethod();
        }

        public override XmlWriter GetPaymentSpecificXml(XmlWriter xmlw)
        {
            return xmlw;
        }
    }
}
