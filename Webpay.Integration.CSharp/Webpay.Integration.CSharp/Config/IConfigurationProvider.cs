using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Config
{
    /// <summary>
    /// Create a class (eg. one for testing values, one for production) that implements the ConfigurationProvider Interface.
    /// Let the implemented functions return the authorization values asked for.
    /// The integration package will then call these functions to get the value from your database.
    /// Later when starting a WebPay action in your integration file, put an instance of your class as parameter to the constructor.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>user name string</returns>
        string GetUsername(PaymentType type, CountryCode country);

        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>password string</returns>
        string GetPassword(PaymentType type, CountryCode country);

        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>client number int</returns>
        int GetClientNumber(PaymentType type, CountryCode country);

        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>merchant id string</returns>
        string GetMerchantId(PaymentType type, CountryCode country);

        /// <summary>
        /// Get the return value from your database or likewise
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <param name="country">country code</param>
        /// <returns>secret word string</returns>
        string GetSecretWord(PaymentType type, CountryCode country);

        /// <summary>
        /// Constants for the end point url found in the class SveaConfig
        /// </summary>
        /// <param name="type"> eg. HOSTED, INVOICE or PAYMENTPLAN</param>
        /// <returns>end point url</returns>
        string GetEndPoint(PaymentType type);

    }
}