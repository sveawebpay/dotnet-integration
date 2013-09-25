using System.ServiceModel;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Webservice.Getpaymentplanparams
{
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
        public GetPaymentPlanParams SetCountrycode(CountryCode countryCode)
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
            if (_countryCode == CountryCode.NONE)
            {
                return "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";
            }
            return "";
        }

        /// <summary>
        /// prepareRequest
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>SveaRequest</returns>
        private GetPaymentPlanParamsEuRequest PrepareRequest()
        {
            string errors = ValidateRequest();
            if (errors.Length > 0)
            {
                throw new SveaWebPayValidationException(errors,null);
            }

            var request = new GetPaymentPlanParamsEuRequest
                {
                    Auth = GetStoreAuthorization()
                };

            return request;
        }

        /// <summary>
        /// doRequest
        /// </summary>
        /// <returns>PaymentPlanParamsResponse</returns>
        public GetPaymentPlanParamsEuResponse DoRequest()
        {
            GetPaymentPlanParamsEuRequest request = PrepareRequest();

            Soapsc = new ServiceSoapClient(new BasicHttpBinding
            {
                Name = "ServiceSoap",
                Security = new BasicHttpSecurity
                {
                    Mode = BasicHttpSecurityMode.Transport
                }
            },
                               new EndpointAddress(
                                   _config.GetEndPoint(PaymentType.PAYMENTPLAN)));

            return Soapsc.GetPaymentPlanParamsEu(request);
        }
    }
}