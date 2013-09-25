using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Config
{
    class SveaTestConfigurationProvider : IConfigurationProvider
    {
        public string GetUsername(PaymentType type, CountryCode country)
        {
            if (type == PaymentType.INVOICE || type == PaymentType.PAYMENTPLAN)
            {
                switch (country)
                {
                    case CountryCode.SE:
                        return "sverigetest";
                    case CountryCode.NO:
                        return "webpay_test_no";
                    case CountryCode.FI:
                        return "finlandtest";
                    case CountryCode.DK:
                        return "danmarktest";
                    case CountryCode.NL:
                        return "hollandtest";
                    case CountryCode.DE:
                        return "germanytest";
                }
            }
            return "";
        }

        public string GetPassword(PaymentType type, CountryCode country)
        {
            if (type == PaymentType.INVOICE || type == PaymentType.PAYMENTPLAN)
            {
                switch (country)
                {
                    case CountryCode.SE:
                        return "sverigetest";
                    case CountryCode.NO:
                        return "dvn349hvs9+29hvs";
                    case CountryCode.FI:
                        return "finlandtest";
                    case CountryCode.DK:
                        return "danmarktest";
                    case CountryCode.NL:
                        return "hollandtest";
                    case CountryCode.DE:
                        return "germanytest";
                }
            }
            return "";
        }

        public int GetClientNumber(PaymentType type, CountryCode country)
        {
            switch (country)
            {
                case CountryCode.SE:
                    if (type == PaymentType.INVOICE)
                        return 79021;
                    if (type == PaymentType.PAYMENTPLAN)
                        return 59999;
                    break;
                case CountryCode.NO:
                    if (type == PaymentType.INVOICE)
                        return 32666;
                    if (type == PaymentType.PAYMENTPLAN)
                        return 36000;
                    break;
                case CountryCode.FI:
                    if (type == PaymentType.INVOICE)
                        return 29995;
                    if (type == PaymentType.PAYMENTPLAN)
                        return 29992;
                    break;
                case CountryCode.DK:
                    if (type == PaymentType.INVOICE)
                        return 60006;
                    if (type == PaymentType.PAYMENTPLAN)
                        return 60004;
                    break;
                case CountryCode.NL:
                    if (type == PaymentType.INVOICE)
                        return 85997;
                    if (type == PaymentType.PAYMENTPLAN)
                        return 86997;
                    break;
                case CountryCode.DE:
                    if (type == PaymentType.INVOICE)
                        return 14997;
                    if (type == PaymentType.PAYMENTPLAN)
                        return 16997;
                    break;
            }
            return 0;
        }

        public string GetMerchantId(PaymentType type, CountryCode country)
        {
            return PaymentType.HOSTED == type ? "1130" : "";
        }

        public string GetSecret(PaymentType type, CountryCode country)
        {
            return PaymentType.HOSTED == type ? "8a9cece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3" : "";
        }

        public string GetEndPoint(PaymentType type)
        {
            return PaymentType.HOSTED == type ? SveaConfig.GetTestPayPageUrl() : SveaConfig.GetTestWebserviceUrl();
        }
    }
}
