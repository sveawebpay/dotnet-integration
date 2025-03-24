using Webpay.Integration.Exception;
using Webpay.Integration.Order;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Calculation;
using WebpayWS;

namespace Webpay.Integration.Webservice.Helper;

public class WebServiceRowFormatter<T>
{
    private readonly OrderBuilder<T> _order;
    private readonly bool _useIncVatRequestIfPossible;

    public WebServiceRowFormatter(OrderBuilder<T> order) : this(order, true)
    {
    }

    public WebServiceRowFormatter(OrderBuilder<T> order, bool useIncVatRequestIfPossible)
    {
        _order = order;
        _useIncVatRequestIfPossible = useIncVatRequestIfPossible;
    }

    public class Order
    {
        public decimal TotalAmountIncVat { get; set; }
        public decimal TotalAmountExVat { get; set; }
        public decimal TotalVatAsAmount { get; set; }
        public SortedDictionary<decimal, decimal> TotalAmountPerVatRateIncVat { get; private set; }
        public bool AllPricesAreSpecifiedIncVat { get; set; }
        public OrderBuilder<T> Original { get; private set; }
        public OrderRequest WsOrderRequest { get; private set; }
        public List<OrderRowBuilder> NewOrderRows { get; private set; }
        public List<ShippingFeeBuilder> NewShippingFeeRows { get; private set; }
        public List<InvoiceFeeBuilder> NewInvoiceFeeRows { get; private set; }
        public List<OrderRowBuilder> NewFixedDiscountRows { get; private set; }
        public List<OrderRowBuilder> NewRelativeDiscountRows { get; private set; }

        public Order(OrderBuilder<T> original)
        {
            Original = original;
            NewOrderRows = new List<OrderRowBuilder>();
            NewShippingFeeRows = new List<ShippingFeeBuilder>();
            NewInvoiceFeeRows = new List<InvoiceFeeBuilder>();
            NewFixedDiscountRows = new List<OrderRowBuilder>();
            NewRelativeDiscountRows = new List<OrderRowBuilder>();
            TotalAmountPerVatRateIncVat = new SortedDictionary<decimal, decimal>(new InvertedComparator());
            WsOrderRequest = new OrderRequest();
            TotalAmountExVat = 0;
            TotalAmountIncVat = 0;
            TotalVatAsAmount = 0;
        }

        public class InvertedComparator : IComparer<decimal>
        {
            public int Compare(decimal x, decimal y)
            {
                return (int) (y - x);
            }
        }
    }

    public List<OrderRow> FormatRows()
    {
        return new Maybe<OrderBuilder<T>>(_order)
            .And(ConvertToOrder)
            .And(CheckIfRowsIncVat(_useIncVatRequestIfPossible))
            .And(FillMissingOrderRowValues)
            .And(CalculateOrderRowTotals)
            .And(ApplyRelativeDiscounts)
            .And(ApplyFixedDiscountsForOrderRowsNotSpecifyingVat)
            .And(ApplyFixedDiscountsForOrderRowsSpecifyingVat)
            .And(AddShippingFees)
            .And(AddInvoiceFees)
            .And(ConvertToWebserviceOrder)
            .Value;
    }

    private static Order AddShippingFees(Order order)
    {
        var processedShippingFeeRows = order.Original.GetShippingFeeRows().ConvertAll(shippingFeeRow =>
            {
                var newRow = Item
                    .ShippingFee()
                    .SetDescription(shippingFeeRow.GetDescription())
                    .SetShippingId(shippingFeeRow.GetShippingId())
                    .SetName(shippingFeeRow.GetName())
                    .SetUnit(shippingFeeRow.GetUnit());

                FillMissingVatAndAmount(shippingFeeRow, newRow);

                CheckConsistency(newRow);

                return newRow;
            });
        order.NewShippingFeeRows.AddRange(processedShippingFeeRows);
        return order;
    }

