using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Calculation;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Webservice.Helper
{


    public class WebServiceRowFormatter<T>
    {
        private readonly OrderBuilder<T> _order;
        private readonly bool _useIncVatRequestIfPossible;

        private decimal _totalAmountExVat;
        private decimal _totalAmountIncVat;
        private decimal _totalVatAsAmount;

        private Dictionary<decimal, decimal> _totalAmountPerVatRateIncVat;

        private List<OrderRow> _newRows;

        public WebServiceRowFormatter(OrderBuilder<T> order) : this(order, true)
        {
        }

        public WebServiceRowFormatter(OrderBuilder<T> order, bool useIncVatRequestIfPossible)
        {
            _order = order;
            _useIncVatRequestIfPossible = useIncVatRequestIfPossible;
        }

        public List<OrderRow> FormatRowsOld()
        {
            _newRows = new List<OrderRow>();

            CalculateTotals();

            FormatOrderRows();
            FormatShippingFeeRows();
            FormatInvoiceFeeRows();
            FormatFixedDiscountRows();
            FormatRelativeDiscountRows();
            return _newRows;
        }

        public class Order
        {
            public decimal TotalAmountIncVat { get; set; }
            public decimal TotalAmountExVat { get; set; }
            public decimal TotalVatAsAmount { get; set; }
            public Dictionary<decimal, decimal> TotalAmountPerVatRateIncVat { get; private set; }
            public bool AllPricesAreSpecifiedIncVat { get; set; }
            public OrderBuilder<T> Original { get; private set; }
            public List<OrderBuilder<T>> NewOrderRows { get; private set; }

            public Order(OrderBuilder<T> original)
            {
                Original = original;
                NewOrderRows = new List<OrderBuilder<T>>();
                TotalAmountPerVatRateIncVat = new Dictionary<decimal, decimal>();
            }
        }

        public List<OrderRow> FormatRows()
        {
            _newRows = new List<OrderRow>();

            Order res = new Maybe<OrderBuilder<T>>(_order)
                .And(ConvertToOrder)
                .And(CheckIfRowsIncVat)
                .And(FillMissingValues)
                .And(SumByVat)
                .And(ApplyRelativeDiscounts)
                .And(ApplyFixedDiscounts)
                .And(ConvertToWebserviceOrder)
                .Value;


            CalculateTotals();

            FormatOrderRows();
            FormatShippingFeeRows();
            FormatInvoiceFeeRows();
            FormatFixedDiscountRows();
            FormatRelativeDiscountRows();
            return _newRows;
        }

        public static Order ConvertToOrder(OrderBuilder<T> orderBuilder)
        {
            return new Order(orderBuilder);
        }

        public static Order CheckIfRowsIncVat(Order order)
        {
            order.AllPricesAreSpecifiedIncVat = order.Original.GetOrderRows().All(orderRow => orderRow.GetAmountIncVat() != null);
            return order;
        }

        public static Order FillMissingValues(Order order)
        {
            order.Original.GetOrderRows().ConvertAll(orderRow =>
                {
                    var newRow = NewRowBasedOnExisting(orderRow);

                    return newRow;
                });
            return order;
        }


        public static Order SumByVat(Order order)
        {
            List<OrderRowBuilder> orderRows = order.Original.GetOrderRows();

            foreach (OrderRowBuilder existingRow in orderRows)
            {
                decimal vatPercent = existingRow.GetVatPercent().GetValueOrDefault();
                decimal vatPercentAsHundredth = vatPercent / 100;

                decimal amountExVat = existingRow.GetAmountExVat().GetValueOrDefault();
                decimal amountIncVat = existingRow.GetAmountIncVat().GetValueOrDefault();
                decimal quantity = existingRow.GetQuantity();

                if (existingRow.GetVatPercent() != null && existingRow.GetAmountExVat() != null)
                {
                    order.TotalAmountExVat += amountExVat * quantity;
                    order.TotalVatAsAmount += vatPercentAsHundredth * amountExVat * quantity;
                    order.TotalAmountIncVat += (amountExVat + (vatPercentAsHundredth * amountExVat)) * quantity;

                    if (order.TotalAmountPerVatRateIncVat.ContainsKey(vatPercent))
                    {
                        order.TotalAmountPerVatRateIncVat[vatPercent] +=
                            (amountExVat * quantity * (1 + vatPercentAsHundredth));
                    }
                    else
                    {
                        order.TotalAmountPerVatRateIncVat.Add(vatPercent, amountExVat * quantity * (1 + vatPercentAsHundredth));
                    }
                }
                else if (existingRow.GetVatPercent() != null && existingRow.GetAmountIncVat() != null)
                {
                    order.TotalAmountIncVat += amountIncVat * quantity;
                    order.TotalVatAsAmount += (vatPercentAsHundredth / (1 + vatPercentAsHundredth)) * amountIncVat * quantity;
                    order.TotalAmountExVat += (amountIncVat - ((vatPercentAsHundredth / (1 + vatPercentAsHundredth)) * amountIncVat)) * quantity;

                    if (order.TotalAmountPerVatRateIncVat.ContainsKey(vatPercent))
                    {
                        order.TotalAmountPerVatRateIncVat[vatPercent] += amountIncVat * quantity;
                    }
                    else
                    {
                        order.TotalAmountPerVatRateIncVat.Add(vatPercent, amountIncVat * quantity);
                    }
                }
                else
                {
                    order.TotalAmountIncVat += amountIncVat * quantity;
                    order.TotalAmountExVat += amountExVat * quantity;
                    order.TotalVatAsAmount += (amountIncVat - amountExVat) * quantity;

                    decimal vatRate = (amountIncVat == 0.0M || amountExVat == 0.0M) ? 0 :
                        ((amountIncVat / amountExVat) - 1) * 100;

                    if (order.TotalAmountPerVatRateIncVat.ContainsKey(vatRate))
                    {
                        order.TotalAmountPerVatRateIncVat[vatRate] +=
                            (amountExVat * quantity * (1 + vatRate / 100));
                    }
                    else
                    {
                        order.TotalAmountPerVatRateIncVat.Add(vatRate, amountExVat * quantity * (1 + vatRate / 100));
                    }
                }
            }
            return order;
        }

        public static Order ApplyFixedDiscounts(Order order)
        {
            return order;
        }

        public static Order ApplyRelativeDiscounts(Order order)
        {
            return order;
        }

        public static Order ConvertToWebserviceOrder(Order order)
        {
            return order;
        }




        private void CalculateTotals()
        {
            _totalAmountIncVat = 0;
            _totalAmountExVat = 0;
            _totalVatAsAmount = 0;

            _totalAmountPerVatRateIncVat = new Dictionary<decimal, decimal>();

            List<OrderRowBuilder> orderRows = _order.GetOrderRows();

            foreach (OrderRowBuilder existingRow in orderRows)
            {
                decimal vatPercent = existingRow.GetVatPercent().GetValueOrDefault();
                decimal vatPercentAsHundredth = vatPercent / 100;

                decimal amountExVat = existingRow.GetAmountExVat().GetValueOrDefault();
                decimal amountIncVat = existingRow.GetAmountIncVat().GetValueOrDefault();
                decimal quantity = existingRow.GetQuantity();

                if (existingRow.GetVatPercent() != null && existingRow.GetAmountExVat() != null)
                {
                    _totalAmountExVat += amountExVat * quantity;
                    _totalVatAsAmount += vatPercentAsHundredth * amountExVat * quantity;
                    _totalAmountIncVat += (amountExVat + (vatPercentAsHundredth * amountExVat)) * quantity;

                    if (_totalAmountPerVatRateIncVat.ContainsKey(vatPercent))
                    {
                        _totalAmountPerVatRateIncVat[vatPercent] +=
                            (amountExVat * quantity * (1 + vatPercentAsHundredth));
                    }
                    else
                    {
                        _totalAmountPerVatRateIncVat.Add(vatPercent, amountExVat * quantity * (1 + vatPercentAsHundredth));
                    }
                }
                else if (existingRow.GetVatPercent() != null && existingRow.GetAmountIncVat() != null)
                {
                    _totalAmountIncVat += amountIncVat * quantity;
                    _totalVatAsAmount += (vatPercentAsHundredth / (1 + vatPercentAsHundredth)) * amountIncVat * quantity;
                    _totalAmountExVat += (amountIncVat - ((vatPercentAsHundredth / (1 + vatPercentAsHundredth)) * amountIncVat)) * quantity;

                    if (_totalAmountPerVatRateIncVat.ContainsKey(vatPercent))
                    {
                        _totalAmountPerVatRateIncVat[vatPercent] += amountIncVat * quantity;
                    }
                    else
                    {
                        _totalAmountPerVatRateIncVat.Add(vatPercent, amountIncVat * quantity);
                    }
                }
                else
                {
                    _totalAmountIncVat += amountIncVat * quantity;
                    _totalAmountExVat += amountExVat * quantity;
                    _totalVatAsAmount += (amountIncVat - amountExVat) * quantity;

                    decimal vatRate = (amountIncVat == 0.0M || amountExVat == 0.0M) ? 0 : 
                        ((amountIncVat / amountExVat) - 1) * 100;

                    if (_totalAmountPerVatRateIncVat.ContainsKey(vatRate))
                    {
                        _totalAmountPerVatRateIncVat[vatRate] +=
                            (amountExVat * quantity * (1 + vatRate / 100));
                    }
                    else
                    {
                        _totalAmountPerVatRateIncVat.Add(vatRate, amountExVat * quantity * (1 + vatRate / 100));
                    }
                }
            }
        }

        private static OrderRow NewRowBasedOnExisting(IRowBuilder existingRow)
        {
            var newOrderRow = new OrderRow();
            newOrderRow = SerializeOrder(existingRow.GetArticleNumber(), existingRow.GetDescription(),
                                         existingRow.GetName(), existingRow.GetUnit(), newOrderRow);

            newOrderRow.DiscountPercent = existingRow.GetDiscountPercent();
            newOrderRow.NumberOfUnits = existingRow.GetQuantity();

            return newOrderRow;
        }

        private void FormatRowLists(IEnumerable<IRowBuilder> rows)
        {

            foreach (var existingRow in rows)
            {
                if (existingRow is FixedDiscountBuilder)
                {
                    if (existingRow.GetAmountIncVat() != null && existingRow.GetVatPercent() == null && existingRow.GetAmountExVat() == null)
                    {
                        foreach (var rateAmountValuePair in _totalAmountPerVatRateIncVat)
                        {
                            var orderRow = NewRowBasedOnExisting(existingRow);

                            decimal vatRate = rateAmountValuePair.Key;
                            decimal amountAtThisVatRateIncVat = rateAmountValuePair.Value;

                            if (_totalAmountPerVatRateIncVat.Count > 1)
                            {
                                string name = existingRow.GetName();
                                string description = existingRow.GetDescription();

                                orderRow.Description = FormatDiscountRowDescription(name, description, (long) vatRate);
                            }

                            decimal discountAtThisVatRateIncVat =
                                existingRow.GetAmountIncVat().GetValueOrDefault() * (amountAtThisVatRateIncVat / _totalAmountIncVat);
                            decimal discountAtThisVatRateExVat = discountAtThisVatRateIncVat - discountAtThisVatRateIncVat * MathUtil.ReverseVatRate(vatRate);

                            orderRow.PricePerUnit = -MathUtil.BankersRound(discountAtThisVatRateExVat);
                            orderRow.VatPercent = vatRate;

                            _newRows.Add(orderRow);
                        }
                    }
                    else if (existingRow.GetAmountIncVat() != null && existingRow.GetVatPercent() != null && existingRow.GetAmountExVat() == null)
                    {
                        var orderRow = NewRowBasedOnExisting(existingRow);

                        decimal vatRate = existingRow.GetVatPercent().GetValueOrDefault();
                        decimal discountAtThisVatRateIncVat = existingRow.GetAmountIncVat().GetValueOrDefault();
                        decimal discountAtThisVatRateExVat = discountAtThisVatRateIncVat - discountAtThisVatRateIncVat * MathUtil.ReverseVatRate(vatRate);

                        orderRow.PricePerUnit = -MathUtil.BankersRound(discountAtThisVatRateExVat);
                        orderRow.VatPercent = vatRate;

                        _newRows.Add(orderRow);
                    }
                    else if (existingRow.GetAmountIncVat() == null && existingRow.GetVatPercent() != null && existingRow.GetAmountExVat() != null)
                    {
                        var orderRow = NewRowBasedOnExisting(existingRow);

                        orderRow.PricePerUnit = -MathUtil.BankersRound(existingRow.GetAmountExVat().GetValueOrDefault());
                        orderRow.VatPercent = existingRow.GetVatPercent().GetValueOrDefault();

                        _newRows.Add(orderRow);
                    }
                }
                else if (existingRow is RelativeDiscountBuilder)
                {
                    foreach (var rateAmountValuePair in _totalAmountPerVatRateIncVat)
                    {
                        var orderRow = NewRowBasedOnExisting(existingRow);

                        decimal vatRate = rateAmountValuePair.Key;
                        decimal amountAtThisVatRateIncVat = rateAmountValuePair.Value;

                        if (_totalAmountPerVatRateIncVat.Count > 1)
                        {
                            string name = existingRow.GetName();
                            string description = existingRow.GetDescription();

                            orderRow.Description = FormatDiscountRowDescription(name, description, (long) vatRate);
                        }

                        decimal amountAtThisVatRateExVat = amountAtThisVatRateIncVat - amountAtThisVatRateIncVat * MathUtil.ReverseVatRate(vatRate);
                        decimal discountExVat = amountAtThisVatRateExVat * (existingRow.GetDiscountPercent() / 100);

                        orderRow.PricePerUnit = -MathUtil.BankersRound(discountExVat);
                        orderRow.VatPercent = vatRate;

                        //Relative discounts is a special case where we want to use the discount percent in calculations
                        //but not display it on the order row. Since that would imply that it's a discount on a discount.
                        //So that is why we force the value to 0 below.
                        orderRow.DiscountPercent = 0;

                        _newRows.Add(orderRow);
                    }
                }
                else
                {
                    var allPricesAreSpecifiedIncVat = rows.All(orderRow => orderRow.GetAmountIncVat() != null);
                    _newRows.Add(SerializeAmountAndVat(
                            existingRow.GetAmountExVat(), 
                            existingRow.GetVatPercent(),
                            existingRow.GetAmountIncVat(), 
                            NewRowBasedOnExisting(existingRow), 
                            _useIncVatRequestIfPossible && allPricesAreSpecifiedIncVat));
                }
            }
        }

        private string FormatDiscountRowDescription(string name, string description, long vatRate)
        {
            string formattedDescription;
            if (name != null)
            {
                formattedDescription = name + (description == null ? "" : ": " + description);
            }
            else
            {
                formattedDescription = description ?? "";
            }

            formattedDescription += " (" + vatRate + "%)";

            return formattedDescription;
        }

        private void FormatOrderRows()
        {
            FormatRowLists(_order.GetOrderRows());
        }

        private void FormatShippingFeeRows()
        {
            if (_order.GetShippingFeeRows() == null)
            {
                return;
            }
            FormatRowLists(_order.GetShippingFeeRows());
        }

        private void FormatInvoiceFeeRows()
        {
            if (_order.GetInvoiceFeeRows() == null)
            {
                return;
            }
            FormatRowLists(_order.GetInvoiceFeeRows());
        }

        private void FormatFixedDiscountRows()
        {
            if (_order.GetFixedDiscountRows() == null)
            {
                return;
            }
            FormatRowLists(_order.GetFixedDiscountRows());
        }

        private void FormatRelativeDiscountRows()
        {
            if (_order.GetRelativeDiscountRows() == null)
            {
                return;
            }
            FormatRowLists(_order.GetRelativeDiscountRows());
        }

        private static OrderRow SerializeOrder(string articleNumber, string description, string name, string unit, OrderRow orderRow)
        {
            orderRow.ArticleNumber = articleNumber;

            if (name != null)
            {
                orderRow.Description = name + (description == null ? "" : ": " + description);
            }
            else
            {
                orderRow.Description = description ?? "";
            }

            orderRow.Unit = unit;

            return orderRow;
        }

        private OrderRow SerializeAmountAndVat(decimal? amountExVat, decimal? vatPercent, decimal? amountIncVat, OrderRow orderRow, bool useIncVatSpecification)
        {

            if (vatPercent != null && amountExVat != null)
            {
                orderRow.PricePerUnit = MathUtil.BankersRound(amountExVat.GetValueOrDefault());
                orderRow.VatPercent = vatPercent.GetValueOrDefault();
            }
            else if (vatPercent != null && amountIncVat != null)
            {
                var value = useIncVatSpecification 
                    ? amountIncVat.GetValueOrDefault() 
                    : amountIncVat.GetValueOrDefault()/((0.01M*vatPercent.GetValueOrDefault()) + 1);

                orderRow.PricePerUnit = MathUtil.BankersRound(value);
                orderRow.VatPercent = vatPercent.GetValueOrDefault();
                orderRow.PriceIncludingVat = useIncVatSpecification;
            }
            else if (amountExVat != null && amountIncVat != null)
            {
                var amount = useIncVatSpecification 
                    ? amountIncVat.GetValueOrDefault() 
                    : amountExVat.GetValueOrDefault();
                orderRow.PricePerUnit = MathUtil.BankersRound(amount);
                orderRow.VatPercent = ((amountIncVat.GetValueOrDefault() / amountExVat.GetValueOrDefault()) - 1) * 100;
                orderRow.PriceIncludingVat = useIncVatSpecification;
            }
            return orderRow;
        }
    }
}