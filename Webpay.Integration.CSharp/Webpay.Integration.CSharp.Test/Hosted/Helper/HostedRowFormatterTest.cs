using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Hosted;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;

namespace Webpay.Integration.CSharp.Test.Hosted.Helper
{
    [TestFixture]
    public class HostedRowFormatterTest
    {
        [Test]
        public void TestFormatOrderRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetArticleNumber("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1)
                                                                        .SetUnit("st"));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

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
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddFee(Item.ShippingFee()
                                                                   .SetShippingId("0")
                                                                   .SetName("Tess")
                                                                   .SetDescription("Tester")
                                                                   .SetUnit("st"));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.That(newRow.GetSku(), Is.EqualTo("0"));
            Assert.That(newRow.GetName(), Is.EqualTo("Tess"));
            Assert.That(newRow.GetDescription(), Is.EqualTo("Tester"));
            Assert.That(newRow.GetQuantity(), Is.EqualTo(1));
            Assert.That(newRow.GetUnit(), Is.EqualTo("st"));
        }

        [Test]
        public void TestFormatShippingFeeRowsVat()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddFee(Item.ShippingFee()
                                                                   .SetAmountExVat(4)
                                                                   .SetVatPercent(25));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.That(newRow.GetAmount(), Is.EqualTo(500L));
            Assert.That(newRow.GetVat(), Is.EqualTo(100L));
        }

        [Test]
        public void TestFormatFixedDiscountRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetUnit("st"));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.That(newRow.GetSku(), Is.EqualTo("0"));
            Assert.That(newRow.GetName(), Is.EqualTo("Tess"));
            Assert.That(newRow.GetDescription(), Is.EqualTo("Tester"));
            Assert.That(newRow.GetQuantity(), Is.EqualTo(1));
            Assert.That(newRow.GetUnit(), Is.EqualTo("st"));
        }

        [Test]
        public void TestFormatFixedDiscountRowsAmount()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetAmountIncVat(4));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.That(newRow.GetAmount(), Is.EqualTo(-400L));
        }

        [Test]
        public void TestFormatFixedDiscountRowsVat()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetAmountIncVat(1)
                                                                        .SetDiscountId("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester"));
            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[1];

            Assert.That(newRow.GetAmount(), Is.EqualTo(-100L));
            Assert.That(newRow.GetVat(), Is.EqualTo(-20L));
        }

        [Test]
        public void TestFormatRelativeDiscountRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetUnit("st"));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.That(newRow.GetSku(), Is.EqualTo("0"));
            Assert.That(newRow.GetName(), Is.EqualTo("Tess"));
            Assert.That(newRow.GetDescription(), Is.EqualTo("Tester"));
            Assert.That(newRow.GetQuantity(), Is.EqualTo(1));
            Assert.That(newRow.GetUnit(), Is.EqualTo("st"));
        }

        [Test]
        public void TestFormatRelativeDiscountRowsVat()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountPercent(10));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[1];

            Assert.That(newRow.GetAmount(), Is.EqualTo(-50L));
            Assert.That(newRow.GetVat(), Is.EqualTo(-10L));
        }

        [Test]
        public void TestFormatTotalAmount()
        {
            var row = new HostedOrderRowBuilder()
                .SetAmount(100L)
                .SetQuantity(2);
            var rows = new List<HostedOrderRowBuilder> {row};

            Assert.That(new HostedRowFormatter<CreateOrderBuilder>().FormatTotalAmount(rows), Is.EqualTo(200L));
        }

        [Test]
        public void TestFormatTotalVat()
        {
            var row = new HostedOrderRowBuilder()
                .SetVat(100L)
                .SetQuantity(2);
            var rows = new List<HostedOrderRowBuilder> {row};

            Assert.That(new HostedRowFormatter<CreateOrderBuilder>().FormatTotalVat(rows), Is.EqualTo(200L));
        }

        [Test]
        public void TestFormatTotalVatNegative()
        {
            var row = new HostedOrderRowBuilder()
                .SetVat(-100L)
                .SetQuantity(2);
            var rows = new List<HostedOrderRowBuilder> {row};

            Assert.That(new HostedRowFormatter<CreateOrderBuilder>().FormatTotalVat(rows), Is.EqualTo(-200L));
        }
    }
}