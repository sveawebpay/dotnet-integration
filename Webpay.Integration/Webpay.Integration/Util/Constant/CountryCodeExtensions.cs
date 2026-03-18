namespace Webpay.Integration.Util.Constant;

public static class CountryCodeExtensions
{
    public static CountryCode GetCountryCode(this string countryId)
    {
        if (string.IsNullOrWhiteSpace(countryId))
        {
            throw new ArgumentException("CountryId cannot be null or empty.", nameof(countryId));
        }

        if (Enum.TryParse(typeof(CountryCode), countryId, true, out var result))
        {
            if (Enum.IsDefined(typeof(CountryCode), result))
            {
                return (CountryCode)result;
            }
        }

        throw new ArgumentException($"Invalid CountryId: {countryId}", nameof(countryId));
    }
}
