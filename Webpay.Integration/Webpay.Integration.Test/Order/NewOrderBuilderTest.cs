using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Testing;
using WebpayWS;

namespace Webpay.Integration.Test.Order;

[TestFixture]
public class NewOrderBuilderTest
{
    [Test]
    public void TestBuildOrderRowList()
    {
        var orderRows = new List<OrderRowBuilder>
            {
                TestingTool.CreateExVatBasedOrderRow("1"),
                TestingTool.CreateExVatBasedOrderRow("2")
            };

        CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRows(orderRows)
            .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.CustomerIdentity.NationalIdNumber, Is.EqualTo("194608142222"));
        Assert.That(request.CreateOrderInformation.OrderRows[0].ArticleNumber, Is.EqualTo("1"));
        Assert.That(request.CreateOrderInformation.OrderRows[1].ArticleNumber, Is.EqualTo("2"));
    }

    [Test]
    public void TestBuildOrderWithCompanyCustomer()
    {
        CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.CustomerIdentity.NationalIdNumber, Is.EqualTo("194608142222"));
    }
}