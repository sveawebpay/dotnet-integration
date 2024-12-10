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

public static class PaymentTypeExtensions
{
    public static PaymentType FromString(string orderType)
    {
        if (string.IsNullOrWhiteSpace(orderType))
        {
            return PaymentType.NONE;
        }

        return orderType.Trim().ToLowerInvariant() switch
        {
            "invoice" => PaymentType.INVOICE,
            "paymentplan" => PaymentType.PAYMENTPLAN,
            "accountcredit" => PaymentType.ACCOUNTCREDIT,
            "hosted" => PaymentType.HOSTED,
            "admin_type" => PaymentType.ADMIN_TYPE,
            _ => PaymentType.NONE
        };
    }

    public static OrderType ToOrderType(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.INVOICE => OrderType.INVOICE,
            PaymentType.PAYMENTPLAN => OrderType.PAYMENTPLAN,
            PaymentType.ACCOUNTCREDIT => OrderType.ACCOUNTCREDIT,
            _ => OrderType.NONE
        };
    }
}