    private static Order AddInvoiceFees(Order order)
    {
        var processedInvoiceFeeRows = order.Original.GetInvoiceFeeRows().ConvertAll(shippingFeeRow =>
        {
            var newRow = Item
                .InvoiceFee()
                .SetDescription(shippingFeeRow.GetDescription())
                .SetName(shippingFeeRow.GetName())
                .SetUnit(shippingFeeRow.GetUnit());

            FillMissingVatAndAmount(shippingFeeRow, newRow);

            CheckConsistency(newRow);

            return newRow;
        });
        order.NewInvoiceFeeRows.AddRange(processedInvoiceFeeRows);
        return order;
    }

    public static Order ConvertToOrder(OrderBuilder<T> orderBuilder)
    {
        return new Order(orderBuilder);
    }

    public static Func<Order, Order> CheckIfRowsIncVat(bool useIncVatIfPossible)
    {
        return order =>
            {
                Func<IRowBuilder, bool> orderRowHasAmountIncVat = orderRow => orderRow.GetAmountIncVat() != null;

                order.AllPricesAreSpecifiedIncVat =
                    useIncVatIfPossible
                    &&
                    order.Original.GetOrderRows()
                        .All(orderRowHasAmountIncVat)
                    ;
                return order;
            };
    }

    public static Order FillMissingOrderRowValues(Order order)
    {
        var newRows = order.Original.GetOrderRows().ConvertAll(orderRow =>
            {
                var newRow = Item
                    .OrderRow()
                    .SetArticleNumber(orderRow.GetArticleNumber())
                    .SetDescription(orderRow.GetDescription())
                    .SetName(orderRow.GetName())
                    .SetQuantity(orderRow.GetQuantity())
                    .SetUnit(orderRow.GetUnit());

                if (orderRow.GetDiscountPercent() != 0)
                {
                    newRow.SetDiscountPercent((int) orderRow.GetDiscountPercent());
                }

                FillMissingVatAndAmount(orderRow, newRow);

                CheckConsistency(newRow);

                return newRow;
            });
        order.NewOrderRows.AddRange(newRows);
        return order;
    }

    private static void FillMissingVatAndAmount<TR>(IPriced<TR> orderRow, IPriced<TR> newRow)
    {
        if (orderRow.GetAmountExVat() != null && orderRow.GetAmountIncVat() != null &&
            orderRow.GetVatPercent() != null)
        {
            newRow.SetAmountExVat(orderRow.GetAmountExVat().GetValueOrDefault());
            newRow.SetAmountIncVat(orderRow.GetAmountIncVat().GetValueOrDefault());
            newRow.SetVatPercent(orderRow.GetVatPercent().GetValueOrDefault());
        }
        else if (orderRow.GetAmountIncVat() == 0 && orderRow.GetAmountExVat() > 0 ||
                 orderRow.GetAmountExVat() == 0 && orderRow.GetAmountIncVat() > 0)
        {
            throw new SveaWebPayValidationException(
                "Order is inconsistent. Amount excluding and including vat must either both be 0 or both be >0.");
        }
        else if (orderRow.GetAmountExVat() != null && orderRow.GetAmountIncVat() != null)
        {
            newRow.SetAmountExVat(orderRow.GetAmountExVat().GetValueOrDefault());
            newRow.SetAmountIncVat(orderRow.GetAmountIncVat().GetValueOrDefault());
            var vat = orderRow.GetAmountExVat() == 0 || orderRow.GetAmountIncVat() == 0
                          ? 0
                          : (orderRow.GetAmountIncVat() - orderRow.GetAmountExVat())/
                            orderRow.GetAmountExVat();
            var vatPercent = MathUtil.BankersRound((vat*100).GetValueOrDefault());
            newRow.SetVatPercent(vatPercent);
        }
        else if (orderRow.GetVatPercent() != null && orderRow.GetAmountIncVat() != null)
        {
            newRow.SetAmountIncVat(orderRow.GetAmountIncVat().GetValueOrDefault());
            newRow.SetVatPercent(orderRow.GetVatPercent().GetValueOrDefault());
            var exVatAmount = orderRow.GetAmountIncVat().GetValueOrDefault()*100/
                              (100 + orderRow.GetVatPercent().GetValueOrDefault());
            newRow.SetAmountExVat(exVatAmount);
        }
        else if (orderRow.GetVatPercent() != null && orderRow.GetAmountExVat() != null)
        {
            newRow.SetAmountExVat(orderRow.GetAmountExVat().GetValueOrDefault());
            newRow.SetVatPercent(orderRow.GetVatPercent().GetValueOrDefault());
            var incVatAmount = orderRow.GetAmountExVat().GetValueOrDefault()*
                               (100 + orderRow.GetVatPercent().GetValueOrDefault())/100;
            newRow.SetAmountIncVat(incVatAmount);
        }
        else
        {
            throw new SveaWebPayValidationException(
                "Order is inconsistent. You need to set at least two of SetAmountIncVat, SetAmountExVat and SetVatPercent. If you set all three, make sure their values are consistent.");
        }
    }

