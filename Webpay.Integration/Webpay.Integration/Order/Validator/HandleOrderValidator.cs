﻿using System.Text;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Order.Validator;

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
            RequiresOrderRowValidation(order.GetOrderType()) ? ValidateOrderRows(order) : ""
        };

        foreach (var line in list.Where(line => !string.IsNullOrWhiteSpace(line)))
        {
            stringBuilder.AppendLine(line);
        }

        return stringBuilder.ToString().Trim();
    }

    private static bool RequiresOrderRowValidation(OrderType orderType)
    {
        return orderType == OrderType.INVOICE ||
               orderType == OrderType.PAYMENTPLAN ||
               orderType == OrderType.ACCOUNTCREDIT;
    }

    private static string ValidateCountry(DeliverOrderBuilder order)
    {
        return order.GetCountryCode() == CountryCode.NONE ||
               !Enum.IsDefined(typeof (CountryCode), order.GetCountryCode())
                   ? "MISSING VALUE - CountryCode is required, use SetCountryCode(...)."
                   : "";
    }

    private static string ValidateOrderType(DeliverOrderBuilder order)
    {
        return order.GetOrderType() == OrderType.NONE ||
               !Enum.IsDefined(typeof (OrderType), order.GetOrderType())
                   ? "MISSING VALUE - OrderType is missing for DeliverOrder, use SetOrderType()."
                   : "";
    }

    private static string ValidateOrderId(DeliverOrderBuilder order)
    {
        return order.GetOrderId() <= 0 ? "MISSING VALUE - SetOrderId is required." : "";
    }

    private static string ValidateInvoiceDetails(DeliverOrderBuilder order)
    {
        string errors = "";
        if (order.GetOrderId() > 0 && order.GetOrderType() == OrderType.INVOICE &&
            (order.GetInvoiceDistributionType() == DistributionType.NONE ||
             !Enum.IsDefined(typeof (DistributionType), order.GetInvoiceDistributionType())))
        {
            errors += "MISSING VALUE - SetInvoiceDistributionType is required for DeliverInvoiceOrder.\n";
        }

        if(order.GetInvoiceDistributionType() == DistributionType.EINVOICEB2B && order.GetCountryCode() != CountryCode.NO)
        {
            errors += "NOT VALID - Invalid country code, must be CountryCode.NO if InvoiceDistributionType is DistributionType.EInvoiceB2B.\n";
        }

        if(order.GetInvoiceDistributionType() == DistributionType.EINVOICEB2B && order.GetOrderType() == OrderType.PAYMENTPLAN)
        {
            errors += "NOT VALID - Invalid payment method, DistributionType.EINVOICEB2B can only be used when payment method is invoice.\n";
        }

        return errors;
    }

    private static string ValidateOrderRows(DeliverOrderBuilder order)
    {
        var rowCount = order.GetOrderRows().Count + order.GetShippingFeeRows().Count +
                       order.GetInvoiceFeeRows().Count;
        if (order.GetOrderType() == OrderType.INVOICE && rowCount == 0)
        {
            return "MISSING VALUE - No order or fee has been included. Use AddOrderRow(...) or AddFee(...).";
        }
        return "";
    }
}