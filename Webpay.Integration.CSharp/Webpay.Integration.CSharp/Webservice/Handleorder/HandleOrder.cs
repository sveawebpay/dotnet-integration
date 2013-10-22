using System.ServiceModel;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Validator;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Helper;
using InvoiceDistributionType = Webpay.Integration.CSharp.WebpayWS.InvoiceDistributionType;
using OrderType = Webpay.Integration.CSharp.Util.Constant.OrderType;

namespace Webpay.Integration.CSharp.Webservice.Handleorder
{
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

        /// <summary>
        /// ValidateOrder
        /// </summary>
        /// <returns>Error message compilation string</returns>
        public string ValidateOrder()
        {
            return _order == null ? "NullReference in validaton of HandleOrder" : HandleOrderValidator.Validate(_order);
        }

        /// <summary>
        /// PrepareRequest
        /// </summary>
        /// <exception cref="SveaWebPayValidationException"></exception>
        /// <returns>SveaRequest</returns>
        public DeliverOrderEuRequest PrepareRequest()
        {
            var errors = ValidateOrder();
            if (errors.Length > 0)
            {
                throw new SveaWebPayValidationException(errors);
            }

            var formatter = new WebServiceRowFormatter<DeliverOrderBuilder>(_order);

            DeliverInvoiceDetails deliverInvoiceDetails = null;
            if (_order.GetOrderType() == OrderType.INVOICE)
            {
                deliverInvoiceDetails = new DeliverInvoiceDetails
                    {
                        InvoiceDistributionType = ConvertInvoiceDistributionType(_order.GetInvoiceDistributionType()),
                        InvoiceIdToCredit = _order.GetCreditInvoice(),
                        IsCreditInvoice = _order.GetCreditInvoice().HasValue,
                        NumberOfCreditDays = _order.GetNumberOfCreditDays(),
                        OrderRows = formatter.FormatRows()
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

        private static InvoiceDistributionType ConvertInvoiceDistributionType(
            Util.Constant.InvoiceDistributionType getInvoiceDistributionType)
        {
            return getInvoiceDistributionType == Util.Constant.InvoiceDistributionType.EMAIL
                       ? InvoiceDistributionType.Email
                       : InvoiceDistributionType.Post;
        }

        private static WebpayWS.OrderType ConvertOrderType(OrderType orderType)
        {
            return orderType == OrderType.INVOICE
                       ? WebpayWS.OrderType.Invoice
                       : WebpayWS.OrderType.PaymentPlan;
        }

        /// <summary>
        /// DoRequest
        /// </summary>
        /// <returns>DeliverOrderResponse</returns>
        public DeliverOrderEuResponse DoRequest()
        {
            var request = PrepareRequest();

            _soapsc = new ServiceSoapClient(new BasicHttpBinding
                {
                    Name = "ServiceSoap",
                    Security = new BasicHttpSecurity
                        {
                            Mode = BasicHttpSecurityMode.Transport
                        }
                },
                                            new EndpointAddress(
                                                _order.GetConfig()
                                                      .GetEndPoint(_order.GetOrderType() == OrderType.INVOICE
                                                                       ? PaymentType.INVOICE
                                                                       : PaymentType.PAYMENTPLAN)))
                ;

            return _soapsc.DeliverOrderEu(request);
        }
    }
}