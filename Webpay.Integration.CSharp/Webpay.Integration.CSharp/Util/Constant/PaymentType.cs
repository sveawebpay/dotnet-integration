namespace Webpay.Integration.CSharp.Util.Constant
{
    public enum PaymentType
    {
        NONE = 0,
        HOSTED = 1,
        INVOICE = 2,
        PAYMENTPLAN = 3,
        ADMIN_TYPE = 99     // used to make AdminServiceRequets pick up correct test/prod AdminWS endpoint from IConfigurationProvider
    }
}