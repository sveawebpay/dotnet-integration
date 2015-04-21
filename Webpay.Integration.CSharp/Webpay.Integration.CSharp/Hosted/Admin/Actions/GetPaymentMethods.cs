namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class GetPaymentMethods
    {
        public int MerchantId { get; private set; }

        public GetPaymentMethods(int merchantId)
        {
            MerchantId = merchantId;
        }
    }
}