using System.Collections.Generic;
using System.Xml;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Payment
{
    /// <summary>
    /// Defines specific payment methods to be shown in PayPage
    /// </summary>
    public class PayPagePayment : HostedPayment
    {
        private string _paymentMethod;
        private readonly List<string> _includedPaymentMethod;

        public PayPagePayment(CreateOrderBuilder orderBuilder) : base(orderBuilder)
        {
            _includedPaymentMethod = new List<string>();
        }

        public string GetPaymentMethod()
        {
            return _paymentMethod;
        }

        public PayPagePayment SetPaymentMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod == PaymentMethod.INVOICE)
            {
                _paymentMethod = GetValidInvoiceTypeForIncludedList();
            }
            else if (paymentMethod == PaymentMethod.PAYMENTPLAN)
            {
                _paymentMethod = GetValidPaymentPlanTypeForIncludedList();
            }
            else
            {
                _paymentMethod = paymentMethod.Value;
            }
            return this;
        }

        public List<string> GetIncludedPaymentMethod()
        {
            return _includedPaymentMethod;
        }

        /// <summary>
        /// Only used in CardPayment and DirectPayment
        /// </summary>
        public PayPagePayment SetReturnUrl(string returnUrl)
        {
            ReturnUrl = returnUrl;
            return this;
        }

        public PayPagePayment SetCancelUrl(string returnUrl)
        {
            CancelUrl = returnUrl;
            return this;
        }

        public PayPagePayment SetCallbackUrl(string callbackUrl)
        {
            CallbackUrl = callbackUrl;
            return this;
        }

        public PayPagePayment SetPayPageLanguageCode(LanguageCode languageCode)
        {
            LanguageCode = languageCode.ToString().ToLower();
            return this;
        }

        public PayPagePayment ConfigureExcludedPaymentMethod()
        {
            return this;
        }

        public PayPagePayment ExcludeCardPaymentMethod()
        {
            ExcludedPaymentMethod.Add(PaymentMethod.KORTCERT.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SVEACARDPAY.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SKRILL.Value);

            return this;
        }

        public PayPagePayment ExcludeDirectPaymentMethod()
        {
            ExcludedPaymentMethod.Add(PaymentMethod.NORDEASE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SEBSE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SEBFTGSE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SHBSE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SWEDBANKSE.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.BANKAXESS.Value);
            return this;
        }

        public PayPagePayment ExcludePaymentMethod(IEnumerable<PaymentMethod> paymentMethods)
        {
            AddCollectionToExcludePaymentMethodList(paymentMethods);
            return this;
        }

        private void AddCollectionToExcludePaymentMethodList(IEnumerable<PaymentMethod> paymentMethods)
        {
            foreach (var pm in paymentMethods)
            {
                if (pm == PaymentMethod.INVOICE)
                {
                    ExcludedPaymentMethod.AddRange(InvoiceType.GetAllInvoiceValues());
                }
                else if (pm == PaymentMethod.PAYMENTPLAN)
                {
                    ExcludedPaymentMethod.AddRange(PaymentPlanType.AllPaymentPlanValues());
                }
                else
                {
                    ExcludedPaymentMethod.Add(pm.Value);
                }
            }
        }

        public PayPagePayment ExcludePaymentMethod()
        {
            var emptyList = new List<PaymentMethod>();
            return ExcludePaymentMethod(emptyList);
        }

        public PayPagePayment IncludePaymentMethod()
        {
            var emptyList = new List<PaymentMethod>();
            return IncludePaymentMethod(emptyList);
        }

        public PayPagePayment IncludePaymentMethod(IEnumerable<PaymentMethod> paymentMethods)
        {
            AddCollectionToIncludedPaymentMethodList(paymentMethods);

            // Exclude all payment methods		
            var excluded = new ExcludePayments();
            ExcludedPaymentMethod = excluded.ExcludeInvoicesAndPaymentPlan();

            ExcludedPaymentMethod.Add(PaymentMethod.KORTCERT.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SVEACARDPAY.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.SKRILL.Value);
            ExcludedPaymentMethod.Add(PaymentMethod.PAYPAL.Value);
            ExcludeDirectPaymentMethod();

            // Remove the included methods from the excluded payment methods
            foreach (string pm in _includedPaymentMethod)
            {
                ExcludedPaymentMethod.Remove(pm);
            }

            return this;
        }

        private string GetValidPaymentPlanTypeForIncludedList()
        {
            foreach (var ppt in PaymentPlanType.AllPaymentPlanValueTypes)
            {
                //never include from old flow to include list - won´t show in paypage
                if (CrOrderBuilder.GetCountryCode() == CountryCode.SE &&
                    ppt == PaymentPlanType.PAYMENTPLANSE)
                {
                    continue;
                }
                //include only Payment plan for current country 
                if (ppt.CountryCode == CrOrderBuilder.GetCountryCode())
                {
                    return ppt.Value;
                }
            }
            return "";
        }

        private string GetValidInvoiceTypeForIncludedList()
        {
            foreach (var it in InvoiceType.AllInvoiceValueTypes)
            {
                //never include old flow to include list
                if (CrOrderBuilder.GetCountryCode() == CountryCode.SE && it == InvoiceType.INVOICESE)
                {
                    continue;
                }
                if (it.CountryCode == CrOrderBuilder.GetCountryCode())
                {
                    return it.Value;
                }
            }
            return "";
        }

        private void AddCollectionToIncludedPaymentMethodList(IEnumerable<PaymentMethod> paymentMethods)
        {
            foreach (var pm in paymentMethods)
            {
                if (pm == PaymentMethod.INVOICE)
                {
                    _includedPaymentMethod.Add(GetValidInvoiceTypeForIncludedList());
                }
                else if (pm == PaymentMethod.PAYMENTPLAN)
                {
                    _includedPaymentMethod.Add(GetValidPaymentPlanTypeForIncludedList());
                }
                else
                {
                    _includedPaymentMethod.Add(pm.Value);
                }
            }
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
            if (_paymentMethod != null)
            {
                WriteSimpleElement(xmlw, "paymentmethod", _paymentMethod);
            }
        }
    }
}