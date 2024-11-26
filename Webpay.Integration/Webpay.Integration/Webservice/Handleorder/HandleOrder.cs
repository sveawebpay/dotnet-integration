using System.ServiceModel;
using Webpay.Integration.Exception;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Order.Validator;
using Webpay.Integration.Util.Constant;
using WebpayWS;
using Webpay.Integration.Webservice.Helper;
using InvoiceDistributionType = WebpayWS.InvoiceDistributionType;
using OrderType = Webpay.Integration.Util.Constant.OrderType;

namespace Webpay.Integration.Webservice.Handleorder;

public class HandleOrder
{
    private ServiceSoapClient _soapsc;
    private readonly DeliverOrderBuilder _order;
    private DeliverOrderEuRequest _sveaDeliverOrder;
    private DeliverOrderInformation _orderInformation;

    public HandleOrder(DeliverOrderBuilder orderBuilder)
    {
        _order = orderBuilder;
    }

    private ClientAuthInfo GetStoreAuthorization()
    {
        var type = (_order.GetOrderType() == OrderType.INVOICE)
            ? PaymentType.INVOICE
            : PaymentType.PAYMENTPLAN;

        var auth = new ClientAuthInfo
        {
            Username = _order.GetConfig().GetUsername(type, _order.GetCountryCode()),
            Password = _order.GetConfig().GetPassword(type, _order.GetCountryCode()),
            ClientNumber = _order.GetConfig().GetClientNumber(type, _order.GetCountryCode())
        };
        return auth;
    }

    public string ValidateOrder()
    {
        return _order == null ? "NullReference in validation of HandleOrder" : HandleOrderValidator.Validate(_order);
    }

    public DeliverOrderEuRequest PrepareRequest()
    {
        return PrepareRequestInternal(true);
    }

    private DeliverOrderEuRequest PrepareRequestInternal(bool useIncVatRequestIfPossible)
    {
        var errors = ValidateOrder();
        if (errors.Length > 0)
        {
            throw new SveaWebPayValidationException(errors);
        }

        var formatter = new WebServiceRowFormatter<DeliverOrderBuilder>(_order, useIncVatRequestIfPossible);

        DeliverInvoiceDetails deliverInvoiceDetails = null;
        if (_order.GetOrderType() == OrderType.INVOICE)
        {
            deliverInvoiceDetails = new DeliverInvoiceDetails
            {
                InvoiceDistributionType = ConvertInvoiceDistributionType(_order.GetInvoiceDistributionType()),
                InvoiceIdToCredit = _order.GetCreditInvoice(),
                IsCreditInvoice = _order.GetCreditInvoice().HasValue,
                NumberOfCreditDays = _order.GetNumberOfCreditDays(),
                OrderRows = formatter.FormatRows().ToArray()
            };
        }

        _orderInformation = new DeliverOrderInformation
        {
            DeliverInvoiceDetails = deliverInvoiceDetails,
            OrderType = ConvertOrderType(_order.GetOrderType()),
            SveaOrderId = _order.GetOrderId()
        };

        _sveaDeliverOrder = new DeliverOrderEuRequest
        {
            Auth = GetStoreAuthorization(),
            DeliverOrderInformation = _orderInformation
        };

        return _sveaDeliverOrder;
    }

    private static InvoiceDistributionType ConvertInvoiceDistributionType(DistributionType getDistributionType)
    {
        return getDistributionType switch
        {
            DistributionType.POST => InvoiceDistributionType.Post,
            DistributionType.EMAIL => InvoiceDistributionType.Email,
            DistributionType.EINVOICEB2B => InvoiceDistributionType.EInvoiceB2B,
            _ => InvoiceDistributionType.Post
        };
    }

    private static WebpayWS.OrderType ConvertOrderType(OrderType orderType)
    {
        return orderType == OrderType.INVOICE
            ? WebpayWS.OrderType.Invoice
            : WebpayWS.OrderType.PaymentPlan;
    }

    public async Task<DeliverOrderEuResponse> DoRequest()
    {
        var request = PrepareRequestInternal(true);
        var response = await DoRequestInternalAsync(request);

        if (response.ResultCode == 50036)
        {
            request = PrepareRequestInternal(false);
            response = await DoRequestInternalAsync(request);
        }

        return response;
    }

    private async Task<DeliverOrderEuResponse> DoRequestInternalAsync(DeliverOrderEuRequest request)
    {
        var endpointAddress = new EndpointAddress(
            _order.GetConfig().GetEndPoint(_order.GetOrderType() == OrderType.INVOICE
                ? PaymentType.INVOICE
                : PaymentType.PAYMENTPLAN));

        _soapsc = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, endpointAddress);

        return await _soapsc.DeliverOrderEuAsync(request);
    }
}
