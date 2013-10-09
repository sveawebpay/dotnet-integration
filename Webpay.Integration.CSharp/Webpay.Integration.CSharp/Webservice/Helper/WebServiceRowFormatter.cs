using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Webservice.Helper
{
    public class WebServiceRowFormatter<T>
    {
        private OrderBuilder<T> _order;

        private decimal _totalAmountExVat;
        private decimal _totalAmountInclVat;
        private decimal _totalVatAsAmount;
        private decimal _totalVatAsPercent;

        private List<OrderRow> _newRows;

        public WebServiceRowFormatter(OrderBuilder<T> order)
        {
            _order = order;
        }

        public OrderRow[] FormatRows()
        {
            _newRows = new List<OrderRow>();

            CalculateTotals();

            FormatOrderRows();
            FormatShippingFeeRows();
            FormatInvoiceFeeRows();
            FormatFixedDiscountRows();
            FormatRelativeDiscountRows();
            return _newRows.ToArray();
        }

        private void CalculateTotals()
        {
            _totalAmountExVat = 0;
            _totalVatAsAmount = 0;
            List<OrderRowBuilder> orderRows = _order.GetOrderRows();
            foreach (OrderRowBuilder existingRow in orderRows)
            {
                decimal vatPercentAsHundredth = existingRow.GetVatPercent() != null ? existingRow.GetVatPercent().GetValueOrDefault() * new decimal(0.01) : 0;

                if (existingRow.GetVatPercent() != null && existingRow.GetAmountExVat() != null)
                {
                    _totalAmountExVat += existingRow.GetAmountExVat().GetValueOrDefault();
                    _totalVatAsAmount += vatPercentAsHundredth * existingRow.GetAmountExVat().GetValueOrDefault();
                }
                else if (existingRow.GetVatPercent() != null && existingRow.GetAmountIncVat() != null)
                {
                    _totalAmountInclVat += existingRow.GetAmountIncVat().GetValueOrDefault();
                    _totalVatAsAmount += (vatPercentAsHundredth/(1 + vatPercentAsHundredth))*
                                         existingRow.GetAmountIncVat().GetValueOrDefault();
                }
                else
                {
                    _totalAmountInclVat += existingRow.GetAmountIncVat().GetValueOrDefault();
                    _totalAmountExVat += existingRow.GetAmountExVat().GetValueOrDefault();
                    _totalVatAsAmount += existingRow.GetAmountIncVat().GetValueOrDefault() - existingRow.GetAmountExVat().GetValueOrDefault();
                }
            }
            _totalAmountInclVat = _totalAmountExVat + _totalVatAsAmount;
            _totalAmountExVat = _totalAmountInclVat - _totalVatAsAmount;
            _totalVatAsPercent = _totalAmountInclVat != 0 ? (_totalVatAsAmount / _totalAmountInclVat) : 0; // e.g. 0,20 if percentage 20
        }

        private void FormatOrderRows()
        {
            List<OrderRowBuilder> orderRows = _order.GetOrderRows();
            foreach (OrderRowBuilder existingRow in orderRows)
            {
                var orderRow = new OrderRow();
                orderRow = SerializeOrder(existingRow.GetArticleNumber(), existingRow.GetDescription(),
                                          existingRow.GetName(), existingRow.GetUnit(), orderRow);

                orderRow.DiscountPercent = existingRow.GetDiscountPercent();
                orderRow.NumberOfUnits = existingRow.GetQuantity();

                orderRow = SerializeAmountAndVat(existingRow.GetAmountExVat(), existingRow.GetVatPercent(),
                                                 existingRow.GetAmountIncVat(), orderRow);
                _newRows.Add(orderRow);
            }
        }

        private void FormatShippingFeeRows()
        {
            if (_order.GetShippingFeeRows() == null)
            {
                return;
            }
            List<ShippingFeeBuilder> shippingFeeRows = _order.GetShippingFeeRows();
            foreach (ShippingFeeBuilder row in shippingFeeRows)
            {
                var orderRow = new OrderRow();
                orderRow = SerializeOrder(row.GetShippingId(), row.GetDescription(), row.GetName(), row.GetUnit(),
                                          orderRow);

                orderRow.DiscountPercent = row.GetDiscountPercent();
                orderRow.NumberOfUnits = 1;
                orderRow = SerializeAmountAndVat(row.GetAmountExVat(), row.GetVatPercent(), row.GetAmountIncVat(), orderRow);
                _newRows.Add(orderRow);
            }
        }

        private void FormatInvoiceFeeRows()
        {
            if (_order.GetInvoiceFeeRows() == null)
            {
                return;
            }
            List<InvoiceFeeBuilder> invoiceFeeRows = _order.GetInvoiceFeeRows();
            foreach (InvoiceFeeBuilder row in invoiceFeeRows)
            {
                var orderRow = new OrderRow();
                orderRow = SerializeOrder("", row.GetDescription(), row.GetName(), row.GetUnit(), orderRow);

                orderRow.DiscountPercent = row.GetDiscountPercent();
                orderRow.NumberOfUnits = 1;
                orderRow = SerializeAmountAndVat(row.GetAmountExVat(), row.GetVatPercent(), row.GetAmountIncVat(), orderRow);

                _newRows.Add(orderRow);
            }
        }

        private void FormatFixedDiscountRows()
        {
            if (_order.GetFixedDiscountRows() == null)
            {
                return;
            }
            List<FixedDiscountBuilder> fixedDiscountRows = _order.GetFixedDiscountRows();
            foreach (FixedDiscountBuilder row in fixedDiscountRows)
            {
                decimal productTotalAfterDiscount = _totalAmountInclVat - row.GetAmount();
                decimal totalProductVatAsAmountAfterDiscount = _totalVatAsPercent * productTotalAfterDiscount;
                decimal discountVatAsAmount = _totalVatAsAmount - totalProductVatAsAmountAfterDiscount;

                var orderRow = new OrderRow();
                orderRow = SerializeOrder(row.GetDiscountId(), row.GetDescription(), row.GetName(), row.GetUnit(), orderRow);

                orderRow.DiscountPercent = 0; // no discount on discount
                orderRow.NumberOfUnits = 1; // only one discount per row
                decimal pricePerUnitExMoms = Math.Round((row.GetAmount() - discountVatAsAmount) * new decimal(100.0)) / new decimal(100.0);

                orderRow.PricePerUnit = - pricePerUnitExMoms;
                orderRow.VatPercent =
                    Math.Round((discountVatAsAmount * new decimal(100.0) / (row.GetAmount() - discountVatAsAmount) * new decimal(100.0))) / new decimal(100.0);

                _newRows.Add(orderRow);
            }
        }

        private void FormatRelativeDiscountRows()
        {
            if (_order.GetRelativeDiscountRows() == null)
            {
                return;
            }
            List<RelativeDiscountBuilder> relativeDiscountRows = _order.GetRelativeDiscountRows();
            foreach (RelativeDiscountBuilder row in relativeDiscountRows)
            {
                var orderRow = new OrderRow();

                orderRow = SerializeOrder(row.GetDiscountId(), row.GetDescription(), row.GetName(), row.GetUnit(),
                                          orderRow);

                orderRow.DiscountPercent = 0; // no discount on discount
                orderRow.NumberOfUnits = 1; // only one discount per row

                decimal pricePerUnitExMoms = Math.Round((_totalAmountExVat * (row.GetDiscountPercent() * new decimal(0.01))) * new decimal(100.00)) / new decimal(100.00);
                orderRow.PricePerUnit = - pricePerUnitExMoms;

                orderRow.VatPercent = Math.Round((_totalVatAsAmount * new decimal(100.0) * (row.GetDiscountPercent() / pricePerUnitExMoms / new decimal(100.0))), 2);

                _newRows.Add(orderRow);
            }
        }

        private OrderRow SerializeOrder(string articleNumber, string description, string name, string unit,
                                            OrderRow orderRow)
        {
            if (articleNumber != null)
                orderRow.ArticleNumber = articleNumber;

            if (description != null)
                orderRow.Description = (name != null ? name + ": " : "") + "" + description;
            else if (name != null)
                orderRow.Description = name;

            if (unit != null)
                orderRow.Unit = unit;
            return orderRow;
        }

        private OrderRow SerializeAmountAndVat(decimal? amountExVat, decimal? vatPercent, decimal? amountIncVat,
                                                   OrderRow orderRow)
        {
            if (vatPercent != null && amountExVat != null)
            {
                orderRow.PricePerUnit = amountExVat.GetValueOrDefault();
                orderRow.VatPercent = vatPercent.GetValueOrDefault();
            }
            else if (vatPercent != null && amountIncVat != null)
            {
                orderRow.PricePerUnit = amountIncVat.GetValueOrDefault() / ((new decimal(0.01) * vatPercent.GetValueOrDefault()) + 1);
                orderRow.VatPercent = vatPercent.GetValueOrDefault();
            }
            else
            {
                orderRow.PricePerUnit = amountExVat.GetValueOrDefault();
                orderRow.VatPercent = ((amountIncVat.GetValueOrDefault() / amountExVat.GetValueOrDefault()) - 1) * 100;
            }
            return orderRow;
        }
    }
}