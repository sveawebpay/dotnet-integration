using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order
{
    public abstract class Builder<T>
    {
        protected IConfigurationProvider _config;
        protected CountryCode _countryCode;

        public IConfigurationProvider GetConfig()
        {
            return _config;
        }

        public CountryCode GetCountryCode()
        {
            return _countryCode;
        }
        public abstract T SetCountryCode(CountryCode countryCode);
    }
}