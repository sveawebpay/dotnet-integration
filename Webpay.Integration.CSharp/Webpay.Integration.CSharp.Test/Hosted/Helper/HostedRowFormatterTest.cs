using System.Collections.Generic;
using NUnit.Framework;
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
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            Assert.AreEqual("0", newRow.GetSku());
            Assert.AreEqual("Tess", newRow.GetName());
            Assert.AreEqual("Tester", newRow.GetDescription());
            Assert.AreEqual(500L, newRow.GetAmount());
            Assert.AreEqual(100, newRow.GetVat());
            Assert.AreEqual(1, newRow.GetQuantity());
            Assert.AreEqual("st", newRow.GetUnit());
        }

        [Test]
        public void TestFormatShippingFeeRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddFee(Item.ShippingFee()
                                                                   .SetShippingId("0")
                                                                   .SetName("Tess")
                                                                   .SetDescription("Tester")
                                                                   .SetUnit("st"));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.AreEqual("0", newRow.GetSku());
            Assert.AreEqual("Tess", newRow.GetName());
            Assert.AreEqual("Tester", newRow.GetDescription());
            Assert.AreEqual(1, newRow.GetQuantity());
            Assert.AreEqual("st", newRow.GetUnit());
        }

        [Test]
        public void TestFormatShippingFeeRowsVat()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddFee(Item.ShippingFee()
                                                                   .SetAmountExVat(4)
                                                                   .SetVatPercent(25));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.AreEqual(500L, newRow.GetAmount());
            Assert.AreEqual(100L, newRow.GetVat());
        }

        [Test]
        public void TestFormatFixedDiscountRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetUnit("st"));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.AreEqual("0", newRow.GetSku());
            Assert.AreEqual("Tess", newRow.GetName());
            Assert.AreEqual("Tester", newRow.GetDescription());
            Assert.AreEqual(1, newRow.GetQuantity());
            Assert.AreEqual("st", newRow.GetUnit());
        }

        [Test]
        public void TestFormatFixedDiscountRowsAmount()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetAmountIncVat(4));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.AreEqual(-400L, newRow.GetAmount());
        }

        [Test]
        public void TestFormatFixedDiscountRowsVat()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(-100L, newRow.GetAmount());
            Assert.AreEqual(-20L, newRow.GetVat());
        }

        [Test]
        public void TestFormatRelativeDiscountRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetUnit("st"));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[0];

            Assert.AreEqual("0", newRow.GetSku());
            Assert.AreEqual("Tess", newRow.GetName());
            Assert.AreEqual("Tester", newRow.GetDescription());
            Assert.AreEqual(1, newRow.GetQuantity());
            Assert.AreEqual("st", newRow.GetUnit());
        }

        [Test]
        public void TestFormatRelativeDiscountRowsVat()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountPercent(10));

            List<HostedOrderRowBuilder> newRows = new HostedRowFormatter<CreateOrderBuilder>().FormatRows(order);
            HostedOrderRowBuilder newRow = newRows[1];

            Assert.AreEqual(-50L, newRow.GetAmount());
            Assert.AreEqual(-10L, newRow.GetVat());
        }

        [Test]
        public void TestFormatTotalAmount()
        {
            var row = new HostedOrderRowBuilder()
                .SetAmount(100L)
                .SetQuantity(2);
            var rows = new List<HostedOrderRowBuilder> {row};

            Assert.AreEqual(200L, new HostedRowFormatter<CreateOrderBuilder>().FormatTotalAmount(rows));
        }

        [Test]
        public void TestFormatTotalVat()
        {
            var row = new HostedOrderRowBuilder()
                .SetVat(100L)
                .SetQuantity(2);
            var rows = new List<HostedOrderRowBuilder> {row};

            Assert.AreEqual(200L, new HostedRowFormatter<CreateOrderBuilder>().FormatTotalVat(rows));
        }

        [Test]
        public void TestFormatTotalVatNegative()
        {
            var row = new HostedOrderRowBuilder()
                .SetVat(-100L)
                .SetQuantity(2);
            var rows = new List<HostedOrderRowBuilder> {row};

            Assert.AreEqual(-200L, new HostedRowFormatter<CreateOrderBuilder>().FormatTotalVat(rows));
        }
    }
}