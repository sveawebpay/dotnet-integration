namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class GetPaymentMethods
    {
        public readonly int MerchantId;

        public GetPaymentMethods(int merchantId)
        {
            MerchantId = merchantId;
        }
    }
}