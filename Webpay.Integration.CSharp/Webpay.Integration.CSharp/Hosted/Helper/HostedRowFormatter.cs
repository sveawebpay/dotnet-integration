using System;
using System.Collections.Generic;
using System.Linq;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Row;

namespace Webpay.Integration.CSharp.Hosted.Helper
{
    public class HostedRowFormatter<T>
    {
        private long _totalAmount;
        private long _totalVat;
        private readonly List<HostedOrderRowBuilder> _newRows;

        public HostedRowFormatter()
        {
            _totalAmount = 0L;
            _totalVat = 0L;
            _newRows = new List<HostedOrderRowBuilder>();
        }

        public List<HostedOrderRowBuilder> FormatRows(OrderBuilder<T> order)
        {
            FormatOrderRows(order);
            FormatShippingFeeRows(order);
            FormatFixedDiscountRows(order);
            FormatRelativeDiscountRows(order);

            return _newRows;
        }

        private void FormatOrderRows(OrderBuilder<T> orderBuilder)
        {
            foreach (OrderRowBuilder row in orderBuilder.GetOrderRows())
            {
                var tempRow = GetNewTempRow(row, row.GetArticleNumber());

                decimal vatFactor = row.GetVatPercent() != null
                                        ? (row.GetVatPercent().GetValueOrDefault() * 0.01M) + 1
                                        : 0;

                decimal amountExVat = row.GetAmountExVat().GetValueOrDefault();
                tempRow.SetAmount(Convert.ToInt64((amountExVat * 100) * vatFactor));

                if (row.GetAmountExVat() != null && row.GetVatPercent() != null)
                {
                    tempRow.SetAmount(Convert.ToInt64((row.GetAmountExVat() * 100) * vatFactor));
                    tempRow.SetVat(Convert.ToInt64(tempRow.GetAmount() - (row.GetAmountExVat() * 100)));
                }
                else if (row.GetAmountIncVat() != null && row.GetVatPercent() != null)
                {
                    tempRow.SetAmount(Convert.ToInt64((row.GetAmountIncVat() * 100)));
                    tempRow.SetVat(Convert.ToInt64(tempRow.GetAmount() - (tempRow.GetAmount() / vatFactor)));
                }
                else
                {
                    tempRow.SetAmount(Convert.ToInt64(row.GetAmountIncVat() * 100));
                    tempRow.SetVat(Convert.ToInt64((row.GetAmountIncVat() - row.GetAmountExVat()) * 100));
                }

                tempRow.SetQuantity(row.GetQuantity());

                _newRows.Add(tempRow);
                _totalAmount += Convert.ToInt64(tempRow.GetAmount() * row.GetQuantity());
                _totalVat += (long)(tempRow.GetVat() * row.GetQuantity());
            }
        }

        private void FormatShippingFeeRows(OrderBuilder<T> orderBuilder)
        {
            if (orderBuilder.GetShippingFeeRows() == null)
            {
                return;
            }

            foreach (ShippingFeeBuilder row in orderBuilder.GetShippingFeeRows())
            {
                var tempRow = GetNewTempRow(row, row.GetShippingId());

                decimal plusVatCounter = row.GetVatPercent() != null
                             ? (row.GetVatPercent().GetValueOrDefault() * 0.01M) + 1
                             : 0;

                if (row.GetAmountExVat() != null && row.GetVatPercent() != null)
                {
                    tempRow.SetAmount(Convert.ToInt64((row.GetAmountExVat().GetValueOrDefault() * 100) * plusVatCounter));
                    tempRow.SetVat(
                        Convert.ToInt64(tempRow.GetAmount() - (row.GetAmountExVat().GetValueOrDefault() * 100)));
                }
                else if (row.GetAmountIncVat() != null && row.GetVatPercent() != null)
                {
                    tempRow.SetAmount(Convert.ToInt64(row.GetAmountIncVat().GetValueOrDefault() * 100));
                    tempRow.SetVat(Convert.ToInt64(tempRow.GetAmount() - (tempRow.GetAmount() / plusVatCounter)));
                }
                else
                {
                    decimal amountIncVat = row.GetAmountIncVat().GetValueOrDefault();
                    tempRow.SetAmount(Convert.ToInt64(amountIncVat * 100));
                    decimal amountExVat = row.GetAmountExVat().GetValueOrDefault();
                    tempRow.SetVat(Convert.ToInt64(amountIncVat - amountExVat));
                }

                _newRows.Add(tempRow);
            }
        }

        private void FormatFixedDiscountRows(OrderBuilder<T> orderBuilder)
        {
            if (orderBuilder.GetFixedDiscountRows() == null)
            {
                return;
            }

            foreach (FixedDiscountBuilder row in orderBuilder.GetFixedDiscountRows())
            {
                var tempRow = GetNewTempRow(row, row.GetDiscountId());

                tempRow.SetAmount(Convert.ToInt64(-(row.GetAmount() * 100)));

                _totalAmount -= Convert.ToInt64(row.GetAmount());

                double discountFactor = tempRow.GetAmount() * 1.0 / _totalAmount;

                if (_totalVat > 0)
                {
                    tempRow.SetVat(Convert.ToInt64(_totalVat * discountFactor));
                }

                _newRows.Add(tempRow);
            }
        }

        private void FormatRelativeDiscountRows(OrderBuilder<T> orderBuilder)
        {
            if (orderBuilder.GetRelativeDiscountRows() == null)
            {
                return;
            }

            foreach (RelativeDiscountBuilder row in orderBuilder.GetRelativeDiscountRows())
            {
                var tempRow = GetNewTempRow(row, row.GetDiscountId());

                double discountFactor = row.GetDiscountPercent() * 0.01;

                tempRow.SetAmount(Convert.ToInt64(-(discountFactor * _totalAmount)));
                _totalAmount -= tempRow.GetAmount();

                if (_totalVat > 0)
                {
                    tempRow.SetVat(Convert.ToInt64(-(_totalVat * discountFactor)));
                }

                _newRows.Add(tempRow);
            }
        }

        private HostedOrderRowBuilder GetNewTempRow(IRowBuilder row, string sku)
        {
            var tempRow = new HostedOrderRowBuilder();

            if (row.GetName() != null)
            {
                tempRow.SetName(row.GetName());
            }

            if (row.GetDescription() != null)
            {
                tempRow.SetDescription(row.GetDescription());
            }

            tempRow.SetQuantity(1);
            tempRow.SetUnit(row.GetUnit());
            tempRow.SetSku(sku);

            return tempRow;
        }

        public long FormatTotalAmount(IEnumerable<HostedOrderRowBuilder> rows)
        {
            return (long) rows.Sum(row => row.GetAmount() * row.GetQuantity());
        }

        public long FormatTotalVat(IEnumerable<HostedOrderRowBuilder> rows)
        {
            return (long) rows.Sum(row => row.GetVat() * row.GetQuantity());
        }
    }
}