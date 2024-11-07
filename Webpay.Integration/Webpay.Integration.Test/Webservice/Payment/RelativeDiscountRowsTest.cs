using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Config;

namespace Webpay.Integration.Test.Webservice.Payment;

[TestFixture]
public class RelativeDiscountRowsTest
{
    [Test]
    public async Task TestAmountExVatWithRelativeDiscount()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item.OrderRow()
                .SetQuantity(1)
                .SetAmountExVat(100.00M)
                .SetVatPercent(25.00M)
            )
            .AddDiscount(Item.RelativeDiscount()
                .SetDiscountPercent(50))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber("4605092222"))
            .SetCountryCode(CountryCode.SE)
            .SetCustomerReference("dotnet-integration")
            .SetClientOrderNumber("RelativeDiscountExVat")
            .SetOrderDate(new DateTime(2012, 12, 12))
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(62.5M));
    }

    [Test]
    public async Task TestAmountIncVatWithRelativeDiscount()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item.OrderRow()
                .SetQuantity(1)
                .SetAmountIncVat(100.00M)
                .SetVatPercent(25.00M)
            )
            .AddDiscount(Item.RelativeDiscount()
                .SetDiscountPercent(50))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber("4605092222"))
            .SetCountryCode(CountryCode.SE)
            .SetCustomerReference("dotnet-integration")
            .SetClientOrderNumber("RelativeDiscountIncVat")
            .SetOrderDate(new DateTime(2012, 12, 12))
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(50.0M));
    }
}
