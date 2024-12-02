using System.ServiceModel;
using System.ServiceModel.Channels;
using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Util.Constant;
using WebpayWS;

namespace Webpay.Integration.Webservice.Getaddresses;

public class GetAddresses
{
    protected ServiceSoapClient _soapsc;
    private string _nationalNumber;
    private string _companyId;
    private CountryCode _countryCode;
    private string _orderType;
    private readonly IConfigurationProvider _config;
    private string _zipCode;

    public GetAddresses(IConfigurationProvider config)
    {
        _config = config;
    }

    public string GetIndividual() => _nationalNumber;

    public GetAddresses SetIndividual(string nationalNumber)
    {
        _nationalNumber = nationalNumber;
        return this;
    }

    public string GetCompanyId() => _companyId;

    public GetAddresses SetCompany(string companyId)
    {
        _companyId = companyId;
        return this;
    }

    public CountryCode GetCountryCode() => _countryCode;

    public GetAddresses SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public string GetZipCode() => _zipCode;

    public GetAddresses SetZipCode(string zipCode)
    {
        _zipCode = zipCode;
        return this;
    }

    public GetAddresses SetOrderTypePaymentPlan()
    {
        _orderType = "PaymentPlan";
        return this;
    }

    public GetAddresses SetOrderTypeInvoice()
    {
        _orderType = "Invoice";
        return this;
    }

    public string GetOrderType() => _orderType;

    private ClientAuthInfo GetStoreAuthorization()
    {
        var type = _orderType == "Invoice" ? PaymentType.INVOICE : PaymentType.PAYMENTPLAN;
        return new ClientAuthInfo
        {
            ClientNumber = _config.GetClientNumber(type, _countryCode),
            Password = _config.GetPassword(type, _countryCode),
            Username = _config.GetUsername(type, _countryCode)
        };
    }

    public string ValidateRequest()
    {
        var errors = "";
        if (_countryCode == CountryCode.NONE)
        {
            errors += "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";
        }
        if (_nationalNumber == null && _companyId == null)
        {
            errors += "MISSING VALUE - either nationalNumber or companyId is required. Use: SetCompany(...) or SetIndividual(...).\n";
        }
        return errors;
    }

    public GetCustomerAddressesRequest PrepareRequest()
    {
        var errors = ValidateRequest();
        if (errors.Length > 0)
        {
            throw new SveaWebPayValidationException(errors);
        }

        return new GetCustomerAddressesRequest
        {
            Auth = GetStoreAuthorization(),
            CountryCode = _countryCode.ToString().ToUpper(),
            IsCompany = _companyId != null,
            SecurityNumber = _companyId ?? _nationalNumber
        };
    }

    public async Task<GetCustomerAddressesResponse> DoRequestAsync()
    {
        var request = PrepareRequest();

        _soapsc = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap,
                                       _config.GetEndPoint(_orderType == "Invoice" ? PaymentType.INVOICE : PaymentType.PAYMENTPLAN));

        using (new OperationContextScope(_soapsc.InnerChannel))
        {
            var httpRequestMessage = new HttpRequestMessageProperty();
            httpRequestMessage.Headers["X-Svea-Integration-Platform"] = IntegrationConstants.IntegrationPlatform;
            httpRequestMessage.Headers["X-Svea-Integration-Version"] = IntegrationConstants.IntegrationPlatformVersion;
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestMessage;

            return await _soapsc.GetAddressesAsync(request);
        }
    }
}

/*Awaiting approval
private GetAddressesEuRequest PrepareRequest()
{
string errors = ValidateRequest();
if (errors.Length > 0)
{
    throw new SveaWebPayValidationException(errors);
}

var request = new GetAddressesEuRequest
    {
        Auth = GetStoreAuthorization(),
        GetAddressesInformation = new GetAddressesInformation
            {
                CountryCode = _countryCode.ToString().ToUpper(),
                CustomerType = _companyId == null ? CustomerType.Individual : CustomerType.Company,
                NationalIdNumber = _nationalNumber,
                ZipCode = _zipCode
            }
    };
return request;
}

private GetAddressesEuResponse DoRequestAsync()
{
GetAddressesEuRequest request = PrepareRequest();

Soapsc = new ServiceSoapClient(new BasicHttpBinding
    {
        Name = "ServiceSoap",
        Security = new BasicHttpSecurity
            {
                Mode = BasicHttpSecurityMode.Transport
            }
    },
                               new EndpointAddress(
                                   _config.GetEndPoint(_orderType == "Invoice"
                                                           ? PaymentType.INVOICE
                                                           : PaymentType.PAYMENTPLAN)));

return Soapsc.GetAddressesEu(request);
} */
