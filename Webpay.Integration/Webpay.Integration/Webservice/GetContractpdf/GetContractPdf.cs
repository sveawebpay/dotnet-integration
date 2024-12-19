using System.ServiceModel;
using System.ServiceModel.Channels;
using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Util.Constant;
using WebpayWS;

namespace Webpay.Integration.Webservice.GetContractPdf;

public class GetContractPdf
{
    protected ServiceSoapClient _soapsc;
    private CountryCode _countryCode;
    private long _contractNumber;
    private readonly IConfigurationProvider _config;

    public GetContractPdf(IConfigurationProvider config)
    {
        _config = config;
    }

    /// <summary>
    /// Required: Set the Country Code
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns>GetContractPdf</returns>
    public GetContractPdf SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    /// <summary>
    /// Required: Set the Contract Number
    /// </summary>
    /// <param name="contractNumber"></param>
    /// <returns>GetContractPdf</returns>
    public GetContractPdf SetContractNumber(long contractNumber)
    {
        _contractNumber = contractNumber;
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
        var errors = string.Empty;

        if (_countryCode == CountryCode.NONE)
        {
            errors += "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";
        }

        if (_contractNumber <= 0)
        {
            errors += "MISSING VALUE - ContractNumber is required, use SetContractNumber(...).\n";
        }

        return errors;
    }

    /// <summary>
    /// PrepareRequest
    /// </summary>
    /// <exception cref="SveaWebPayValidationException"></exception>
    /// <returns>GetContractPdfEuRequest</returns>
    public GetContractPdfEuRequest PrepareRequest()
    {
        var errors = ValidateRequest();
        if (errors.Length > 0)
        {
            throw new SveaWebPayValidationException(errors, null);
        }

        var request = new GetContractPdfEuRequest
        {
            Auth = GetStoreAuthorization(),
            ContractNumber = _contractNumber
        };

        return request;
    }

    /// <summary>
    /// DoRequestAsync - Asynchronous version
    /// </summary>
    /// <returns>Task<GetContractPdfEuResponse></returns>
    public async Task<GetContractPdfEuResponse> DoRequestAsync()
    {
        var request = PrepareRequest();
        _soapsc = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, _config.GetEndPoint(PaymentType.PAYMENTPLAN));

        using (new OperationContextScope(_soapsc.InnerChannel))
        {
            var httpRequestMessage = new HttpRequestMessageProperty();
            httpRequestMessage.Headers["X-Svea-Integration-Platform"] = IntegrationConstants.IntegrationPlatform;
            httpRequestMessage.Headers["X-Svea-Integration-Version"] = IntegrationConstants.IntegrationPlatformVersion;
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestMessage;

            return await _soapsc.GetContractPdfEuAsync(request);
        }
    }
}
