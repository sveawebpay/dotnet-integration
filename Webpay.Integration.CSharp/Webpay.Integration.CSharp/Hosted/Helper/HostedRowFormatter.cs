using System;
using System.Collections.Generic;
using System.Linq;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Calculation;

namespace Webpay.Integration.CSharp.Hosted.Helper
{
    public class HostedRowFormatter<T>
    {
        private decimal _totalAmount;
        private decimal _totalVat;
        private decimal _totalShippingAmount;
        private decimal _totalShippingVat;
        private readonly List<HostedOrderRowBuilder> _newRows;

        public HostedRowFormatter()
        {
            _totalAmount = 0M;
            _totalVat = 0M;

            _totalShippingAmount = 0M;
            _totalShippingVat = 0M;

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
                                        ? (row.GetVatPercent().GetValueOrDefault() / 100) + 1
                                        : 0;

                decimal amountExVat = row.GetAmountExVat().GetValueOrDefault();
                decimal amountIncVat = row.GetAmountIncVat().GetValueOrDefault();
                decimal tempAmount;
                decimal tempVat;

                if (row.GetAmountExVat() != null && row.GetVatPercent() != null)
                {
                    tempAmount = amountExVat * vatFactor;
                    tempVat = amountExVat * (row.GetVatPercent().GetValueOrDefault() / 100);
                }
                else if (row.GetAmountIncVat() != null && row.GetVatPercent() != null)
                {
                    tempAmount = amountIncVat;
                    tempVat = amountIncVat - (amountIncVat / vatFactor);
                }
                else
                {
                    tempAmount = amountIncVat;
                    tempVat = amountIncVat - amountExVat;
                }

                tempRow.SetAmount(MathUtil.ConvertFromDecimalToCentesimal(tempAmount));
                tempRow.SetVat(MathUtil.ConvertFromDecimalToCentesimal(tempVat));
                tempRow.SetQuantity(row.GetQuantity());

                _totalAmount += tempAmount * row.GetQuantity();
                _totalVat += tempVat * row.GetQuantity();

                _newRows.Add(tempRow);
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

                decimal vatFactor = row.GetVatPercent() != null
                                        ? (row.GetVatPercent().GetValueOrDefault() / 100) + 1
                                        : 0;

                decimal amountIncVat = row.GetAmountIncVat().GetValueOrDefault();
                decimal amountExVat = row.GetAmountExVat().GetValueOrDefault();
                decimal tempAmount;
                decimal tempVat;

                if (row.GetAmountExVat() != null && row.GetVatPercent() != null)
                {
                    tempAmount = amountExVat * vatFactor;
                    tempVat = tempAmount - amountExVat;
                }
                else if (row.GetAmountIncVat() != null && row.GetVatPercent() != null)
                {
                    tempAmount = amountIncVat;
                    tempVat = tempAmount - (tempAmount / vatFactor);
                }
                else
                {
                    tempAmount = amountIncVat;
                    tempVat = amountIncVat - amountExVat;
                }

                tempRow.SetAmount(MathUtil.ConvertFromDecimalToCentesimal(tempAmount));
                tempRow.SetVat(MathUtil.ConvertFromDecimalToCentesimal(tempVat));
                tempRow.SetQuantity(row.GetQuantity());

                _totalShippingAmount = tempAmount * row.GetQuantity();
                _totalShippingVat = tempVat * row.GetQuantity();

                _totalAmount += _totalShippingAmount;
                _totalVat += _totalShippingVat;

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

                decimal vatFactor = row.GetVatPercent() != null
                                        ? (row.GetVatPercent().GetValueOrDefault() / 100) + 1
                                        : 0;

                decimal tempAmount;
                decimal tempVat;
                decimal amountExVat = row.GetAmountExVat().GetValueOrDefault();
                decimal amountIncVat = row.GetAmountIncVat().GetValueOrDefault();

                if (row.GetAmountExVat() != null && row.GetVatPercent() != null)
                {
                    tempAmount = amountExVat * vatFactor;
                    tempVat = amountExVat * (row.GetVatPercent().GetValueOrDefault() / 100);
                }
                else if (row.GetAmountIncVat() != null && row.GetVatPercent() != null)
                {
                    tempAmount = amountIncVat;
                    tempVat = amountIncVat - (amountIncVat / vatFactor);
                }
                else if (row.GetAmountIncVat() != null && row.GetAmountExVat() != null)
                {
                    tempAmount = amountIncVat;
                    tempVat = amountIncVat - amountExVat;
                }
                else
                {
                    tempAmount = amountIncVat;
                    tempVat = _totalAmount * _totalVat == 0
                                  ? amountIncVat
                                  : amountIncVat / _totalAmount * _totalVat;
                }

                decimal discountedAmount = -tempAmount;
                tempRow.SetAmount(MathUtil.ConvertFromDecimalToCentesimal(discountedAmount));

                _totalAmount += discountedAmount;

                if (_totalVat > 0)
                {
                    decimal discountedVat = -tempVat;
                    tempRow.SetVat(MathUtil.ConvertFromDecimalToCentesimal(discountedVat));
                    _totalVat += discountedVat;
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

                decimal discountFactor = row.GetDiscountPercent() / 100;

                decimal discountAmount = (_totalAmount - _totalShippingAmount) * discountFactor;
                decimal discountVat = 0;
                tempRow.SetAmount(-MathUtil.ConvertFromDecimalToCentesimal(discountAmount));
                _totalAmount = _totalAmount - discountAmount;

                if (_totalVat > 0)
                {
                    discountVat = (_totalVat - _totalShippingVat) * discountFactor;
                    _totalVat = _totalVat - discountVat;
                }

                tempRow.SetAmount(-MathUtil.ConvertFromDecimalToCentesimal(discountAmount));
                tempRow.SetVat(-MathUtil.ConvertFromDecimalToCentesimal(discountVat));

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

        [Obsolete("This method is considered imprecise and may return values effected by rounding errors. Use GetTotalAmount() instead")]
        public long FormatTotalAmount(IEnumerable<HostedOrderRowBuilder> rows)
        {
            return (long) rows.Sum(row => row.GetAmount() * row.GetQuantity());
        }

        [Obsolete("This method is considered imprecise and may return values effected by rounding errors. Use GetTotalVat() instead")]
        public long FormatTotalVat(IEnumerable<HostedOrderRowBuilder> rows)
        {
            return (long) rows.Sum(row => row.GetVat() * row.GetQuantity());
        }

        public long GetTotalAmount()
        {
            return MathUtil.ConvertFromDecimalToCentesimal(_totalAmount);
        }

        public long GetTotalVat()
        {
            return MathUtil.ConvertFromDecimalToCentesimal(_totalVat);
        }
    }
}