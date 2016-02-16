using System.Runtime.InteropServices;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;
using InvoiceDistributionType = Webpay.Integration.CSharp.AdminWS.InvoiceDistributionType;
using OrderType = Webpay.Integration.CSharp.AdminWS.OrderType;

namespace Webpay.Integration.CSharp.AdminService
{
    public class DeliverOrderRowsRequest
    {
        private DeliverOrderRowsBuilder _builder;

        public DeliverOrderRowsRequest(DeliverOrderRowsBuilder builder)
        {
            _builder = builder;
        }

        private AdminWS.InvoiceDistributionType ConvertDistributionTypeToInvoiceDistributionType(DistributionType dt)
        {
            switch (dt)
            {
                #pragma warning disable 0162 //CS0162 Unreachable code detected
                case DistributionType.NONE:
                    throw new SveaWebPayException("Invalid DistributionType");
                    break;
                case DistributionType.EMAIL:
                    return AdminWS.InvoiceDistributionType.Email;
                    break;
                case DistributionType.POST:
                    return AdminWS.InvoiceDistributionType.Post;
                    break;
                default:
                    throw new SveaWebPayException("Invalid DistributionType");
                #pragma warning restore 0162
            }
        }

        private AdminWS.OrderType ConvertPaymentTypeToOrderType(Util.Constant.PaymentType pt)
        {
            switch (pt)
            {
                #pragma warning disable 0162 //CS0162 Unreachable code detected
                case PaymentType.INVOICE:
                    return AdminWS.OrderType.Invoice;
                    break;
                case PaymentType.PAYMENTPLAN:
                    return AdminWS.OrderType.PaymentPlan;
                    break;
                default:
                    throw new SveaWebPayException("Invalid PaymentType");
                #pragma warning restore 0162
            }
        }

        public Webpay.Integration.CSharp.AdminWS.DeliveryResponse DoRequest()
        {
            var auth = new AdminWS.Authentication()
            {
                Password = _builder.GetConfig().GetPassword(_builder.OrderType,_builder.GetCountryCode()),
                Username = _builder.GetConfig().GetUsername(_builder.OrderType,_builder.GetCountryCode())                
            };

            var orderToDeliver = new AdminWS.DeliverOrderInformation()
            {
                ClientId = _builder.GetConfig().GetClientNumber(_builder.OrderType, _builder.GetCountryCode()),
                SveaOrderId = _builder.GetOrderId(),
                OrderType = ConvertPaymentTypeToOrderType(_builder.OrderType)
                //PrintType // optional for EU-clients
            };

            var request = new AdminWS.PartialDeliveryRequest()
            {
                Authentication = auth,
                OrderToDeliver = orderToDeliver,
                RowNumbers = _builder._rowIndexesToDeliver.ToArray(),
                InvoiceDistributionType = ConvertDistributionTypeToInvoiceDistributionType(_builder._distributionType)
            };

            // make request to correct endpoint, return response object
            var endpoint = _builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.DeliverPartial(request);

            return response;
        }
    }
}