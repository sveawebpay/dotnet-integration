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

    }
}