using Webpay.Integration.Config;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.IntegrationTest;

[TestFixture]
public class WebpayConnectionIntegrationTest
{
    [Test]
    public async Task Test_CreateOrder_CreatePaymentPlanOrder_WithAllExVatRows()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();

        Assert.That(order.Accepted);
    }

    [Test]
    public async Task Test_DeliverOrder_DeliverInvoiceOrder_WithAllIdenticalRows()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var builder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetNumberOfCreditDays(30)
            .SetCaptureDate(DateTime.Now)
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"));

        var delivery = await builder.DeliverInvoiceOrder().DoRequestAsync();

        Assert.That(delivery.Accepted);
        Assert.That(delivery.DeliverOrderResult.Amount, Is.EqualTo(500.00M)); // 100ex@25%*2st *2rows
    }

    [Test]
    public async Task Test_DeliverOrder_DeliverInvoiceOrder_WithOneIdenticalRow()
    {
        var order = await TestingTool.CreateInvoiceOrderWithTwoOrderRows();

        var builder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetInvoiceDistributionType(DistributionType.POST)
            .SetNumberOfCreditDays(30)
            .SetCaptureDate(DateTime.Now)
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"));

        var delivery = await builder.DeliverInvoiceOrder().DoRequestAsync();

        Assert.That(delivery.Accepted);
        Assert.That(delivery.DeliverOrderResult.Amount, Is.EqualTo(250.00M)); // 100ex@25%*2st *1row
    }

    [Test]
    public async Task Test_DeliverOrder_DeliverPaymentPlanOrder_WithAllIdenticalRows()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();

        var builder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"));

        var delivery = await builder.DeliverPaymentPlanOrder().DoRequestAsync();

        Assert.That(delivery.Accepted);
        Assert.That(delivery.DeliverOrderResult.Amount, Is.EqualTo(5000.00M)); // 1000ex@25%*2st *2rows
    }

    [Test]
    public async Task Test_DeliverOrder_DeliverPaymentPlanOrder_IgnoresOrderRows()
    {
        var order = await TestingTool.CreatePaymentPlanOrderWithTwoOrderRows();

        var builder = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(order.CreateOrderResult.SveaOrderId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"));

        var delivery = await builder.DeliverPaymentPlanOrder().DoRequestAsync();

        Assert.That(delivery.Accepted);
        Assert.That(delivery.DeliverOrderResult.Amount, Is.EqualTo(5000.00M)); // 1000ex@25%*2st *2row
    }
}
