using System.Xml;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Payment
{
    /// <summary>
    /// Defines all direct payments to bank viewable in PayPage
    /// </summary>
    public class DirectPayment : HostedPayment
    {
        public DirectPayment(CreateOrderBuilder orderBuilder) : base(orderBuilder)
        {
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>DirectPayment</returns>
        public DirectPayment SetReturnUrl(string returnUrl)
        {
            ReturnUrl = returnUrl;
            return this;
        }

        public DirectPayment SetCancelUrl(string returnUrl)
        {
            CancelUrl = returnUrl;
            return this;
        }

        public DirectPayment SetPayPageLanguageCode(LanguageCode languageCode)
        {
            LanguageCode = languageCode.ToString().ToLower();
            return this;
        }

        public DirectPayment ConfigureExcludedPaymentMethod()
        {
            CountryCode countryCode = CrOrderBuilder.GetCountryCode();

            if (countryCode != CountryCode.SE)
            {
                ExcludedPaymentMethod.Add(PaymentMethod.SEBSE.Value);
                ExcludedPaymentMethod.Add(PaymentMethod.NORDEASE.Value);
                ExcludedPaymentMethod.Add(PaymentMethod.SEBFTGSE.Value);
                ExcludedPaymentMethod.Add(PaymentMethod.SHBSE.Value);
                ExcludedPaymentMethod.Add(PaymentMethod.SWEDBANKSE.Value);
            }
            if (countryCode != (CountryCode.NO))
            {
                ExcludedPaymentMethod.Add(PaymentMethod.BANKAXESS.Value);
            }

            ExcludedPaymentMethod.Add(PaymentMethod.PAYPAL.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.KORTCERT.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SKRILL.Value);

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

        public override void WritePaymentSpecificXml(XmlWriter xmlw)
        {
        }
    }
}