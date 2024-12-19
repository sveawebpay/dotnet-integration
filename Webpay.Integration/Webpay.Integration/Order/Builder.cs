using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order;

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