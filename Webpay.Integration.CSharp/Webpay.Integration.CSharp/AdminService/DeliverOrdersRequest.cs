using System.Collections.Generic;
using Webpay.Integration.CSharp.AdminWS;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.AdminService
{
    public class DeliverOrdersRequest
    {
        internal DeliverOrdersBuilder Builder { get; private set; }

        public DeliverOrdersRequest(DeliverOrdersBuilder builder)
        {
            Builder = builder;
        }

        // TODO refactor, break out methods
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
                Password = Builder.GetConfig().GetPassword(Builder.OrderType, Builder.GetCountryCode()),
                Username = Builder.GetConfig().GetUsername(Builder.OrderType, Builder.GetCountryCode())
            };

            var ordersToDeliver = new List<AdminWS.DeliverOrderInformation>();
            var clientId = Builder.GetConfig().GetClientNumber(Builder.OrderType, Builder.GetCountryCode());
            var orderType = ConvertPaymentTypeToOrderType(Builder.OrderType);
            foreach (var orderId in Builder.OrderIds)
            {
                var orderToDeliver = new AdminWS.DeliverOrderInformation()
                {
                    ClientId = clientId,
                    SveaOrderId = orderId,
                    OrderType = orderType
                    //PrintType // optional for EU-clients
                };
                ordersToDeliver.Add(orderToDeliver);
            }

            var request = new AdminWS.DeliveryRequest()
            {
                Authentication = auth,
                OrdersToDeliver = ordersToDeliver.ToArray(),
                InvoiceDistributionType = ConvertDistributionTypeToInvoiceDistributionType(Builder.DistributionType)
            };

            // make request to correct endpoint, return response object
            var endpoint = Builder.GetConfig().GetEndPoint(PaymentType.ADMIN_TYPE);
            var adminWS = new AdminServiceClient("WcfAdminSoapService", endpoint);
            var response = adminWS.DeliverOrders(request);

            return response;
        }
    }
}