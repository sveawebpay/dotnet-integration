using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;
using InvoiceDistributionType = Webpay.Integration.CSharp.Util.Constant.InvoiceDistributionType;

namespace Webpay.Integration.CSharp.Order.Validator
{
    public class HandleOrderValidator
    {
        public static string Validate(DeliverOrderBuilder order)
        {
            var stringBuilder = new StringBuilder();

            var list = new List<string>
                {
                    ValidateCountry(order),
                    ValidateOrderType(order),
                    ValidateOrderId(order),
                    ValidateInvoiceDetails(order),
                    ValidateOrderRows(order),
                };

            foreach (var line in list.Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                stringBuilder.AppendLine(line);
            }

            return stringBuilder.ToString().Trim();
        }

        private static string ValidateCountry(DeliverOrderBuilder order)
        {
            return order.GetCountryCode() == CountryCode.NONE 
                ? "MISSING VALUE - CountryCode is required, use SetCountryCode(...)." : "";
        }

        private static string ValidateOrderType(DeliverOrderBuilder order)
        {
            return order.GetOrderType() == OrderType.NONE ? "MISSING VALUE - OrderType is missing for DeliverOrder, use SetOrderType()." : "";
        }

        private static string ValidateOrderId(DeliverOrderBuilder order)
        {
            return order.GetOrderId() <= 0 ? "MISSING VALUE - SetOrderId is required." : "";
        }

        private static string ValidateInvoiceDetails(DeliverOrderBuilder order)
        {
            if (order.GetOrderId() > 0 && order.GetOrderType() == OrderType.INVOICE &&
                    order.GetInvoiceDistributionType() == InvoiceDistributionType.NONE)
                return "MISSING VALUE - SetInvoiceDistributionType is requred for DeliverInvoiceOrder.";
            return "";
        }

        private static string ValidateOrderRows(DeliverOrderBuilder order)
        {
            var rowCount = order.GetOrderRows().Count + order.GetShippingFeeRows().Count + order.GetInvoiceFeeRows().Count;
            if (order.GetOrderType() == OrderType.INVOICE && rowCount == 0)
                return"MISSING VALUE - No order or fee has been included. Use AddOrder(...) or AddFee(...).";
            return "";
        }
    }
}
