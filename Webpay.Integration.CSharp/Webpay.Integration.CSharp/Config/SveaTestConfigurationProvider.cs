using System.Collections;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Config
{
    public class SveaTestConfigurationProvider : IConfigurationProvider
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
                        return "norgetest2";
                    case CountryCode.FI:
                        return "finlandtest2";
                    case CountryCode.DK:
                        return "danmarktest2";
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
                        return "norgetest2";
                    case CountryCode.FI:
                        return "finlandtest2";
                    case CountryCode.DK:
                        return "danmarktest2";
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
                    {
                        return 79021;
                    }
                    if (type == PaymentType.PAYMENTPLAN)
                    {
                        return 59999;
                    }
                    break;
                case CountryCode.NO:
                    if (type == PaymentType.INVOICE)
                    {
                        return 33308;
                    }
                    if (type == PaymentType.PAYMENTPLAN)
                    {
                        return 32503;
                    }
                    break;
                case CountryCode.FI:
                    if (type == PaymentType.INVOICE)
                    {
                        return 26136;
                    }
                    if (type == PaymentType.PAYMENTPLAN)
                    {
                        return 27136;
                    }
                    break;
                case CountryCode.DK:
                    if (type == PaymentType.INVOICE)
                    {
                        return 62008;
                    }
                    if (type == PaymentType.PAYMENTPLAN)
                    {
                        return 64008;
                    }
                    break;
                case CountryCode.NL:
                    if (type == PaymentType.INVOICE)
                    {
                        return 85997;
                    }
                    if (type == PaymentType.PAYMENTPLAN)
                    {
                        return 86997;
                    }
                    break;
                case CountryCode.DE:
                    if (type == PaymentType.INVOICE)
                    {
                        return 14997;
                    }
                    if (type == PaymentType.PAYMENTPLAN)
                    {
                        return 16997;
                    }
                    break;
            }
            return 0;
        }
        //1110
        //? (country == CountryCode.NO? "9d8d83fe18ed0fe3cf6de8bfd29d82bdaf228f3f8b292b087ec48736b083ce4699c3493a406f02f1300e24490e0c7d0d6d55bfc38dd8e9c1390ac17ce56bdb1a" : "1f8bcd8a564073f7156efd2522d5998f5487a1dcd19e1e120276fb1fb7e233a6059c45d6eb44a8d7342a4989bbb95acd4708051bbc145bda43ae0dd3503928db")
                       
        public string GetMerchantId(PaymentType type, CountryCode country)
        {
            return type == PaymentType.HOSTED ? (country == CountryCode.NO ? "1109" : "1110") : "";
        }

        public string GetSecretWord(PaymentType type, CountryCode country)
        {
            return type == PaymentType.HOSTED
            //? (country == CountryCode.NO ? "9d8d83fe18ed0fe3cf6de8bfd29d82bdaf228f3f8b292b087ec48736b083ce4699c3493a406f02f1300e24490e0c7d0d6d55bfc38dd8e9c1390ac17ce56bdb1a" : "8a9cece566e808da63c6f07ff415ff9e127909d000d259aba24daa2fed6d9e3f8b0b62e8ad1fa91c7d7cd6fc3352deaae66cdb533123edf127ad7d1f4c77e7a3")
            ? (country == CountryCode.NO ? "9d8d83fe18ed0fe3cf6de8bfd29d82bdaf228f3f8b292b087ec48736b083ce4699c3493a406f02f1300e24490e0c7d0d6d55bfc38dd8e9c1390ac17ce56bdb1a" : "1f8bcd8a564073f7156efd2522d5998f5487a1dcd19e1e120276fb1fb7e233a6059c45d6eb44a8d7342a4989bbb95acd4708051bbc145bda43ae0dd3503928db")

            : "";
        }

        public string GetEndPoint(PaymentType type)
        {
            switch (type)
            {
                case PaymentType.HOSTED:
                    return SveaConfig.GetTestPayPageUrl();

                case PaymentType.INVOICE:
                    return SveaConfig.GetTestWebserviceUrl();

                case PaymentType.PAYMENTPLAN:
                    return SveaConfig.GetTestWebserviceUrl();

                case PaymentType.ADMIN_TYPE:
                    return SveaConfig.GetTestAdminServiceUrl();

                default:
                    throw new SveaWebPayException("Unknown PaymentType");
            }
        }

    }
}