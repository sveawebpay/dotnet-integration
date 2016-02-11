using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order
{
    public abstract class Builder<T>
    {
        protected IConfigurationProvider Config;
        protected CountryCode CountryCode;

        public IConfigurationProvider GetConfig()
        {
            return Config;
        }

        public CountryCode GetCountryCode()
        {
            return CountryCode;
        }

        public abstract T SetCountryCode(CountryCode countryCode);
    }
}