    private static void CheckConsistency<TR>(IPriced<TR> newRow)
    {
        if (newRow.GetAmountIncVat() - newRow.GetAmountExVat()*(100+newRow.GetVatPercent())/100 != 0)
        {
            throw new SveaWebPayValidationException(string.Format("Orderrow amounts and vat is inconsistent for row. Ex vat: {0} Inc vat: {1} Vat: {2}", newRow.GetAmountExVat(), newRow.GetAmountIncVat(), newRow.GetVatPercent()));
        }
    }

    public static Order CalculateOrderRowTotals(Order order)
    {
        order.NewOrderRows.ForEach(row=>order.TotalAmountPerVatRateIncVat[row.GetVatPercent().GetValueOrDefault()]=0);

        order.NewOrderRows.ForEach(row =>
            {
                order.TotalAmountExVat += row.GetAmountExVat().GetValueOrDefault() * row.GetQuantity();
                order.TotalAmountIncVat += row.GetAmountIncVat().GetValueOrDefault() * row.GetQuantity();
                order.TotalAmountPerVatRateIncVat[row.GetVatPercent().GetValueOrDefault()] +=
                    row.GetAmountIncVat().GetValueOrDefault() * row.GetQuantity();
            });

        order.TotalVatAsAmount = order.TotalAmountIncVat - order.TotalAmountExVat;

        return order;
    }

    public static Order ApplyFixedDiscountsForOrderRowsSpecifyingVat(Order order)
    {
        order.Original
             .GetFixedDiscountRows()
             .FindAll(discount => discount.GetVatPercent() != null)
             .ForEach(discount =>
             {
                 var orderRowBuilder = Item.OrderRow();
                 var newRow = ConvertFixedDiscountToOrderRow(orderRowBuilder, discount);

                 if (discount.GetAmountIncVat() != null)
                 {
                     newRow
                         .SetAmountIncVat(discount.GetAmountIncVat().GetValueOrDefault())
                         .SetAmountExVat(discount.GetAmountIncVat().GetValueOrDefault() * 100 / (100 + discount.GetVatPercent().GetValueOrDefault()))
                         .SetVatPercent(discount.GetVatPercent().GetValueOrDefault());
                 }
                 else if (discount.GetAmountExVat() != null)
                 {
                     newRow
                         .SetAmountExVat(discount.GetAmountExVat().GetValueOrDefault())
                         .SetAmountIncVat(discount.GetAmountExVat().GetValueOrDefault() * (100 + discount.GetVatPercent().GetValueOrDefault()) / 100)
                         .SetVatPercent(discount.GetVatPercent().GetValueOrDefault());
                 }
                 else
                 {
                     throw new SveaWebPayValidationException("A fixed discount cannot only contain vat but must also include the amount including vat, or the amount excluding vat.");
                 }
                 order.NewFixedDiscountRows.Add(newRow);

             });

        return order;
    }

