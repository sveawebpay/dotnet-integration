using System.ServiceModel;
using Webpay.Integration.Exception;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Order.Validator;
using Webpay.Integration.Util.Constant;
using WebpayWS;
using Webpay.Integration.Webservice.Helper;
using System.ServiceModel.Channels;

namespace Webpay.Integration.Webservice.Payment;

public abstract class WebServicePayment
{
    protected ServiceSoapClient _soapsc;
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

    public string ValidateOrder()
    {
        return CrOrderBuilder == null
            ? "NullReference in validation of WebServiceOrderValidator"
            : new WebServiceOrderValidator().Validate(CrOrderBuilder);
    }

    public CreateOrderEuRequest PrepareRequest()
    {
        var errors = ValidateOrder();
        if (errors.Length > 0)
        {
            throw new SveaWebPayValidationException(errors);
        }

        var sveaOrder = new CreateOrderEuRequest { Auth = GetPasswordBasedAuthorization() };

        var formatter = new WebServiceRowFormatter<CreateOrderBuilder>(CrOrderBuilder);
        var formattedOrderRows = formatter.FormatRows();
        var campaignCode = CrOrderBuilder.GetCampaignCode();

        OrderInfo = new CreateOrderInformation
        {
            CustomerIdentity = CrOrderBuilder.GetSoapPurifiedCustomer(),
            ClientOrderNumber = CrOrderBuilder.GetClientOrderNumber(),
            CreatePaymentPlanDetails = PayType == PaymentType.PAYMENTPLAN && campaignCode != null
            ? new CreatePaymentPlanDetails
            {
                CampaignCode = campaignCode,
                SendAutomaticGiroPaymentForm = CrOrderBuilder.GetSendAutomaticGiroPaymentForm()
            }
            : null,
            CreateAccountCreditDetails = PayType == PaymentType.ACCOUNTCREDIT
            ? new CreateAccountCreditDetails
            {
                CampaignCode = campaignCode,
            }
            : null,
            OrderDate = CrOrderBuilder.GetOrderDate(),
            CustomerReference = CrOrderBuilder.GetCustomerReference(),
            OrderRows = formattedOrderRows.ToArray(),
            PeppolId = CrOrderBuilder.GetPeppolId()
        };

        sveaOrder.CreateOrderInformation = SetOrderType(OrderInfo);
        sveaOrder.Navigation = CrOrderBuilder.GetNavigation();

        return sveaOrder;
    }

    public async Task<CreateOrderEuResponse> DoRequestAsync()
    {
        var request = PrepareRequest();

        var endpointAddress = new EndpointAddress(
            CrOrderBuilder.GetConfig().GetEndPoint(PayType));

        _soapsc = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, endpointAddress);

        using (new OperationContextScope(_soapsc.InnerChannel))
        {
            var httpRequestMessage = new HttpRequestMessageProperty();
            httpRequestMessage.Headers["X-Svea-Integration-Platform"] = IntegrationConstants.IntegrationPlatform;
            httpRequestMessage.Headers["X-Svea-Integration-Version"] = IntegrationConstants.IntegrationPlatformVersion;
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestMessage;

            return await _soapsc.CreateOrderEuAsync(request);
        }
    }

    protected abstract CreateOrderInformation SetOrderType(CreateOrderInformation information);
}
