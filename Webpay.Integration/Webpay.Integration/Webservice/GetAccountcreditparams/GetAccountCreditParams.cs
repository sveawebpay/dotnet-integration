using System.ServiceModel;
using System.ServiceModel.Channels;
using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Util.Constant;
using WebpayWS;

namespace Webpay.Integration.Webservice.GetAccountCreditParams;

public class GetAccountCreditParams
{
    protected ServiceSoapClient _soapsc;
    private CountryCode _countryCode;
    private readonly IConfigurationProvider _config;

    public GetAccountCreditParams(IConfigurationProvider config)
    {
        _config = config;
    }

    /// <summary>
    /// Required
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns>GetAccountCreditParams</returns>
    public GetAccountCreditParams SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    protected ClientAuthInfo GetStoreAuthorization()
    {
        var auth = new ClientAuthInfo
        {
            ClientNumber = _config.GetClientNumber(PaymentType.ACCOUNTCREDIT, _countryCode),
            Password = _config.GetPassword(PaymentType.ACCOUNTCREDIT, _countryCode),
            Username = _config.GetUsername(PaymentType.ACCOUNTCREDIT, _countryCode)
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
    /// <returns>GetAccountCreditParamsEuRequest</returns>
    public GetAccountCreditParamsEuRequest PrepareRequest()
    {
        var errors = ValidateRequest();
        if (errors.Length > 0)
        {
            throw new SveaWebPayValidationException(errors, null);
        }

        var request = new GetAccountCreditParamsEuRequest
        {
            Auth = GetStoreAuthorization()
        };

        return request;
    }

    /// <summary>
    /// DoRequestAsync
    /// </summary>
    /// <returns>Task<GetAccountCreditParamsEuResponse></returns>
    public async Task<GetAccountCreditParamsEuResponse> DoRequestAsync()
    {
        var request = PrepareRequest();
        _soapsc = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, _config.GetEndPoint(PaymentType.ACCOUNTCREDIT));

        using (new OperationContextScope(_soapsc.InnerChannel))
        {
            var httpRequestMessage = new HttpRequestMessageProperty();
            httpRequestMessage.Headers["X-Svea-Integration-Platform"] = IntegrationConstants.IntegrationPlatform;
            httpRequestMessage.Headers["X-Svea-Integration-Version"] = IntegrationConstants.IntegrationPlatformVersion;
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestMessage;

            return await _soapsc.GetAccountCreditParamsEuAsync(request);
        }
    }
}