    public static Order ApplyFixedDiscountsForOrderRowsNotSpecifyingVat(Order order)
    {
        order.Original
             .GetFixedDiscountRows()
             .FindAll(discount => discount.GetVatPercent() == null)
             .ForEach(discount =>
                 {
                     var discounts =  order.TotalAmountPerVatRateIncVat.Aggregate(new List<OrderRowBuilder>(), (res, vatAndAmount) =>
                         {
                             var byVatVatPercent = vatAndAmount.Key;
                             var byVatAmountIncVat = vatAndAmount.Value;
                             var orderRowBuilder = Item.OrderRow();
                             var newRow = ConvertFixedDiscountToOrderRow(orderRowBuilder, discount);

                             if (discount.GetAmountIncVat() != null)
                             {
                                 var discountAmountIncVat = discount.GetAmountIncVat().GetValueOrDefault() * byVatAmountIncVat / order.TotalAmountIncVat;
                                 var discountAmountExVat = discountAmountIncVat*100/(100 + byVatVatPercent);
                                 newRow
                                     .SetAmountIncVat(discountAmountIncVat)
                                     .SetVatPercent(byVatVatPercent)
                                     .SetAmountExVat(discountAmountExVat);
                                    
                             }
                             else if (discount.GetAmountExVat() != null)
                             {
                                 var exVatRatio = (byVatAmountIncVat * 100 / (100 + byVatVatPercent)) / order.TotalAmountExVat;
                                 var discountAmountExVat = discount.GetAmountExVat().GetValueOrDefault() * exVatRatio;

                                 newRow
                                     .SetAmountExVat(discountAmountExVat)
                                     .SetAmountIncVat(discountAmountExVat * (100 + byVatVatPercent) / 100)
                                     .SetVatPercent(byVatVatPercent);
                             }
                             else
                             {
                                 throw new SveaWebPayValidationException("A fixed discount the amount including vat, or the amount excluding vat, and optionally vat.");
                             }
                             res.Add(newRow);
                             return res;
                         });
                     order.NewFixedDiscountRows.AddRange(discounts);

             });

        return order;
    }

    private static OrderRowBuilder ConvertFixedDiscountToOrderRow(OrderRowBuilder orderRowBuilder, FixedDiscountBuilder discount)
    {
        var newRow = orderRowBuilder
            .SetName(discount.GetName())
            .SetDescription(string.Format("{0}", discount.GetDescription()))
            .SetUnit(discount.GetUnit())
            .SetQuantity(1)
            .SetArticleNumber(discount.GetDiscountId());
        return newRow;
    }

    public static Order ApplyRelativeDiscounts(Order order)
    {
        var relativeDiscounts = order.Original.GetRelativeDiscountRows()
            .Aggregate(new List<OrderRowBuilder>(), (allRelativeDiscounts, row) =>
                {
                    var discounts =
                        order.TotalAmountPerVatRateIncVat.Aggregate(
                            new List<OrderRowBuilder>(), (relativeDiscountsByVat, vatAndAmount) =>
                                {
                                    var byVatVatPercent = vatAndAmount.Key;
                                    var byVatAmountIncVat = vatAndAmount.Value;

                                    relativeDiscountsByVat.Add(Item.OrderRow()
                                        .SetArticleNumber(row.GetDiscountId())
                                        .SetDescription(row.GetDescription())
                                        .SetName(row.GetName())
                                        .SetQuantity(row.GetQuantity())
                                        .SetUnit(row.GetUnit())
                                        .SetAmountIncVat(byVatAmountIncVat* row.GetDiscountPercent() / 100)
                                        .SetAmountExVat(byVatAmountIncVat * (100 / (100 + byVatVatPercent)) * row.GetDiscountPercent() / 100)
                                        .SetVatPercent(byVatVatPercent));

                                    return relativeDiscountsByVat;

                                });
                    allRelativeDiscounts.AddRange(discounts);
                    return allRelativeDiscounts;
                });
        order.NewRelativeDiscountRows.AddRange(relativeDiscounts);
        return order;
    }

