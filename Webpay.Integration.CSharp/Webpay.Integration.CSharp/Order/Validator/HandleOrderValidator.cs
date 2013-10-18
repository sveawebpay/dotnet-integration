using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Util.Constant;

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
            return order.GetCountryCode() == CountryCode.NONE ||
                !Enum.IsDefined(typeof(CountryCode), order.GetCountryCode())
                       ? "MISSING VALUE - CountryCode is required, use SetCountryCode(...)."
                       : "";
        }

        private static string ValidateOrderType(DeliverOrderBuilder order)
        {
            return order.GetOrderType() == OrderType.NONE ||
                !Enum.IsDefined(typeof(OrderType), order.GetOrderType())
                       ? "MISSING VALUE - OrderType is missing for DeliverOrder, use SetOrderType()."
                       : "";
        }

        private static string ValidateOrderId(DeliverOrderBuilder order)
        {
            return order.GetOrderId() <= 0 ? "MISSING VALUE - SetOrderId is required." : "";
        }

        private static string ValidateInvoiceDetails(DeliverOrderBuilder order)
        {
            if (order.GetOrderId() > 0 && order.GetOrderType() == OrderType.INVOICE &&
                (order.GetInvoiceDistributionType() == InvoiceDistributionType.NONE ||
                !Enum.IsDefined(typeof (InvoiceDistributionType), order.GetInvoiceDistributionType())))
            {
                return "MISSING VALUE - SetInvoiceDistributionType is required for DeliverInvoiceOrder.";
            }
            return "";
        }

        private static string ValidateOrderRows(DeliverOrderBuilder order)
        {
            var rowCount = order.GetOrderRows().Count + order.GetShippingFeeRows().Count +
                           order.GetInvoiceFeeRows().Count;
            if (order.GetOrderType() == OrderType.INVOICE && rowCount == 0)
            {
                return "MISSING VALUE - No order or fee has been included. Use AddOrder(...) or AddFee(...).";
            }
            return "";
        }
    }
}