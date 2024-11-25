using Webpay.Integration.Config;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using WebpayWS;
using OrderType = WebpayWS.OrderType;

namespace Webpay.Integration.Test.Webservice.Payment;

[TestFixture]
public class InvoicePaymentTest
{
    [Test]
    public void TestInvoiceRequestObjectWithPeppolId()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber)
                .SetIpAddress("123.123.123"))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetPeppolId("1234:asdf")
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.PeppolId, Is.EqualTo("1234:asdf"));
    }

    [Test]
    public void TestInvoiceRequestObjectForCustomerIdentityIndividualFromSe()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.CustomerIdentity.NationalIdNumber, Is.EqualTo(TestingTool.DefaultTestIndividualNationalIdNumber));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CountryCode, Is.EqualTo(CountryCode.SE.ToString().ToUpper()));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Individual));
    }

    [Test]
    public void TestInvoiceRequestObjectWithAuth()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddFee(TestingTool.CreateExVatBasedShippingFee())
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.Auth.Username, Is.EqualTo("sverigetest"));
        Assert.That(request.Auth.Password, Is.EqualTo("sverigetest"));
        Assert.That(request.Auth.ClientNumber, Is.EqualTo(79021));
    }

    [Test]
    public void TestSetAuth()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.Auth.ClientNumber, Is.EqualTo(79021));
        Assert.That(request.Auth.Username, Is.EqualTo("sverigetest"));
        Assert.That(request.Auth.Password, Is.EqualTo("sverigetest"));
    }

    [Test]
    public void TestInvoiceRequestObjectForCustomerIdentityIndividualFromNl()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer(CountryCode.NL))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .SetCountryCode(CountryCode.NL)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.CustomerIdentity.Email, Is.EqualTo("test@svea.com"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.PhoneNumber, Is.EqualTo("999999"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IpAddress, Is.EqualTo("123.123.123.123"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.FullName, Is.EqualTo("Sneider Boasman"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.Street, Is.EqualTo("Gate"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CoAddress, Is.EqualTo("138"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.ZipCode, Is.EqualTo("1102 HG"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.HouseNumber, Is.EqualTo("42"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.Locality, Is.EqualTo("BARENDRECHT"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CountryCode, Is.EqualTo(CountryCode.NL.ToString().ToUpper()));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Individual));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.FirstName, Is.EqualTo("Sneider"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.LastName, Is.EqualTo("Boasman"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.Initials, Is.EqualTo("SB"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.BirthDate, Is.EqualTo("19550307"));
    }

    [Test]
    public void TestCompanyIdRequest()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetNationalIdNumber("4354kj"))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.Auth.ClientNumber, Is.EqualTo(79021));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.NationalIdNumber, Is.EqualTo("4354kj"));
    }

    [Test]
    public void TestInvoiceRequestObjectForCustomerIdentityCompanyFromNl()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddCustomerDetails(TestingTool.CreateCompanyCustomer(CountryCode.NL))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .SetCountryCode(CountryCode.NL)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.CustomerIdentity.Email, Is.EqualTo("test@svea.com"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.PhoneNumber, Is.EqualTo("999999"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.IpAddress, Is.EqualTo("123.123.123.123"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.FullName, Is.EqualTo("Svea bakkerij 123"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.Street, Is.EqualTo("broodstraat"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CoAddress, Is.EqualTo("236"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.ZipCode, Is.EqualTo("1111 CD"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.HouseNumber, Is.EqualTo("1"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.Locality, Is.EqualTo("BARENDRECHT"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CountryCode, Is.EqualTo(CountryCode.NL.ToString().ToUpper()));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
    }

    [Test]
    public void TestInvoiceRequestObjectForCustomerIdentityCompanyFromSe()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.CustomerIdentity.NationalIdNumber, Is.EqualTo("2345234"));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CountryCode, Is.EqualTo(CountryCode.SE.ToString().ToUpper()));
        Assert.That(request.CreateOrderInformation.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
    }

    [Test]
    public void TestInvoiceRequestObjectForSEOrderOnOneProductRow()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
            .AddFee(TestingTool.CreateExVatBasedShippingFee())
            .AddFee(TestingTool.CreateExVatBasedInvoiceFee())
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.OrderRows[0].ArticleNumber, Is.EqualTo("1"));
        Assert.That(request.CreateOrderInformation.OrderRows[0].Description, Is.EqualTo("Prod: Specification"));
        Assert.That(request.CreateOrderInformation.OrderRows[0].PricePerUnit, Is.EqualTo(100.00));
        Assert.That(request.CreateOrderInformation.OrderRows[0].NumberOfUnits, Is.EqualTo(2));
        Assert.That(request.CreateOrderInformation.OrderRows[0].Unit, Is.EqualTo("st"));
        Assert.That(request.CreateOrderInformation.OrderRows[0].VatPercent, Is.EqualTo(25));
        Assert.That(request.CreateOrderInformation.OrderRows[0].DiscountPercent, Is.EqualTo(0));

        Assert.That(request.CreateOrderInformation.OrderRows[1].ArticleNumber, Is.EqualTo("33"));
        Assert.That(request.CreateOrderInformation.OrderRows[1].Description, Is.EqualTo("shipping: Specification"));
        Assert.That(request.CreateOrderInformation.OrderRows[1].PricePerUnit, Is.EqualTo(50));
        Assert.That(request.CreateOrderInformation.OrderRows[1].NumberOfUnits, Is.EqualTo(1));
        Assert.That(request.CreateOrderInformation.OrderRows[1].Unit, Is.EqualTo("st"));
        Assert.That(request.CreateOrderInformation.OrderRows[1].VatPercent, Is.EqualTo(25));
        Assert.That(request.CreateOrderInformation.OrderRows[1].DiscountPercent, Is.EqualTo(0));

        Assert.That(request.CreateOrderInformation.OrderRows[2].ArticleNumber, Is.EqualTo(""));
        Assert.That(request.CreateOrderInformation.OrderRows[2].Description, Is.EqualTo("Svea fee: Fee for invoice"));
        Assert.That(request.CreateOrderInformation.OrderRows[2].PricePerUnit, Is.EqualTo(50));
        Assert.That(request.CreateOrderInformation.OrderRows[2].NumberOfUnits, Is.EqualTo(1));
        Assert.That(request.CreateOrderInformation.OrderRows[2].Unit, Is.EqualTo("st"));
        Assert.That(request.CreateOrderInformation.OrderRows[2].VatPercent, Is.EqualTo(25));
        Assert.That(request.CreateOrderInformation.OrderRows[2].DiscountPercent, Is.EqualTo(0));
    }

    [Test]
    public void TestInvoiceRequestObjectWithRelativeDiscountOnDifferentProductVat()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(1)
                .SetAmountExVat(240.00M)
                .SetDescription("CD")
                .SetVatPercent(25))
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(1)
                .SetAmountExVat(188.68M)
                .SetDescription("Bok")
                .SetVatPercent(6))
            .AddDiscount(Item.RelativeDiscount()
                .SetDiscountId("1")
                .SetDiscountPercent(20)
                .SetDescription("RelativeDiscount"))
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.OrderRows[2].ArticleNumber, Is.EqualTo("1"));
        Assert.That(request.CreateOrderInformation.OrderRows[2].Description, Is.EqualTo("RelativeDiscount (25%)"));
        Assert.That(request.CreateOrderInformation.OrderRows[3].Description, Is.EqualTo("RelativeDiscount (6%)"));

        var combinedPrice = request.CreateOrderInformation.OrderRows[2].PricePerUnit +
                            request.CreateOrderInformation.OrderRows[3].PricePerUnit;
        Assert.That(combinedPrice, Is.EqualTo(-85.74));

        Assert.That(request.CreateOrderInformation.OrderRows[2].NumberOfUnits, Is.EqualTo(1));
        Assert.That(request.CreateOrderInformation.OrderRows[2].VatPercent, Is.EqualTo(25));
        Assert.That(request.CreateOrderInformation.OrderRows[3].VatPercent, Is.EqualTo(6));
    }

    [Test]
    public void TestInvoiceRequestObjectWithFixedDiscountOnDifferentProductVat()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(1)
                .SetAmountExVat(240.00M)
                .SetDescription("CD")
                .SetVatPercent(25))
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(1)
                .SetAmountExVat(188.68M)
                .SetDescription("Bok")
                .SetVatPercent(6))
            .AddDiscount(Item.FixedDiscount()
                .SetDiscountId("1")
                .SetAmountIncVat(100.00M)
                .SetDescription("FixedDiscount"))
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.OrderRows[2].ArticleNumber, Is.EqualTo("1"));
        Assert.That(request.CreateOrderInformation.OrderRows[2].Description, Is.EqualTo("FixedDiscount (25%)"));
        Assert.That(request.CreateOrderInformation.OrderRows[3].Description, Is.EqualTo("FixedDiscount (6%)"));

        var combinedPrice = request.CreateOrderInformation.OrderRows[2].PricePerUnit +
                            request.CreateOrderInformation.OrderRows[3].PricePerUnit;
        Assert.That(combinedPrice, Is.EqualTo(-85.74));

        Assert.That(request.CreateOrderInformation.OrderRows[2].NumberOfUnits, Is.EqualTo(1));
        Assert.That(request.CreateOrderInformation.OrderRows[2].VatPercent, Is.EqualTo(25));
        Assert.That(request.CreateOrderInformation.OrderRows[3].VatPercent, Is.EqualTo(6));
        Assert.That(request.CreateOrderInformation.OrderRows[2].DiscountPercent, Is.EqualTo(0));
    }

    [Test]
    public void TestInvoiceWithFixedDiscountWithUnevenAmount()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(Item.OrderRow()
                .SetArticleNumber("1")
                .SetQuantity(1)
                .SetAmountExVat(240.00M)
                .SetDescription("CD")
                .SetVatPercent(25))
            .AddDiscount(Item.FixedDiscount()
                .SetAmountIncVat(101.50M)
                .SetDescription("FixedDiscount")
                .SetDiscountId("1"))
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        var discountRow = request.CreateOrderInformation.OrderRows[1];
        Assert.That(discountRow.ArticleNumber, Is.EqualTo("1"));
        Assert.That(discountRow.Description, Is.EqualTo("FixedDiscount (25%)"));
        Assert.That(discountRow.PricePerUnit, Is.EqualTo(-81.2));
        Assert.That(discountRow.NumberOfUnits, Is.EqualTo(1));
        Assert.That(discountRow.Unit, Is.EqualTo(""));
        Assert.That(discountRow.VatPercent, Is.EqualTo(25));
        Assert.That(discountRow.DiscountPercent, Is.EqualTo(0));
    }

    [Test]
    public void TestInvoiceRequestObjectWithCreateOrderInformation()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
            .AddFee(TestingTool.CreateExVatBasedShippingFee())
            .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber)
                .SetAddressSelector("ad33")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("999999")
                .SetIpAddress("123.123.123")
                .SetStreetAddress("Gatan", "23")
                .SetCoAddress("c/o Eriksson")
                .SetZipCode("2222")
                .SetLocality("Stan"))
            .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        Assert.That(request.CreateOrderInformation.OrderDate, Is.EqualTo(TestingTool.DefaultTestDate));
        Assert.That(request.CreateOrderInformation.ClientOrderNumber, Is.EqualTo("33"));
        Assert.That(request.CreateOrderInformation.OrderType, Is.EqualTo(OrderType.Invoice));
        Assert.That(request.CreateOrderInformation.CustomerReference, Is.EqualTo("ref33"));
        Assert.That(request.CreateOrderInformation.AddressSelector, Is.EqualTo("ad33"));
    }

    [Test]
    public void TestInvoiceRequestUsingAmountIncVatWithVatPercent()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddCustomerDetails(Item.CompanyCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber)
                .SetAddressSelector("ad33")
                .SetEmail("test@svea.com")
                .SetPhoneNumber("999999")
                .SetIpAddress("123.123.123")
                .SetStreetAddress("Gatan", "23")
                .SetCoAddress("c/o Eriksson")
                .SetZipCode("2222")
                .SetLocality("Stan"))
            .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow())
            .AddFee(TestingTool.CreateIncVatBasedShippingFee())
            .AddFee(TestingTool.CreateIncVatBasedInvoiceFee())
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        var orderRow1 = request.CreateOrderInformation.OrderRows[0];
        Assert.That(orderRow1.ArticleNumber, Is.EqualTo("1"));
        Assert.That(orderRow1.Description, Is.EqualTo("Prod: Specification"));
        Assert.That(orderRow1.PricePerUnit, Is.EqualTo(125.00M));
        Assert.That(orderRow1.NumberOfUnits, Is.EqualTo(2));
        Assert.That(orderRow1.Unit, Is.EqualTo("st"));
        Assert.That(orderRow1.VatPercent, Is.EqualTo(25));
        Assert.That(orderRow1.DiscountPercent, Is.EqualTo(0));
        Assert.That(orderRow1.PriceIncludingVat, Is.True);

        var orderRow2 = request.CreateOrderInformation.OrderRows[1];
        Assert.That(orderRow2.ArticleNumber, Is.EqualTo("33"));
        Assert.That(orderRow2.Description, Is.EqualTo("shipping: Specification"));
        Assert.That(orderRow2.PricePerUnit, Is.EqualTo(62.50M));
        Assert.That(orderRow2.NumberOfUnits, Is.EqualTo(1));
        Assert.That(orderRow2.Unit, Is.EqualTo("st"));
        Assert.That(orderRow2.VatPercent, Is.EqualTo(25));
        Assert.That(orderRow2.DiscountPercent, Is.EqualTo(0));
        Assert.That(orderRow2.PriceIncludingVat, Is.True);

        var orderRow3 = request.CreateOrderInformation.OrderRows[2];
        Assert.That(orderRow3.ArticleNumber, Is.EqualTo(""));
        Assert.That(orderRow3.Description, Is.EqualTo("Svea fee: Fee for invoice"));
        Assert.That(orderRow3.PricePerUnit, Is.EqualTo(62.50M));
        Assert.That(orderRow3.NumberOfUnits, Is.EqualTo(1));
        Assert.That(orderRow3.Unit, Is.EqualTo("st"));
        Assert.That(orderRow3.VatPercent, Is.EqualTo(25));
        Assert.That(orderRow3.DiscountPercent, Is.EqualTo(0));
        Assert.That(orderRow3.PriceIncludingVat, Is.True);
    }

    [Test]
    public void TestInvoiceRequestUsingAmountIncVatWithAmountExVat()
    {
        var request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateIncAndExVatOrderRow())
            .AddFee(TestingTool.CreateIncAndExVatShippingFee())
            .AddFee(TestingTool.CreateIncAndExVatInvoiceFee())
            .AddCustomerDetails(Item.IndividualCustomer()
                .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .SetOrderDate(TestingTool.DefaultTestDate)
            .SetCurrency(TestingTool.DefaultTestCurrency)
            .UseInvoicePayment()
            .PrepareRequest();

        var orderRow1 = request.CreateOrderInformation.OrderRows[0];
        Assert.That(orderRow1.ArticleNumber, Is.EqualTo("1"));
        Assert.That(orderRow1.Description, Is.EqualTo("Prod: Specification"));
        Assert.That(orderRow1.PricePerUnit, Is.EqualTo(125.00M));
        Assert.That(orderRow1.NumberOfUnits, Is.EqualTo(2));
        Assert.That(orderRow1.Unit, Is.EqualTo("st"));
        Assert.That(orderRow1.VatPercent, Is.EqualTo(25));
        Assert.That(orderRow1.DiscountPercent, Is.EqualTo(0));

        var orderRow2 = request.CreateOrderInformation.OrderRows[1];
        Assert.That(orderRow2.ArticleNumber, Is.EqualTo("33"));
        Assert.That(orderRow2.Description, Is.EqualTo("shipping: Specification"));
        Assert.That(orderRow2.PricePerUnit, Is.EqualTo(62.5M));
        Assert.That(orderRow2.NumberOfUnits, Is.EqualTo(1));
        Assert.That(orderRow2.Unit, Is.EqualTo("st"));
        Assert.That(orderRow2.VatPercent, Is.EqualTo(25));
        Assert.That(orderRow2.DiscountPercent, Is.EqualTo(0));

        var orderRow3 = request.CreateOrderInformation.OrderRows[2];
        Assert.That(orderRow3.ArticleNumber, Is.EqualTo(""));
        Assert.That(orderRow3.Description, Is.EqualTo("Svea fee: Fee for invoice"));
        Assert.That(orderRow3.PricePerUnit, Is.EqualTo(62.5M));
        Assert.That(orderRow3.NumberOfUnits, Is.EqualTo(1));
        Assert.That(orderRow3.Unit, Is.EqualTo("st"));
        Assert.That(orderRow3.VatPercent, Is.EqualTo(25));
        Assert.That(orderRow3.DiscountPercent, Is.EqualTo(0));
    }
}