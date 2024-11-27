using Webpay.Integration.Config;
using Webpay.Integration.IntegrationTest.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using WebpayWS;

namespace Webpay.Integration.IntegrationTest.Webservice.CreateOrder;

[TestFixture]
public class CreateOrderTest
{
    [Test]
    public async Task TestConfiguration()
    {
        var conf = new ConfigurationProviderTestData();
        var response = await WebpayConnection.CreateOrder(conf)
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber)
                .SetIpAddress("123.123.123"))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequestAsync();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
    }

    [Test]
    public async Task TestFormatShippingFeeRowsZero()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddFee(Item.ShippingFee()
                .SetShippingId("0")
                .SetName("Tess")
                .SetDescription("Tester")
                .SetAmountExVat(0)
                .SetVatPercent(0)
                .SetUnit("st"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .UseInvoicePayment()
            .DoRequestAsync();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
    }

    [Test]
    public async Task TestCompanyIdResponse()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequestAsync();

        Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
        Assert.That(response.CreateOrderResult.CustomerIdentity.IndividualIdentity == null);
        Assert.That(response.CreateOrderResult.CustomerIdentity.CompanyIdentity == null);
        Assert.That(response.Accepted, Is.True);
    }

    [Test, Ignore("Credit score is broken for Germany in stage")]
    public async Task TestDeCompanyIdentity()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateOrderRowDe())
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetNationalIdNumber("12345")
                .SetVatNumber("DE123456789")
                .SetStreetAddress("Adalbertsteinweg", "1")
                .SetZipCode("52070")
                .SetLocality("AACHEN"))
            .SetCountryCode(CountryCode.DE)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(Currency.EUR)
            .UseInvoicePayment()
            .DoRequestAsync();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
        Assert.That(response.Accepted, Is.True);
    }

    [Test, Ignore("Credit score is broken for Netherlands in stage")]
    public async Task TestNlCompanyIdentity()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateOrderRowNl())
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetCompanyName("Svea bakkerij 123")
                .SetVatNumber("NL123456789A12")
                .SetStreetAddress("broodstraat", "1")
                .SetZipCode("1111 CD")
                .SetLocality("BARENDRECHT"))
            .SetCountryCode(CountryCode.NL)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(Currency.EUR)
            .UseInvoicePayment()
            .DoRequestAsync();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
        Assert.That(response.Accepted, Is.True);
    }

    [Test]
    public async Task Test_CreateOrder_SE_WithOnlyNationalIdNumber_ShouldNotSetIndividualIdentity()
    {
        var createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency);

        var request = createOrderBuilder.UseInvoicePayment().PrepareRequest();
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity == null);

        // Execute the order asynchronously and check response
        var order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();
        Assert.That(order.Accepted);
    }

    [Test]
    public async Task Test_CreateOrder_NO_WithOnlyNationalIdNumber_ShouldNotSetIndividualIdentity()
    {
        var createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber("17054512066"))    // NO test individual "Ola Norrmann"
            .SetCountryCode(CountryCode.NO) // NO
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber("33308")  // NO Invoice
            .SetCurrency(TestingTool.DefaultTestCurrency);

        var request = createOrderBuilder.UseInvoicePayment().PrepareRequest();
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity == null);

        var order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();
        Assert.That(order.Accepted);
    }

    [Test]
    public async Task Test_CreateOrder_NO_WithAllCustomerDetailsSet_ShouldNotSetIndividualIdentity()
    {
        var createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber("17054512066") // NO test individual "Ola Norrmann"
                .SetBirthDate("19460509")
                .SetName("Tess", "Testson")
                .SetInitials("SB")
                .SetStreetAddress("Gatan", "23")
                .SetCoAddress("c/o Eriksson")
                .SetZipCode("9999")
                .SetLocality("Stan")
                .SetPhoneNumber("999999")
                .SetEmail("test@svea.com")
                .SetIpAddress("123.123.123"))
            .SetCountryCode(CountryCode.NO) // NO
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber("33308") // NO Invoice
            .SetCurrency(TestingTool.DefaultTestCurrency);

        var request = createOrderBuilder.UseInvoicePayment().PrepareRequest();
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity == null);

        var order = await createOrderBuilder.UseInvoicePayment().DoRequestAsync();
        Assert.That(order.Accepted);
    }
}
