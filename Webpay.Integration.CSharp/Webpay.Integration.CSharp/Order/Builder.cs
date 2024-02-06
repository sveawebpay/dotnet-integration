using System;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order
{
    public abstract class Builder<T>
    {
        protected IConfigurationProvider _config;
        protected CountryCode _countryCode;
        protected Guid? _correlationId;

        protected Builder(IConfigurationProvider config)
        {
            _config = config;
        } 

        public IConfigurationProvider GetConfig()
        {
            return _config;
        }

        public CountryCode GetCountryCode()
        {
            return _countryCode;
        }

        public Guid? GetCorrelationId()
        {
            return _correlationId;
        }
        public abstract T SetCountryCode(CountryCode countryCode);
        public abstract T SetCorrelationId(Guid? correlationId);
    }
}