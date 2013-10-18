using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Webservice.Helper
{
    public class WebServiceRowFormatter<T>
    {
        private readonly OrderBuilder<T> _order;

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
                decimal vatPercentAsHundredth = existingRow.GetVatPercent() != null
                                                    ? existingRow.GetVatPercent().GetValueOrDefault() * 0.01M
                                                    : 0;
                if (existingRow.GetVatPercent() != null && existingRow.GetAmountExVat() != null)
                {
                    _totalAmountExVat += existingRow.GetAmountExVat().GetValueOrDefault();
                    _totalVatAsAmount += vatPercentAsHundredth * existingRow.GetAmountExVat().GetValueOrDefault();
                }
                else if (existingRow.GetVatPercent() != null && existingRow.GetAmountIncVat() != null)
                {
                    _totalAmountInclVat += existingRow.GetAmountIncVat().GetValueOrDefault();
                    _totalVatAsAmount += (vatPercentAsHundredth / (1 + vatPercentAsHundredth)) *
                                         existingRow.GetAmountIncVat().GetValueOrDefault();
                }
                else
                {
                    _totalAmountInclVat += existingRow.GetAmountIncVat().GetValueOrDefault();
                    _totalAmountExVat += existingRow.GetAmountExVat().GetValueOrDefault();
                    _totalVatAsAmount += existingRow.GetAmountIncVat().GetValueOrDefault() -
                                         existingRow.GetAmountExVat().GetValueOrDefault();
                }
            }
            _totalAmountInclVat = _totalAmountExVat + _totalVatAsAmount;
            _totalAmountExVat = _totalAmountInclVat - _totalVatAsAmount;
            _totalVatAsPercent = _totalAmountInclVat != 0 ? (_totalVatAsAmount / _totalAmountInclVat) : 0;
            // e.g. 0,20 if percentage 20
        }

        private void FormatRows(IEnumerable<IRowBuilder> rows)
        {
            foreach (var existingRow in rows)
            {
                var orderRow = new OrderRow();
                orderRow = SerializeOrder(existingRow.GetArticleNumber(), existingRow.GetDescription(),
                                          existingRow.GetName(), existingRow.GetUnit(), orderRow);

                orderRow.DiscountPercent = existingRow.GetDiscountPercent();
                orderRow.NumberOfUnits = existingRow.GetQuantity();

                if (existingRow is FixedDiscountBuilder)
                {
                    decimal amount = ((FixedDiscountBuilder)existingRow).GetAmount();
                    decimal productTotalAfterDiscount = _totalAmountInclVat - amount;
                    decimal totalProductVatAsAmountAfterDiscount = _totalVatAsPercent * productTotalAfterDiscount;
                    decimal discountVatAsAmount = _totalVatAsAmount - totalProductVatAsAmountAfterDiscount;
                    decimal pricePerUnitExMoms = Math.Round((amount - discountVatAsAmount) * 100.0M) / 100.0M;

                    orderRow.PricePerUnit = -pricePerUnitExMoms;
                    orderRow.VatPercent =
                        Math.Round((discountVatAsAmount * 100.0M / (amount - discountVatAsAmount) * 100.0M)) /
                        100.0M;
                }
                else if (existingRow is RelativeDiscountBuilder)
                {
                    decimal pricePerUnitExVat =
                        Math.Round((_totalAmountExVat * (existingRow.GetDiscountPercent() * 0.01M)) * 100.00M) /
                        100.00M;

                    orderRow.PricePerUnit = -pricePerUnitExVat;

                    orderRow.VatPercent =
                        Math.Round(
                            (_totalVatAsAmount * 100.0M * (existingRow.GetDiscountPercent() / pricePerUnitExVat / 100.0M)),
                            2);

                    //Relative discounts is a special case where we want to use the discount percent in calculations
                    //but not display it on the order row. Since that would imply that it's a discount on a discount.
                    //So that is why we force the value to 0 below.
                    orderRow.DiscountPercent = 0;
                }
                else
                {
                    orderRow = SerializeAmountAndVat(existingRow.GetAmountExVat(), existingRow.GetVatPercent(),
                                 existingRow.GetAmountIncVat(), orderRow);
                }

                _newRows.Add(orderRow);
            }
        }

        private void FormatOrderRows()
        {
            FormatRows(_order.GetOrderRows());
        }

        private void FormatShippingFeeRows()
        {
            if (_order.GetShippingFeeRows() == null)
            {
                return;
            }
            FormatRows(_order.GetShippingFeeRows());
        }

        private void FormatInvoiceFeeRows()
        {
            if (_order.GetInvoiceFeeRows() == null)
            {
                return;
            }
            FormatRows(_order.GetInvoiceFeeRows());
        }

        private void FormatFixedDiscountRows()
        {
            if (_order.GetFixedDiscountRows() == null)
            {
                return;
            }
            FormatRows(_order.GetFixedDiscountRows());
        }

        private void FormatRelativeDiscountRows()
        {
            if (_order.GetRelativeDiscountRows() == null)
            {
                return;
            }
            FormatRows(_order.GetRelativeDiscountRows());
        }

        private OrderRow SerializeOrder(string articleNumber, string description, string name, string unit,
                                        OrderRow orderRow)
        {
            orderRow.ArticleNumber = articleNumber;

            if (description != null)
            {
                orderRow.Description = (name != null ? name + ": " : "") + "" + description;
            }
            else if (name != null)
            {
                orderRow.Description = name;
            }

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
                orderRow.PricePerUnit = amountIncVat.GetValueOrDefault() /
                                        ((0.01M * vatPercent.GetValueOrDefault()) + 1);
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