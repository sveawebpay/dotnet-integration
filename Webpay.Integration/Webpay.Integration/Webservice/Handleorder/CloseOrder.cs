using System.ServiceModel;
using Webpay.Integration.Exception;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;
using WebpayWS;

namespace Webpay.Integration.Webservice.Handleorder;

public class CloseOrder
{
    protected ServiceSoapClient Soapsc;
    private CloseOrderBuilder _order;

    public CloseOrder(CloseOrderBuilder order)
    {
        _order = order;
    }

    protected ClientAuthInfo GetStoreAuthorization()
    {
        var type = (_order.GetOrderType() == "Invoice" ? PaymentType.INVOICE : PaymentType.PAYMENTPLAN);

        var auth = new ClientAuthInfo
        {
            Username = _order.GetConfig().GetUsername(type, _order.GetCountrycode()),
            Password = _order.GetConfig().GetPassword(type, _order.GetCountrycode()),
            ClientNumber = _order.GetConfig().GetClientNumber(type, _order.GetCountrycode())
        };
        return auth;
    }

    public string ValidateRequest()
    {
        var errors = "";

        if (_order.GetCountrycode() == CountryCode.NONE)
        {
            errors += "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";
        }
        return errors;
    }

    public CloseOrderEuRequest PrepareRequest()
    {
        var errors = ValidateRequest();
        if (errors != "")
        {
            throw new SveaWebPayValidationException(errors);
        }

        var orderInfo = new CloseOrderInformation { SveaOrderId = _order.GetOrderId() };

        var sveaCloseOrder = new CloseOrderEuRequest
        {
            CloseOrderInformation = orderInfo,
            Auth = GetStoreAuthorization()
        };

        return sveaCloseOrder;
    }

    public async Task<CloseOrderEuResponse> DoRequest()
    {
        var request = PrepareRequest();

        var endpointAddress = new EndpointAddress(
            _order.GetConfig().GetEndPoint(
                _order.GetOrderType() == "Invoice" ? PaymentType.INVOICE : PaymentType.PAYMENTPLAN));

        Soapsc = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, endpointAddress);

        return await Soapsc.CloseOrderEuAsync(request);
    }
}
