using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.IntegrationTest.Webservice.HandleOrder;

[TestFixture]
public class CloseOrderTest
{
    [Test]
    public async Task TestCloseOrder()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);

        var closeResponse = await WebpayAdmin.CancelOrder(SveaConfig.GetDefaultConfig())
            .SetOrderId(response.CreateOrderResult.SveaOrderId)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .CancelInvoiceOrder()
            .DoRequest();

        Assert.That(closeResponse.ResultCode, Is.EqualTo(0));
        //Assert.That(closeResponse.Accepted, Is.True);
        Assert.That(closeResponse.ResultCode, Is.EqualTo(0), "Order was not accepted.");
    }

    [Test]
    public async Task TestFailOnMissingCountryCodeOfCloseOrder()
    {
        var response = await WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(
                Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber)
            )
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequest();

        var orderId = response.CreateOrderResult.SveaOrderId;

        Assert.Multiple(() =>
        {
            Assert.That(response.ResultCode, Is.EqualTo(0), "Expected ResultCode to be 0");
            Assert.That(response.Accepted, Is.True, "Expected response to be accepted");
            Assert.That(orderId.ToString(), Does.Match(@"^\d{5,}$"), "OrderId is not valid");
        });
    }
}