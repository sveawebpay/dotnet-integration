using Webpay.Integration.Config;
using Webpay.Integration.Hosted.Helper;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Order.Row;

namespace Webpay.Integration.Test.Hosted.Helper;

[TestFixture]
public class HostedRowFormatterTest
{
    [Test]
    public void TestFormatOrderRows()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(
                Item.OrderRow()
                    .SetArticleNumber("0")
                    .SetName("Tess")
                    .SetDescription("Tester")
                    .SetAmountExVat(4)
                    .SetVatPercent(25)
                    .SetQuantity(1)
                    .SetUnit("st")
            );

        var newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
        var newRow = newRows[0];

        Assert.That(newRow.GetSku(), Is.EqualTo("0"));
        Assert.That(newRow.GetName(), Is.EqualTo("Tess"));
        Assert.That(newRow.GetDescription(), Is.EqualTo("Tester"));
        Assert.That(newRow.GetAmount(), Is.EqualTo(500L));
        Assert.That(newRow.GetVat(), Is.EqualTo(100));
        Assert.That(newRow.GetQuantity(), Is.EqualTo(1));
        Assert.That(newRow.GetUnit(), Is.EqualTo("st"));
    }

    [Test]
    public void TestFormatShippingFeeRows()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddFee(
                Item.ShippingFee()
                    .SetShippingId("0")
                    .SetName("Tess")
                    .SetDescription("Tester")
                    .SetUnit("st")
            );

        var newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
        var newRow = newRows[0];

        Assert.That(newRow.GetSku(), Is.EqualTo("0"));
        Assert.That(newRow.GetName(), Is.EqualTo("Tess"));
        Assert.That(newRow.GetDescription(), Is.EqualTo("Tester"));
        Assert.That(newRow.GetQuantity(), Is.EqualTo(1));
        Assert.That(newRow.GetUnit(), Is.EqualTo("st"));
    }

    [Test]
    public void TestFormatShippingFeeRowsVat()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddFee(
                Item.ShippingFee()
                    .SetAmountExVat(4)
                    .SetVatPercent(25)
            );

        var newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
        var newRow = newRows[0];

        Assert.That(newRow.GetAmount(), Is.EqualTo(500L));
        Assert.That(newRow.GetVat(), Is.EqualTo(100L));
    }

    [Test]
    public void TestFormatFixedDiscountRows()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddDiscount(
                Item.FixedDiscount()
                    .SetDiscountId("0")
                    .SetName("Tess")
                    .SetDescription("Tester")
                    .SetUnit("st")
            );

        var newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
        var newRow = newRows[0];

        Assert.That(newRow.GetSku(), Is.EqualTo("0"));
        Assert.That(newRow.GetName(), Is.EqualTo("Tess"));
        Assert.That(newRow.GetDescription(), Is.EqualTo("Tester"));
        Assert.That(newRow.GetQuantity(), Is.EqualTo(1));
        Assert.That(newRow.GetUnit(), Is.EqualTo("st"));
    }

    [Test]
    public void TestFormatFixedDiscountRowsAmount()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddDiscount(
                Item.FixedDiscount()
                    .SetAmountIncVat(4)
            );

        var newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
        var newRow = newRows[0];

        Assert.That(newRow.GetAmount(), Is.EqualTo(-400L));
    }

    [Test]
    public void TestFormatFixedDiscountRowsVat()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountExVat(4)
                    .SetVatPercent(25)
                    .SetQuantity(1)
            )
            .AddDiscount(
                Item.FixedDiscount()
                    .SetAmountIncVat(1)
                    .SetDiscountId("0")
                    .SetName("Tess")
                    .SetDescription("Tester")
            );

        var newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
        var newRow = newRows[1];

        Assert.That(newRow.GetAmount(), Is.EqualTo(-100L));
        Assert.That(newRow.GetVat(), Is.EqualTo(-20L));
    }

    [Test]
    public void TestFormatRelativeDiscountRows()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddDiscount(
                Item.RelativeDiscount()
                    .SetDiscountId("0")
                    .SetName("Tess")
                    .SetDescription("Tester")
                    .SetUnit("st")
            );

        var newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
        var newRow = newRows[0];

        Assert.That(newRow.GetSku(), Is.EqualTo("0"));
        Assert.That(newRow.GetName(), Is.EqualTo("Tess"));
        Assert.That(newRow.GetDescription(), Is.EqualTo("Tester"));
        Assert.That(newRow.GetQuantity(), Is.EqualTo(1));
        Assert.That(newRow.GetUnit(), Is.EqualTo("st"));
    }

    [Test]
    public void TestFormatRelativeDiscountRowsVat()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountExVat(4)
                    .SetVatPercent(25)
                    .SetQuantity(1)
            )
            .AddDiscount(
                Item.RelativeDiscount()
                    .SetDiscountPercent(10)
            );

        var newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
        var newRow = newRows[1];

        Assert.That(newRow.GetAmount(), Is.EqualTo(-50L));
        Assert.That(newRow.GetVat(), Is.EqualTo(-10L));
    }

    [Test]
    public void TestGetTotalAmount()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountIncVat(100L)
                    .SetVatPercent(25)
                    .SetQuantity(2)
            );

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        _ = formatter.FormatRows(order);

        Assert.That(formatter.GetTotalAmount(), Is.EqualTo(20000L));
    }

    [Test]
    public void TestGetTotalVat()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountIncVat(100L)
                    .SetVatPercent(25)
                    .SetQuantity(2)
            );

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        _ = formatter.FormatRows(order);

        Assert.That(formatter.GetTotalVat(), Is.EqualTo(4000L));
    }

    [Test]
    public void TestGetTotalVatNegative()
    {
        var order = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountIncVat(-100L)
                    .SetVatPercent(25)
                    .SetQuantity(2)
            );

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        _ = formatter.FormatRows(order);

        Assert.That(formatter.GetTotalVat(), Is.EqualTo(-4000L));
    }
}