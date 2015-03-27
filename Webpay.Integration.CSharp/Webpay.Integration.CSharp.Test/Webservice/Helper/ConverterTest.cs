using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Webservice.Helper;

namespace Webpay.Integration.CSharp.Test.Webservice.Helper
{
    [TestFixture]
    public class ConverterTest
    {
        [Test]
        public void ConvertOrderBuilderToOrder()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            var order = WebServiceRowFormatter<CreateOrderBuilder>.ConvertToOrder(orderBuilder);
            Assert.That(order.Original, Is.SameAs(orderBuilder));
        }

        [Test]
        public void AllPricesAreSpecifiedIncVatForSingleRowExVat()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountExVat(12.23M));
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            var resultOrder = WebServiceRowFormatter<CreateOrderBuilder>.CheckIfRowsIncVat(true)(order);

            Assert.That(resultOrder.AllPricesAreSpecifiedIncVat, Is.False);
        }

        [Test]
        public void AllPricesAreSpecifiedIncVatForSingleRowIncVat()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountIncVat(12.23M));
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            var resultOrder = WebServiceRowFormatter<CreateOrderBuilder>.CheckIfRowsIncVat(true)(order);

            Assert.That(resultOrder.AllPricesAreSpecifiedIncVat, Is.True);
        }

        [Test]
        public void AllPricesAreSpecifiedIncVatForMixedRows()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountIncVat(12.23M));
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountExVat(12.23M));
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            var resultOrder = WebServiceRowFormatter<CreateOrderBuilder>.CheckIfRowsIncVat(true)(order);

            Assert.That(resultOrder.AllPricesAreSpecifiedIncVat, Is.False);
        }

        [Test]
        public void FillMissingValuesGivenIncAndExVat()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(100.00M)
                                                                        .SetAmountIncVat(125.00M))
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(200.00M)
                                                                        .SetAmountIncVat(220.00M));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
            AssertVat(result, 0, 25);
            AssertVat(result, 1, 10);
            AssertAmountEx(result, 0, 100M);
            AssertAmountInc(result, 0, 125M);
            AssertAmountEx(result, 1, 200M);
            AssertAmountInc(result, 1, 220M);
        }

        [Test]
        public void FillMissingValuesGivenIncAndExVatBothZero()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(0)
                                                                        .SetAmountIncVat(0));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
            AssertVat(result, 0, 0);
            AssertAmountEx(result, 0, 0);
            AssertAmountInc(result, 0, 0);
        }


        [Test]
        public void FillMissingValuesGivenIncAndVat()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(125.00M)
                                                                        .SetVatPercent(25))
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(220.00M)
                                                                        .SetVatPercent(10));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
            AssertVat(result, 0, 25);
            AssertVat(result, 1, 10);
            AssertAmountEx(result, 0, 100M);
            AssertAmountInc(result, 0, 125M);
            AssertAmountEx(result, 1, 200M);
            AssertAmountInc(result, 1, 220M);
        }

        [Test]
        public void FillMissingValuesExVatIncVatAndVatGivenConsistently()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(125.00M)
                                                                        .SetAmountExVat(100.00M)
                                                                        .SetVatPercent(25));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
            AssertVat(result, 0, 25);
            AssertAmountEx(result, 0, 100M);
            AssertAmountInc(result, 0, 125M);
        }

        [Test]
        public void FillMissingValuesGivenExAndVat()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(100.00M)
                                                                        .SetVatPercent(25))
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(200.00M)
                                                                        .SetVatPercent(10));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
            AssertVat(result, 0, 25);
            AssertVat(result, 1, 10);
            AssertAmountEx(result, 0, 100M);
            AssertAmountInc(result, 0, 125M);
            AssertAmountEx(result, 1, 200M);
            AssertAmountInc(result, 1, 220M);
        }


        [Test, ExpectedException(typeof (SveaWebPayValidationException))]
        public void FillMissingValuesInconsistentThrowsIncZeroExSomething()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(0)
                                                                        .SetAmountExVat(125.00M));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
        }

        [Test, ExpectedException(typeof(SveaWebPayValidationException))]
        public void FillMissingValuesInconsistentThrowsExZeroIncSomething()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(10M)
                                                                        .SetAmountExVat(0));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
        }

        [Test, ExpectedException(typeof(SveaWebPayValidationException))]
        public void FillMissingValuesInconsistentThrowsOnlyAmountIncVat()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(10M));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
        }

        [Test, ExpectedException(typeof(SveaWebPayValidationException))]
        public void FillMissingValuesInconsistentThrowsOnlyAmountExVat()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(10M));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
        }

        [Test, ExpectedException(typeof(SveaWebPayValidationException))]
        public void FillMissingValuesInconsistentThrowsOnlyVat()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetVatPercent(10));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
        }

        [Test, ExpectedException(typeof(SveaWebPayValidationException))]
        public void FillMissingValuesAllValuesSetButInconsistently()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetVatPercent(10)
                                                                        .SetAmountExVat(33)
                                                                        .SetAmountIncVat(110));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
        }


        [Test]
        public void SumByVatSingleRow()
        {
            var orderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig());
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            order.NewOrderRows.Add(Item.OrderRow()
                                    .SetVatPercent(25)
                                    .SetQuantity(1)
                                    .SetAmountExVat(100)
                                    .SetAmountIncVat(125));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.CalculateTotals(order);
            Assert.That(result.TotalAmountExVat, Is.EqualTo(100));
            Assert.That(result.TotalAmountIncVat, Is.EqualTo(125));
            Assert.That(result.TotalVatAsAmount, Is.EqualTo(25));
            Assert.That(result.TotalAmountPerVatRateIncVat[25], Is.EqualTo(125));
            Assert.That(result.TotalAmountPerVatRateIncVat.Keys.Count, Is.EqualTo(1));
        }

        [Test]
        public void SumByVatMultipleRowsSameVat()
        {
            var orderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig());
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            order.NewOrderRows.Add(Item.OrderRow()
                                    .SetVatPercent(25)
                                    .SetQuantity(1)
                                    .SetAmountExVat(100)
                                    .SetAmountIncVat(125));
            order.NewOrderRows.Add(Item.OrderRow()
                                    .SetVatPercent(25)
                                    .SetQuantity(1)
                                    .SetAmountExVat(200)
                                    .SetAmountIncVat(250));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.CalculateTotals(order);
            Assert.That(result.TotalAmountExVat, Is.EqualTo(300));
            Assert.That(result.TotalAmountIncVat, Is.EqualTo(375));
            Assert.That(result.TotalVatAsAmount, Is.EqualTo(75));
            Assert.That(result.TotalAmountPerVatRateIncVat[25], Is.EqualTo(375));
            Assert.That(result.TotalAmountPerVatRateIncVat.Keys.Count, Is.EqualTo(1));
        }


        [Test]
        public void SumByVatMultipleRowsDifferentVat()
        {
            var orderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig());
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            order.NewOrderRows.Add(Item.OrderRow()
                                    .SetVatPercent(25)
                                    .SetQuantity(1)
                                    .SetAmountExVat(100)
                                    .SetAmountIncVat(125));
            order.NewOrderRows.Add(Item.OrderRow()
                                    .SetVatPercent(10)
                                    .SetQuantity(1)
                                    .SetAmountExVat(200)
                                    .SetAmountIncVat(220));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.CalculateTotals(order);
            Assert.That(result.TotalAmountExVat, Is.EqualTo(300));
            Assert.That(result.TotalAmountIncVat, Is.EqualTo(345));
            Assert.That(result.TotalVatAsAmount, Is.EqualTo(45));
            Assert.That(result.TotalAmountPerVatRateIncVat[25], Is.EqualTo(125));
            Assert.That(result.TotalAmountPerVatRateIncVat[10], Is.EqualTo(220));
            Assert.That(result.TotalAmountPerVatRateIncVat.Keys.Count, Is.EqualTo(2));
        }

        [Test]
        public void SumByVatMultipleRowsDifferentVatDifferentQuantity()
        {
            var orderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig());
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            order.NewOrderRows.Add(Item.OrderRow()
                                    .SetVatPercent(25)
                                    .SetQuantity(2)
                                    .SetAmountExVat(100)
                                    .SetAmountIncVat(125));
            order.NewOrderRows.Add(Item.OrderRow()
                                    .SetVatPercent(10)
                                    .SetQuantity(3)
                                    .SetAmountExVat(200)
                                    .SetAmountIncVat(220));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.CalculateTotals(order);
            Assert.That(result.TotalAmountExVat, Is.EqualTo(2*100 + 3*200));
            Assert.That(result.TotalAmountIncVat, Is.EqualTo(2*125 + 3*220));
            Assert.That(result.TotalVatAsAmount, Is.EqualTo(2*25 + 3*20));
            Assert.That(result.TotalAmountPerVatRateIncVat[25], Is.EqualTo(2 * 125));
            Assert.That(result.TotalAmountPerVatRateIncVat[10], Is.EqualTo(3 * 220));
            Assert.That(result.TotalAmountPerVatRateIncVat.Keys.Count, Is.EqualTo(2));
        }


        private void AssertAmountInc(WebServiceRowFormatter<CreateOrderBuilder>.Order result, int item, decimal amount)
        {
            Assert.That(result.NewOrderRows[item].GetAmountIncVat(), Is.EqualTo(amount));
        }

        private void AssertAmountEx(WebServiceRowFormatter<CreateOrderBuilder>.Order result, int item, decimal amount)
        {
            Assert.That(result.NewOrderRows[item].GetAmountExVat(), Is.EqualTo(amount));
        }

        private void AssertVat(WebServiceRowFormatter<CreateOrderBuilder>.Order result, int item, int vat)
        {
            Assert.That(result.NewOrderRows[item].GetVatPercent(), Is.EqualTo(vat));
        }




    }
}