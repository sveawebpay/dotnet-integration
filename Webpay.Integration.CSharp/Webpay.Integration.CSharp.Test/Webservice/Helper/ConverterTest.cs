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
            var resultOrder = WebServiceRowFormatter<CreateOrderBuilder>.CheckIfRowsIncVat(order);

            Assert.That(resultOrder.AllPricesAreSpecifiedIncVat, Is.False);
        }

        [Test]
        public void AllPricesAreSpecifiedIncVatForSingleRowIncVat()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountIncVat(12.23M));
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            var resultOrder = WebServiceRowFormatter<CreateOrderBuilder>.CheckIfRowsIncVat(order);

            Assert.That(resultOrder.AllPricesAreSpecifiedIncVat, Is.True);
        }

        [Test]
        public void AllPricesAreSpecifiedIncVatForMixedRows()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountIncVat(12.23M));
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountExVat(12.23M));
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            var resultOrder = WebServiceRowFormatter<CreateOrderBuilder>.CheckIfRowsIncVat(order);

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