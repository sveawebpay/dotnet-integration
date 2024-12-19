using Webpay.Integration.Config;
using Webpay.Integration.Exception;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Order.Validator;
using Webpay.Integration.Test.Hosted.Payment;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.Test.Order.Validator;

[TestFixture]
public class HostedOrderValidatorTest
{
    private OrderValidator _orderValidator;

    [SetUp]
    public void SetUp()
    {
        _orderValidator = new HostedOrderValidator();
    }

    [Test]
    public void TestFailOnNullClientOrderNumber()
    {
        const string expectedMessage =
            "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
            "MISSING VALUE - ClientOrderNumber is required. Use SetClientOrderNumber(...).\n" +
            "MISSING VALUE - Currency is required. Use SetCurrency(...).\n" +
            "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n";

        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                     .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
                                     .SetValidator(new VoidValidator())
                                     .Build();

        Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
    }

    [Test]
    public void TestFailOnEmptyClientOrderNumber()
    {
        const string expectedMessage =
            "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
            "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n";

        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                     .AddOrderRow(Item.OrderRow()
                                                      .SetQuantity(1)
                                                      .SetAmountExVat(100)
                                                      .SetVatPercent(25))
                                     .SetCurrency(TestingTool.DefaultTestCurrency)
                                     .SetClientOrderNumber("")
                                     .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
                                     .SetValidator(new VoidValidator())
                                     .Build();

        _orderValidator = new HostedOrderValidator();
        Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
    }

    [Test]
    public void TestFailOnMissingReturnUrl()
    {
        const string expectedMessage = "MISSING VALUE - Return url is required, SetReturnUrl(...).\n";

        var exception = Assert.Throws<SveaWebPayValidationException>(() =>
        {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                         .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                         .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                         .SetCurrency(TestingTool.DefaultTestCurrency)
                                         .AddOrderRow(Item.OrderRow()
                                                          .SetAmountExVat(4)
                                                          .SetVatPercent(25)
                                                          .SetQuantity(1))
                                         .AddFee(Item.ShippingFee())
                                         .AddDiscount(Item.FixedDiscount())
                                         .AddDiscount(Item.RelativeDiscount());

            var payment = new FakeHostedPayment(order);
            payment.CalculateRequestValues();
        });

        Assert.That(exception.Message, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void TestSucceedOnGoodValuesSe()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                     .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                     .SetCurrency(TestingTool.DefaultTestCurrency)
                                     .SetValidator(new VoidValidator())
                                     .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                     .AddOrderRow(Item.OrderRow()
                                                      .SetAmountExVat(5.0M)
                                                      .SetVatPercent(25)
                                                      .SetQuantity(1))
                                     .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer());

        _orderValidator = new HostedOrderValidator();

        Assert.That(_orderValidator.Validate(order), Is.Empty);
    }

