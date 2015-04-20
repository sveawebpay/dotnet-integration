namespace Webpay.Integration.CSharp.Hosted.Helper
{
    public class RecurringPayment
    {
        public string Value { get; private set; }

        /// <summary>
        /// Only available for scandinavian acquirers
        /// </summary>
        public static readonly RecurringPayment RECURRING = new RecurringPayment("RECURRING");
        public static readonly RecurringPayment RECURRINGCAPTURE = new RecurringPayment("RECURRINGCAPTURE");
        /// <summary>
        /// Only available for scandinavian acquirers
        /// </summary>
        public static readonly RecurringPayment ONECLICK = new RecurringPayment("ONECLICK");
        public static readonly RecurringPayment ONECLICKCAPTURE = new RecurringPayment("ONECLICKCAPTURE");

        private RecurringPayment(string value)
        {
            Value = value;
        }
    }

}