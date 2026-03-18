using System.ServiceModel;
using System.ServiceModel.Channels;
using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Util.Constant;
using WebpayWS;

namespace Webpay.Integration.Webservice.GetInvoiceCreditAgreementPdf;

public class GetInvoiceCreditAgreementPdf
{
    protected ServiceSoapClient _soapsc;
    private readonly IConfigurationProvider _config;

    private CountryCode _countryCode;
    private string _fullName;
    private string _streetAddress;
    private string _postalCode;
    private string _city;
    private System.DateTime _orderCreatedDate;
    private decimal _totalAmount;
    private string _nationalId;

    public GetInvoiceCreditAgreementPdf(IConfigurationProvider config)
    {
        _config = config;
    }

    public GetInvoiceCreditAgreementPdf SetCountryCode(CountryCode countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public GetInvoiceCreditAgreementPdf SetFullName(string fullName)
    {
        _fullName = fullName;
        return this;
    }

    public GetInvoiceCreditAgreementPdf SetStreetAddress(string streetAddress)
    {
        _streetAddress = streetAddress;
        return this;
    }

    public GetInvoiceCreditAgreementPdf SetPostalCode(string postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    public GetInvoiceCreditAgreementPdf SetCity(string city)
    {
        _city = city;
        return this;
    }

    public GetInvoiceCreditAgreementPdf SetOrderCreatedDate(System.DateTime orderCreatedDate)
    {
        _orderCreatedDate = orderCreatedDate;
        return this;
    }

    public GetInvoiceCreditAgreementPdf SetTotalAmount(decimal totalAmount)
    {
        _totalAmount = totalAmount;
        return this;
    }

    public GetInvoiceCreditAgreementPdf SetNationalId(string nationalId)
    {
        _nationalId = nationalId;
        return this;
    }

    private ClientAuthInfo GetStoreAuthorization()
    {
        return new ClientAuthInfo
        {
            ClientNumber = _config.GetClientNumber(PaymentType.INVOICE, _countryCode),
            Username = _config.GetUsername(PaymentType.INVOICE, _countryCode),
            Password = _config.GetPassword(PaymentType.INVOICE, _countryCode)
        };
    }

    public string ValidateRequest()
    {
        var errors = "";

        if (_countryCode == CountryCode.NONE)
            errors += "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";

        return errors;
    }

    public GetInvoiceCreditAgreementPdfRequest PrepareRequest()
    {
        var errors = ValidateRequest();
        if (errors.Length > 0)
            throw new SveaWebPayValidationException(errors);

        return new GetInvoiceCreditAgreementPdfRequest
        {
            Auth = GetStoreAuthorization(),
            CountryCode = _countryCode.ToString().ToUpper(),
            FullName = _fullName,
            StreetAddress = _streetAddress,
            PostalCode = _postalCode,
            City = _city,
            OrderCreatedDate = _orderCreatedDate,
            TotalAmount = _totalAmount,
            NationalId = _nationalId
        };
    }

    public async Task<GetInvoiceCreditAgreementPdfResponse> DoRequestAsync()
    {
        var request = PrepareRequest();

        var myEndpoint = _config.GetEndPoint(PaymentType.INVOICE);
        _soapsc = new ServiceSoapClient(
            ServiceSoapClient.EndpointConfiguration.ServiceSoap,
            _config.GetEndPoint(PaymentType.INVOICE));

        using (new OperationContextScope(_soapsc.InnerChannel))
        {
            var httpRequestMessage = new HttpRequestMessageProperty();
            httpRequestMessage.Headers["X-Svea-Integration-Platform"] = IntegrationConstants.IntegrationPlatform;
            httpRequestMessage.Headers["X-Svea-Integration-Version"] = IntegrationConstants.IntegrationPlatformVersion;
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestMessage;

            return await _soapsc.GetInvoiceCreditAgreementPdfAsync(request);
        }
    }
}