    [Test]
    public void TestValidateNlCustomerIdentity()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                     .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                     .SetCountryCode(CountryCode.NL)
                                     .SetCurrency(Currency.EUR)
                                     .AddOrderRow(Item.OrderRow()
                                                      .SetAmountExVat(5.0M)
                                                      .SetVatPercent(25)
                                                      .SetQuantity(1))
                                     .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer());

        _orderValidator = new HostedOrderValidator();

        Assert.That(_orderValidator.Validate(order), Is.Empty);
    }

    [Test]
    public void TestValidateDeCustomerIdentity()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                     .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                     .SetCountryCode(CountryCode.DE)
                                     .SetCurrency(Currency.EUR)
                                     .AddOrderRow(Item.OrderRow()
                                                      .SetAmountExVat(5.0M)
                                                      .SetVatPercent(25)
                                                      .SetQuantity(1))
                                     .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer());

        _orderValidator = new HostedOrderValidator();

        Assert.That(_orderValidator.Validate(order), Is.Empty);
    }

    [Test]
    public void TestFailVatPercentIsMissing()
    {
        const string expectedMessage =
            "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
            "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n" +
            "MISSING VALUE - At least one of the values must be set in combination with AmountExVat: AmountIncVat or VatPercent for Orderrow. Use one of: SetAmountIncVat() or SetVatPercent().\n";

        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetQuantity(1)
                                                     .SetAmountExVat(100))
                                    .SetCurrency(TestingTool.DefaultTestCurrency)
                                    .SetClientOrderNumber("")
                                    .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
                                    .SetValidator(new VoidValidator())
                                    .Build();

        _orderValidator = new HostedOrderValidator();

        Assert.That(expectedMessage, Is.EqualTo(_orderValidator.Validate(order)));
    }

    [Test]
    public void TestFailAmountExVatIsMissing()
    {
        const string expectedMessage =
            "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
            "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n" +
            "MISSING VALUE - At least one of the values must be set in combination with VatPercent: AmountIncVat or AmountExVat for Orderrow. Use one of: SetAmountExVat() or SetAmountIncVat().\n";

        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetQuantity(1)
                                                     .SetVatPercent(25))
                                    .SetCurrency(TestingTool.DefaultTestCurrency)
                                    .SetClientOrderNumber("")
                                    .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
                                    .SetValidator(new VoidValidator())
                                    .Build();

        _orderValidator = new HostedOrderValidator();

        Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
    }

    [Test]
    public void TestFailAmountExVatAndVatPercentIsMissing()
    {
        const string expectedMessage =
            "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
            "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n" +
            "MISSING VALUE - At least one of the values must be set in combination with AmountIncVat: AmountExVat or VatPercent for Orderrow. Use one of: SetAmountExVat() or SetVatPercent().\n";

        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetQuantity(1)
                                                     .SetAmountIncVat(125))
                                    .SetCurrency(TestingTool.DefaultTestCurrency)
                                    .SetClientOrderNumber("")
                                    .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
                                    .SetValidator(new VoidValidator())
                                    .Build();

        _orderValidator = new HostedOrderValidator();

        Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
    }

    [Test]
    public void TestValidateFailOrderIsNull()
    {
        const string expectedMessage =
            "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
            "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n" +
            "MISSING VALUES - AmountExVat, Quantity and VatPercent are required for Orderrow. Use SetAmountExVat(), SetQuantity() and SetVatPercent().\n";

        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .AddOrderRow(null)
                                    .SetCurrency(TestingTool.DefaultTestCurrency)
                                    .SetClientOrderNumber("")
                                    .AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer())
                                    .SetValidator(new VoidValidator())
                                    .Build();

        _orderValidator = new HostedOrderValidator();

        Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
    }

    [Test]
    public void TestFailMissingIdentityInHostedNl()
    {
        const string expectedMessage =
            "MISSING VALUE - Initials is required for individual customers when countrycode is NL. Use SetInitials().\n" +
            "MISSING VALUE - Birth date is required for individual customers when countrycode is NL. Use SetBirthDate().\n" +
            "MISSING VALUE - Name is required for individual customers when countrycode is NL. Use SetName().\n" +
            "MISSING VALUE - Street address and house number is required for all customers when countrycode is NL. Use SetStreetAddress().\n" +
            "MISSING VALUE - Locality is required for all customers when countrycode is NL. Use SetLocality().\n" +
            "MISSING VALUE - Zip code is required for all customers when countrycode is NL. Use SetZipCode().\n";

        var exception = Assert.Throws<SveaWebPayValidationException>(() =>
            WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                            .AddDiscount(TestingTool.CreateRelativeDiscount())
                            .AddCustomerDetails(Item.IndividualCustomer())
                            .SetCountryCode(CountryCode.NL)
                            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                            .SetOrderDate(TestingTool.DefaultTestDate)
                            .SetCurrency(TestingTool.DefaultTestCurrency)
                            .UsePaymentMethod(PaymentMethod.INVOICE)
                            .SetReturnUrl("http://myurl.se")
                            .GetPaymentForm());

        Assert.That(exception.Message, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void TestFailMissingIdentityInHostedDe()
    {
        const string expectedMessage =
            "MISSING VALUE - Birth date is required for individual customers when countrycode is DE. Use SetBirthDate().\n" +
            "MISSING VALUE - Name is required for individual customers when countrycode is DE. Use SetName().\n" +
            "MISSING VALUE - Street address is required for all customers when countrycode is DE. Use SetStreetAddress().\n" +
            "MISSING VALUE - Locality is required for all customers when countrycode is DE. Use SetLocality().\n" +
            "MISSING VALUE - Zip code is required for all customers when countrycode is DE. Use SetCustomerZipCode().\n";

        var exception = Assert.Throws<SveaWebPayValidationException>(() =>
            WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                            .AddDiscount(TestingTool.CreateRelativeDiscount())
                            .AddCustomerDetails(Item.IndividualCustomer())
                            .SetCountryCode(CountryCode.DE)
                            .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                            .SetOrderDate(TestingTool.DefaultTestDate)
                            .SetCurrency(TestingTool.DefaultTestCurrency)
                            .UsePaymentMethod(PaymentMethod.INVOICE)
                            .SetReturnUrl("http://myurl.se")
                            .GetPaymentForm());

        Assert.That(exception.Message, Is.EqualTo(expectedMessage));
    }
}