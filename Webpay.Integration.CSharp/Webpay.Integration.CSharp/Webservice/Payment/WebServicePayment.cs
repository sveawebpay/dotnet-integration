using System;
using System.ServiceModel;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Validator;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Helper;

namespace Webpay.Integration.CSharp.Webservice.Payment
{
    public abstract class WebServicePayment
    {
        protected ServiceSoapClient Soapsc;
        protected CreateOrderBuilder CrOrderBuilder;
        protected PaymentType PayType;
        public CreateOrderInformation OrderInfo = new CreateOrderInformation();

        public WebServicePayment(CreateOrderBuilder orderBuilder)
        {
            CrOrderBuilder = orderBuilder;
        }

        private ClientAuthInfo GetPasswordBasedAuthorization()
        {
            var auth = new ClientAuthInfo
                {
                    Username = CrOrderBuilder.GetConfig().GetUsername(PayType, CrOrderBuilder.GetCountryCode()),
                    Password = CrOrderBuilder.GetConfig().GetPassword(PayType, CrOrderBuilder.GetCountryCode()),
                    ClientNumber = CrOrderBuilder.GetConfig().GetClientNumber(PayType, CrOrderBuilder.GetCountryCode())
                };
            return auth;
        }

        /// <summary>
        /// ValidateOrder
        /// </summary>
        /// <returns>Error message compilation string</returns>
        public string ValidateOrder()
        {
            var errors = "";
            try
            {
                var validator = new WebServiceOrderValidator();
                errors = validator.Validate(CrOrderBuilder);
            }
            catch (NullReferenceException ex)
            {
                errors += "NullReference in validaton of WebServiceOrderValidator";
            }

            return errors;
        }

        /// <summary>
        /// PrepareRequest
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>CreateOrderEuRequest</returns>
        public CreateOrderEuRequest PrepareRequest()
        {
            var errors = ValidateOrder();
            if (errors.Length > 0)
                throw new SveaWebPayValidationException(errors);

            var sveaOrder = new CreateOrderEuRequest {Auth = GetPasswordBasedAuthorization()};

            var formatter = new WebServiceRowFormatter<CreateOrderBuilder>(CrOrderBuilder);
            var formattedOrderRows = formatter.FormatRows();

            // make order rows and put in CreateOrderInformation
            OrderInfo = new CreateOrderInformation
                {
                    CustomerIdentity = CrOrderBuilder.GetSoapPurifiedCustomer(),
                    ClientOrderNumber = CrOrderBuilder.GetClientOrderNumber(),
                    CreatePaymentPlanDetails = CrOrderBuilder.GetCampaignCode() != null
                                                   ? new CreatePaymentPlanDetails
                                                       {
                                                           CampaignCode = CrOrderBuilder.GetCampaignCode(),
                                                           SendAutomaticGiroPaymentForm =
                                                               CrOrderBuilder.GetSendAutomaticGiroPaymentForm()
                                                       }
                                                   : null,
                    OrderDate = DateTime.Parse(CrOrderBuilder.GetOrderDate()),
                    CustomerReference = CrOrderBuilder.GetCustomerReference(),
                    OrderRows = formattedOrderRows
                };

            sveaOrder.CreateOrderInformation = SetOrderType(OrderInfo);

            return sveaOrder;
        }

        /// <summary>
        /// DoRequest
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>CreateOrderEuRequest</returns>
        public CreateOrderEuResponse DoRequest()
        {
            CreateOrderEuRequest request = PrepareRequest();

            Soapsc = new ServiceSoapClient(new BasicHttpBinding
                {
                    Name = "ServiceSoap",
                    Security = new BasicHttpSecurity
                        {
                            Mode = BasicHttpSecurityMode.Transport
                        }
                },
                                           new EndpointAddress(
                                               CrOrderBuilder.GetConfig().GetEndPoint(PayType)));

            return Soapsc.CreateOrderEu(request);
        }

        protected abstract CreateOrderInformation SetOrderType(CreateOrderInformation information);
    }
}