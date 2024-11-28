using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Util.Constant;
using WebpayWS;

namespace Webpay.Integration.Webservice.Getpaymentplanparams;

public class GetPaymentPlanParams
{
    protected ServiceSoapClient Soapsc;
    private CountryCode _countryCode;
    private readonly IConfigurationProvider _config;

    public GetPaymentPlanParams(IConfigurationProvider config)
    {
        _config = config;
    }

    /// <summary>
    /// Required
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns>GetPaymentPlanParams</returns>
    public GetPaymentPlanParams SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    protected ClientAuthInfo GetStoreAuthorization()
    {
        var auth = new ClientAuthInfo
        {
            ClientNumber = _config.GetClientNumber(PaymentType.PAYMENTPLAN, _countryCode),
            Password = _config.GetPassword(PaymentType.PAYMENTPLAN, _countryCode),
            Username = _config.GetUsername(PaymentType.PAYMENTPLAN, _countryCode)
        };

        return auth;
    }

    public string ValidateRequest()
    {
        return _countryCode == CountryCode.NONE ? "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n" : "";
    }

    /// <summary>
    /// PrepareRequest
    /// </summary>
    /// <exception cref="SveaWebPayValidationException"></exception>
    /// <returns>GetPaymentPlanParamsEuRequest</returns>
    public GetPaymentPlanParamsEuRequest PrepareRequest()
    {
        var errors = ValidateRequest();
        if (errors.Length > 0)
        {
            throw new SveaWebPayValidationException(errors, null);
        }

        var request = new GetPaymentPlanParamsEuRequest
        {
            Auth = GetStoreAuthorization()
        };

        return request;
    }

    /// <summary>
    /// DoRequestAsync - Asynchronous version
    /// </summary>
    /// <returns>Task<GetPaymentPlanParamsEuResponse></returns>
    public async Task<GetPaymentPlanParamsEuResponse> DoRequestAsync()
    {
        var request = PrepareRequest();
        Soapsc = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, _config.GetEndPoint(PaymentType.PAYMENTPLAN));

        return await Soapsc.GetPaymentPlanParamsEuAsync(request);
    }
}
