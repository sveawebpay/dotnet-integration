using System.ServiceModel;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Webservice.Getaddresses
{
    /// <summary>
    /// Applicable for SE, NO and DK.
    /// If customer has multiple addresses or just to show the address which
    /// the invoice/product is to be delivered. It returns an GetAddressesResponse object containing all associated addresses for a specific 
    /// SecurityNumber.
    /// Each address gets an "AddressSelector" - has to signify the address. This can
    /// be used when creating order to have the invoice be sent to the specified address. 
    /// </summary>
    public class GetAddresses
    {
        protected ServiceSoapClient Soapsc;
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

        public string GetIndividual()
        {
            return _nationalNumber;
        }

        /// <summary>
        /// Required if customer is Individual
        /// </summary>
        /// <param name="nationalNumber">Sweden: Personnummer, Norway: Personalnumber, Denmark: CPR</param>
        /// <returns>GetAddresses</returns>
        public GetAddresses SetIndividual(string nationalNumber)
        {
            _nationalNumber = nationalNumber;
            return this;
        }

        public string GetCompanyId()
        {
            return _companyId;
        }

        /// <summary>
        /// Required if customer is Company
        /// </summary>
        /// <param name="companyId">Sweden: Organisationsnummer, Norway: Vat number, Denmark: CPR</param>
        /// <returns>GetAddresses</returns>
        public GetAddresses SetCompany(string companyId)
        {
            _companyId = companyId;
            return this;
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <returns>CountryCode</returns>
        public CountryCode GetCountryCode()
        {
            return _countryCode;
        }

        public GetAddresses SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }

        public string GetZipCode()
        {
            return _zipCode;
        }

        public GetAddresses SetZipCode(string zipCode)
        {
            _zipCode = zipCode;
            return this;
        }

        /// <summary>
        /// Required for PaymentPlan type
        /// </summary>
        /// <returns>GetAddresses</returns>
        public GetAddresses SetOrderTypePaymentPlan()
        {
            _orderType = "PaymentPlan";
            return this;
        }

        /// <summary>
        /// Required for Invoice type
        /// </summary>
        /// <returns>GetAddresses</returns>
        public GetAddresses SetOrderTypeInvoice()
        {
            _orderType = "Invoice";
            return this;
        }

        public string GetOrderType()
        {
            return _orderType;
        }

        private ClientAuthInfo GetStoreAuthorization()
        {
            PaymentType type = (_orderType == "Invoice" ? PaymentType.INVOICE : PaymentType.PAYMENTPLAN);
            var auth = new ClientAuthInfo
                {
                    ClientNumber = _config.GetClientNumber(type, _countryCode),
                    Password = _config.GetPassword(type, _countryCode),
                    Username = _config.GetUsername(type, _countryCode)
                };
            return auth;
        }

        public string ValidateRequest()
        {
            string errors = "";
            if (_countryCode == CountryCode.NONE)
            {
                errors += "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";
            }
            if (_orderType == null)
            {
                errors +=
                    "MISSING VALUE - orderType is required, use one of: SetOrderTypePaymentPlan() or SetOrderTypeInvoice().\n";
            }
            if (_nationalNumber == null && _companyId == null)
            {
                errors +=
                    "MISSING VALUE - either nationalNumber or companyId is required. Use: SetCompany(...) or SetIndividual(...).\n";
            }
            return errors;
        }

        /// <summary>
        /// prepareRequest
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>SveaRequest</returns>
        public GetCustomerAddressesRequest PrepareRequest()
        {
            string errors = ValidateRequest();
            if (errors.Length > 0)
            {
                throw new SveaWebPayValidationException(errors);
            }

            var request = new GetCustomerAddressesRequest
            {
                Auth = GetStoreAuthorization(),
                CountryCode = _countryCode.ToString().ToUpper(),
                IsCompany = _companyId != null,
                SecurityNumber = _companyId ?? _nationalNumber
            };
            return request;
        }

        /// <summary>
        /// doRequest
        /// </summary>
        /// <returns>GetCustomerAddressesResponse</returns>
        public GetCustomerAddressesResponse DoRequest()
        {
            GetCustomerAddressesRequest request = PrepareRequest();

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

            return Soapsc.GetAddresses(request);
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

        private GetAddressesEuResponse DoRequest()
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
    }
}