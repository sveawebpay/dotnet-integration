using System.Collections.Generic;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Hosted.Helper
{
    public class ExcludePayments
    {
        private readonly List<string> _excludedPaymentMethod;

        public ExcludePayments()
        {
            _excludedPaymentMethod = new List<string>();
        }

        /// <summary>
        /// List of all payment methods for invoices and payment plans
        /// </summary>
        /// <returns>paymentmethos string list</returns>
        public List<string> ExcludeInvoicesAndPaymentPlan()
        {
            SetPaymentMethodSe();
            SetPaymentMethodDe();
            SetPaymentMethodDk();
            SetPaymentMethodFi();
            SetPaymentMethodNl();
            SetPaymentMethodNo();

            return _excludedPaymentMethod;
        }

        private void SetPaymentMethodSe()
        {
            _excludedPaymentMethod.Add(InvoiceType.INVOICESE.Value);
            _excludedPaymentMethod.Add(InvoiceType.INVOICEEUSE.Value);
            _excludedPaymentMethod.Add(PaymentPlanType.PAYMENTPLANSE.Value);
            _excludedPaymentMethod.Add(PaymentPlanType.PAYMENTPLANEUSE.Value);
        }

        private void SetPaymentMethodDe()
        {
            _excludedPaymentMethod.Add(InvoiceType.INVOICEDE.Value);
            _excludedPaymentMethod.Add(PaymentPlanType.PAYMENTPLANDE.Value);
        }

        private void SetPaymentMethodDk()
        {
            _excludedPaymentMethod.Add(InvoiceType.INVOICEDK.Value);
            _excludedPaymentMethod.Add(PaymentPlanType.PAYMENTPLANDK.Value);
        }

        private void SetPaymentMethodFi()
        {
            _excludedPaymentMethod.Add(InvoiceType.INVOICEFI.Value);
            _excludedPaymentMethod.Add(PaymentPlanType.PAYMENTPLANFI.Value);
        }

        private void SetPaymentMethodNl()
        {
            _excludedPaymentMethod.Add(InvoiceType.INVOICENL.Value);
            _excludedPaymentMethod.Add(PaymentPlanType.PAYMENTPLANNL.Value);
        }

        private void SetPaymentMethodNo()
        {
            _excludedPaymentMethod.Add(InvoiceType.INVOICENO.Value);
            _excludedPaymentMethod.Add(PaymentPlanType.PAYMENTPLANNO.Value);
        }
    }
}
