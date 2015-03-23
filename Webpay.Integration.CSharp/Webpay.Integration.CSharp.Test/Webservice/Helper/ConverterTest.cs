using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Webservice.Helper;

namespace Webpay.Integration.CSharp.Test.Webservice.Helper
{
    [TestFixture]
    public class ConverterTest
    {
        [Test]
        public void TestConvertOrderBuilderToOrder()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            var order = WebServiceRowFormatter<CreateOrderBuilder>.ConvertToOrder(orderBuilder);
            Assert.That(order.Original, Is.SameAs(orderBuilder));
        }

        [Test]
        public void TestCheckIfRowsIncVatForSingleRowExVat()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountExVat(12.23M));
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            var resultOrder = WebServiceRowFormatter<CreateOrderBuilder>.CheckIfRowsIncVat(order);

            Assert.That(resultOrder.AllPricesAreSpecifiedIncVat, Is.False);
        }

        [Test]
        public void TestCheckIfRowsIncVatForSingleRowIncVat()
        {
            var orderBuilder = new CreateOrderBuilder(SveaConfig.GetDefaultConfig());
            orderBuilder.AddOrderRow(Item.OrderRow().SetAmountIncVat(12.23M));
            var order = new WebServiceRowFormatter<CreateOrderBuilder>.Order(orderBuilder);
            var resultOrder = WebServiceRowFormatter<CreateOrderBuilder>.CheckIfRowsIncVat(order);

            Assert.That(resultOrder.AllPricesAreSpecifiedIncVat, Is.True);
        }

        [Test]
        public void TestCheckIfRowsIncVatForMixedRows()
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
            Assert.That(result.NewOrderRows[0].GetVatPercent(), Is.EqualTo(25));
            Assert.That(result.NewOrderRows[1].GetVatPercent(), Is.EqualTo(10));
            Assert.That(result.NewOrderRows[0].GetAmountExVat(), Is.EqualTo(100M));
            Assert.That(result.NewOrderRows[0].GetAmountIncVat(), Is.EqualTo(125M));
            Assert.That(result.NewOrderRows[1].GetAmountExVat(), Is.EqualTo(200M));
            Assert.That(result.NewOrderRows[1].GetAmountIncVat(), Is.EqualTo(220M));
        }

        [Test]
        public void FillMissingValuesGivenEitherIncOrExVatPriceIsZeroThenVatPercentIsZero()
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(0)
                                                                        .SetAmountIncVat(125.00M))
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(100M)
                                                                        .SetAmountIncVat(0));
            var result = WebServiceRowFormatter<CreateOrderBuilder>.FillMissingValues(new WebServiceRowFormatter<CreateOrderBuilder>.Order(order));
            Assert.That(result.NewOrderRows[0].GetVatPercent(), Is.EqualTo(0));
            Assert.That(result.NewOrderRows[1].GetVatPercent(), Is.EqualTo(0));
        }


    }
}