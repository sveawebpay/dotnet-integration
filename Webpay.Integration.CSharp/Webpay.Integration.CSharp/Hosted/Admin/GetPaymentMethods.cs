namespace Webpay.Integration.CSharp.Hosted.Admin
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