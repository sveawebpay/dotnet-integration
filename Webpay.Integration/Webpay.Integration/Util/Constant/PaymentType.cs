namespace Webpay.Integration.Util.Constant;

public enum PaymentType
{
    NONE = 0,
    HOSTED = 1,
    INVOICE = 2,
    PAYMENTPLAN = 3,
    ACCOUNTCREDIT = 4,
    ADMIN_TYPE = 99 // Used to make AdminServiceRequets pick up correct test/prod AdminWS endpoint from IConfigurationProvider
}