    public static List<OrderRow> ConvertToWebserviceOrder(Order order)
    {
        var res = order
            .NewOrderRows
            .ConvertAll(row =>
            {
                var wsRow = new OrderRow
                    {
                        NumberOfUnits = row.GetQuantity(),
                        ArticleNumber = row.GetArticleNumber(),
                        Description = FormatDescription(row),
                        PriceIncludingVat = order.AllPricesAreSpecifiedIncVat,
                        PricePerUnit = MathUtil.BankersRound(order.AllPricesAreSpecifiedIncVat ? row.GetAmountIncVat().GetValueOrDefault() : row.GetAmountExVat().GetValueOrDefault()),
                        DiscountPercent = row.GetDiscountPercent(),
                        Unit = row.GetUnit(),
                        VatPercent = row.GetVatPercent().GetValueOrDefault()
                    };
                return wsRow;
            });

        var shippingFees = order.NewShippingFeeRows.ConvertAll(row =>
        {
            var wsRow = new OrderRow
            {
                NumberOfUnits = row.GetQuantity(),
                ArticleNumber = row.GetArticleNumber(),
                Description = FormatDescription(row),
                PriceIncludingVat = order.AllPricesAreSpecifiedIncVat,
                PricePerUnit = MathUtil.BankersRound(order.AllPricesAreSpecifiedIncVat ? row.GetAmountIncVat().GetValueOrDefault() : row.GetAmountExVat().GetValueOrDefault()),
                DiscountPercent = row.GetDiscountPercent(),
                Unit = row.GetUnit(),
                VatPercent = row.GetVatPercent().GetValueOrDefault()
            };
            return wsRow;
        });
        res.AddRange(shippingFees);

        var invoiceFees = order.NewInvoiceFeeRows.ConvertAll(row =>
        {
            var wsRow = new OrderRow
            {
                NumberOfUnits = row.GetQuantity(),
                ArticleNumber = row.GetArticleNumber(),
                Description = FormatDescription(row),
                PriceIncludingVat = order.AllPricesAreSpecifiedIncVat,
                PricePerUnit = MathUtil.BankersRound(order.AllPricesAreSpecifiedIncVat ? row.GetAmountIncVat().GetValueOrDefault() : row.GetAmountExVat().GetValueOrDefault()),
                DiscountPercent = row.GetDiscountPercent(),
                Unit = row.GetUnit(),
                VatPercent = row.GetVatPercent().GetValueOrDefault(),
                RowType = RowType.InvoiceFee
            };
            return wsRow;
        });
        res.AddRange(invoiceFees);

        var discounts = order.NewFixedDiscountRows
            .Concat(order.NewRelativeDiscountRows)
            .ToList()
            .ConvertAll(row =>
            {
                var wsRow = new OrderRow
                {
                    NumberOfUnits = row.GetQuantity(),
                    ArticleNumber = row.GetArticleNumber(),
                    Description = FormatDiscountDescription(row),
                    PriceIncludingVat = order.AllPricesAreSpecifiedIncVat,
                    PricePerUnit = MathUtil.BankersRound( -(order.AllPricesAreSpecifiedIncVat ? row.GetAmountIncVat().GetValueOrDefault() : row.GetAmountExVat().GetValueOrDefault())),
                    DiscountPercent = row.GetDiscountPercent(),
                    Unit = row.GetUnit(),
                    VatPercent = row.GetVatPercent().GetValueOrDefault()
                };
                return wsRow;
            });
        res.AddRange(discounts);

        return res;
    }

    private static string FormatDescription(IRowBuilder row)
    {
        return string.Format(row.GetName() != null && row.GetName().Length > 0 ? "{0}: {1}" : "{1}", row.GetName(), row.GetDescription());
    }

    private static string FormatDiscountDescription(IRowBuilder row)
    {
        return string.Format(row.GetName() != null && row.GetName().Length > 0 ? "{0}: {1} ({2}%)" : "{1} ({2}%)", row.GetName(), row.GetDescription(), (int)row.GetVatPercent().GetValueOrDefault());
    }
}