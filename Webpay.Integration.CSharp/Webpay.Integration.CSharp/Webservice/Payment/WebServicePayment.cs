using System.Collections.Generic;
using System.ServiceModel;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Validator;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Helper;
using System.Linq;
using System.Linq.Expressions;

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
            return CrOrderBuilder == null
                       ? "NullReference in validaton of WebServiceOrderValidator"
                       : new WebServiceOrderValidator().Validate(CrOrderBuilder);
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
            {
                throw new SveaWebPayValidationException(errors);
            }

            var sveaOrder = new CreateOrderEuRequest {Auth = GetPasswordBasedAuthorization()};

            var allPricesAreSpecifiedIncVat = CrOrderBuilder.GetOrderRows().All(orderRow => orderRow.GetAmountIncVat() != null);

            var formatter = new WebServiceRowFormatter<CreateOrderBuilder>(CrOrderBuilder);
            List<OrderRow> formattedOrderRows = formatter.FormatRows();
            formattedOrderRows.ForEach(orderRow => orderRow.PriceIncludingVat = allPricesAreSpecifiedIncVat);

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
                    OrderDate = CrOrderBuilder.GetOrderDate(),
                    CustomerReference = CrOrderBuilder.GetCustomerReference(),
                    OrderRows = formattedOrderRows.ToArray(),
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

            var createOrderEuResponse = Soapsc.CreateOrderEu(request);
            return createOrderEuResponse;
        }

        protected abstract CreateOrderInformation SetOrderType(CreateOrderInformation information);
    }
}