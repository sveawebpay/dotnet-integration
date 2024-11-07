using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using WebpayWS;

namespace Webpay.Integration.IntegrationTest.Webservice.Payment;

[TestFixture]
public class CreateInvoiceOrderTest
{
    [Test]
    public async Task TestInvoiceDoRequestWithPeppolId()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetPeppolId("1234:asdf")
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
    }

    [Test]
    public async Task TestInvoiceDoRequestWithIpAddressSetSe()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber)
                .SetIpAddress("123.123.123"))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
    }

    [Test]
    public async Task TestInvoiceRequestUsingAmountIncVatWithZeroVatPercent()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
    }

    [Test]
    public async Task TestInvoiceRequestFailing()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(Item.IndividualCustomer().SetNationalIdNumber(""))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.Accepted, Is.False);
    }

    [Test]
    public async Task TestCalculationWithTwelvePercentVat()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(1)
                .SetAmountExVat(45M)
                .SetDescription("Specification")
                .SetName("Prod")
                .SetVatPercent(12M)
                .SetDiscountPercent(5))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.Accepted, Is.True);
        Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(47.88m));
    }

    [Test]
    public async Task TestCalculationWithSixPercentVat()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(2)
                .SetAmountExVat(117M)
                .SetDescription("Specification")
                .SetName("Prod")
                .SetVatPercent(6M)
                .SetDiscountPercent(0))
            .AddCustomerDetails(
                Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.Accepted, Is.True);
        Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(248.04));
    }

    [Test]
    public async Task TestInvoiceRequestUsingForNewVatFunctionCornerCase()
    {
        var useInvoicePayment = WebpayConnection
            .CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item
                .OrderRow()
                .SetArticleNumber("1")
                .SetName("Prod")
                .SetDescription("Specification")
                .SetAmountIncVat(100.00M)
                .SetQuantity(1)
                .SetUnit("st")
                .SetVatPercent(24)
                .SetVatDiscount(0))
            .AddCustomerDetails(Item
                .IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .UseInvoicePayment();

        var createOrderEuRequest = useInvoicePayment.PrepareRequest();
        var response = await useInvoicePayment.DoRequest();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
        Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(100.00M));
    }

    [Test]
    public async Task TestInvoiceForIndividualFromSe()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(
                Item.IndividualCustomer().SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.Accepted);
        Assert.That(response.CreateOrderResult.SveaWillBuyOrder);
        Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(250.00));
        Assert.That(response.CreateOrderResult.OrderType, Is.EqualTo("Invoice"));

        // CustomerIdentity            
        Assert.That(response.CreateOrderResult.CustomerIdentity.NationalIdNumber, Is.EqualTo("194605092222"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.CountryCode, Is.EqualTo("SE"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Individual));
        Assert.That(response.CreateOrderResult.CustomerIdentity.FullName, Is.EqualTo("Persson, Tess T"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.Street, Is.EqualTo("Testgatan 1"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.CoAddress, Is.EqualTo("c/o Eriksson, Erik"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.ZipCode, Is.EqualTo("99999"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.Locality, Is.EqualTo("Stan"));
    }

    [Test]
    public async Task TestInvoiceCompanySe()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber))
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(2)
                .SetAmountExVat(100.00M)
                .SetDescription("Specification")
                .SetName("Prod")
                .SetVatPercent(25)
                .SetDiscountPercent(0))
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
        Assert.That(response.CreateOrderResult.SveaWillBuyOrder, Is.True);
        Assert.That(response.CreateOrderResult.CustomerIdentity.CountryCode, Is.EqualTo("SE"));
    }

    [Test, Ignore("Credit score is broken for Netherlands in stage")]
    public async Task TestInvoiceForIndividualFromNl()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateOrderRowNl())
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer(CountryCode.NL))
            .SetCountryCode(CountryCode.NL)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(Currency.EUR)
            .UseInvoicePayment()
            .DoRequest();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
        Assert.That(response.CreateOrderResult.SveaWillBuyOrder, Is.True);
        Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(212.00));
        Assert.That(response.CreateOrderResult.OrderType, Is.EqualTo("Invoice"));

        Assert.That(response.CreateOrderResult.CustomerIdentity.Email, Is.EqualTo("test@svea.com"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.IpAddress, Is.Null);
        Assert.That(response.CreateOrderResult.CustomerIdentity.CountryCode, Is.EqualTo("NL"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.HouseNumber, Is.EqualTo("42"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Individual));
        Assert.That(response.CreateOrderResult.CustomerIdentity.PhoneNumber, Is.EqualTo("999999"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.FullName, Is.EqualTo("Sneider Boasman"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.Street, Is.EqualTo("Gate"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.CoAddress, Is.Null);
        Assert.That(response.CreateOrderResult.CustomerIdentity.ZipCode, Is.EqualTo("1102 HG"));
        Assert.That(response.CreateOrderResult.CustomerIdentity.Locality, Is.EqualTo("BARENDRECHT"));
    }

    [Test]
    public async Task TestFormatShippingFeeRowsZero()
    {
        var response = await WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(2)
                .SetAmountExVat(10)
                .SetDescription("Specification")
                .SetName("Prod")
                .SetVatPercent(0)
                .SetDiscountPercent(0))
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
            .DoRequest();

        Assert.That(response.ResultCode, Is.EqualTo(0));
        Assert.That(response.Accepted, Is.True);
    }
}