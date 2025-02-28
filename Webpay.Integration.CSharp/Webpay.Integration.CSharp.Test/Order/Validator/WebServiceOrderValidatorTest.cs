using System;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Validator;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.Test.Order.Validator
{
    [TestFixture]
    public class WebServiceOrderValidatorTest
    {
        private OrderValidator _orderValidator;

        [SetUp]
        public void SetUp()
        {
            _orderValidator = new WebServiceOrderValidator();
        }

        [Test]
        public void TestCheckOfIdentityClass()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode().\n" +
                                           "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                                           "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddCustomerDetails(TestingTool.CreateCompanyCustomer());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestCustomerIdentityIsNull()
        {
            const string expectedMessage = "MISSING VALUE - CustomerIdentity must be set.\n" +
                                           "MISSING VALUE - CountryCode is required. Use SetCountryCode().\n" +
                                           "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                                           "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .AddCustomerDetails(null);

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingCountryCodeOnCreateOrder()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode().\n" +
                                           "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                                           "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .AddCustomerDetails(Item.IndividualCustomer()
                                                                               .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber));

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingCountryCodeOnDeliverOrder()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).";

            var exception = Assert.Throws<SveaWebPayValidationException>(() => WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                                                               .AddOrderRow(
                                                                                                   TestingTool
                                                                                                       .CreateExVatBasedOrderRow())
                                                                                               .SetNumberOfCreditDays(1)
                                                                                               .SetOrderId(2345L)
                                                                                               .SetInvoiceDistributionType(DistributionType.POST)
                                                                                               .DeliverInvoiceOrder()
                                                                                               .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingValuesForGetAddresses()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n" +
                                           "MISSING VALUE - either nationalNumber or companyId is required. Use: SetCompany(...) or SetIndividual(...).\n";
            var exception =
                Assert.Throws<SveaWebPayValidationException>(() => WebpayConnection.GetAddresses(SveaConfig.GetDefaultConfig()).PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingCustomerIdentity()
        {
            const string expectedMessage =
                "MISSING VALUE - National number(ssn) is required for individual customers when countrycode is SE, NO, DK or FI. Use SetNationalIdNumber(...).\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddCustomerDetails(Item.IndividualCustomer())
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode);

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingOrderRows()
        {
            const string expectedMessage =
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .AddCustomerDetails(Item.IndividualCustomer()
                                                                               .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber));

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingOrderRowValues()
        {
            const string expectedMessage =
                "MISSING VALUE - Quantity is required in Item object. Use Item.SetQuantity().\n" +
                "MISSING VALUE - Two of the values must be set: AmountExVat(not set), AmountIncVat(not set) or VatPercent(not set) for Orderrow. Use two of: SetAmountExVat(), SetAmountIncVat or SetVatPercent().\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .AddOrderRow(Item.OrderRow())
                                                       .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .SetValidator(new VoidValidator());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingSsnForSeOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - National number(ssn) is required for individual customers when countrycode is SE, NO, DK or FI. Use SetNationalIdNumber(...).\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddCustomerDetails(Item.IndividualCustomer())
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode);

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingIdentityDataForDeOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - Birth date is required for individual customers when countrycode is DE. Use SetBirthDate().\n" +
                "MISSING VALUE - Name is required for individual customers when countrycode is DE. Use SetName().\n" +
                "MISSING VALUE - Street address is required for all customers when countrycode is DE. Use SetStreetAddress().\n" +
                "MISSING VALUE - Locality is required for all customers when countrycode is DE. Use SetLocality().\n" +
                "MISSING VALUE - Zip code is required for all customers when countrycode is DE. Use SetCustomerZipCode().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(CountryCode.DE)
                                                       .AddCustomerDetails(Item.IndividualCustomer());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingBirthDateForDeOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - Birth date is required for individual customers when countrycode is DE. Use SetBirthDate().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n";

            var exception = Assert.Throws<SveaWebPayValidationException>(() =>
                {
                    var builder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig());

                    var customer = Item.IndividualCustomer()
                                       .SetName("Tess", "Testson")
                                       .SetStreetAddress("Gatan", "23")
                                       .SetZipCode("9999")
                                       .SetLocality("Stan");

                    var order = builder
                        .SetCountryCode(CountryCode.DE)
                        .SetOrderDate(new DateTime(2005, 08, 15, 15, 52, 1))
                        .AddCustomerDetails(customer)
                        .UseInvoicePayment();

                    order.PrepareRequest();
                });

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingVatNumberForIndividualOrderDe()
        {
            const string expectedMessage =
                "MISSING VALUE - Birth date is required for individual customers when countrycode is DE. Use SetBirthDate().\n" +
                "MISSING VALUE - Name is required for individual customers when countrycode is DE. Use SetName().\n" +
                "MISSING VALUE - Street address is required for all customers when countrycode is DE. Use SetStreetAddress().\n" +
                "MISSING VALUE - Locality is required for all customers when countrycode is DE. Use SetLocality().\n" +
                "MISSING VALUE - Zip code is required for all customers when countrycode is DE. Use SetCustomerZipCode().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(CountryCode.DE)
                                                       .AddCustomerDetails(Item.IndividualCustomer())
                                                       .SetValidator(new VoidValidator());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingVatNumberForCompanyOrderDe()
        {
            const string expectedMessage =
                "MISSING VALUE - Vat number is required for company customers when countrycode is DE. Use SetVatNumber().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(CountryCode.DE)
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetCompanyName("K. H. Maier gmbH")
                                                                               .SetStreetAddress("Adalbertsteinweg", "1")
                                                                               .SetLocality("AACHEN")
                                                                               .SetZipCode("52070"))
                                                       .SetOrderDate(new DateTime(2012, 09, 09))
                                                       .SetValidator(new VoidValidator());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingIdentityDataForNlOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - Initials is required for individual customers when countrycode is NL. Use SetInitials().\n" +
                "MISSING VALUE - Birth date is required for individual customers when countrycode is NL. Use SetBirthDate().\n" +
                "MISSING VALUE - Name is required for individual customers when countrycode is NL. Use SetName().\n" +
                "MISSING VALUE - Street address and house number is required for all customers when countrycode is NL. Use SetStreetAddress().\n" +
                "MISSING VALUE - Locality is required for all customers when countrycode is NL. Use SetLocality().\n" +
                "MISSING VALUE - Zip code is required for all customers when countrycode is NL. Use SetZipCode().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(CountryCode.NL).Build()
                                                       .AddCustomerDetails(Item.IndividualCustomer());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingCompanyIdentityForNlOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - Vat number is required for company customers when countrycode is NL. Use SetVatNumber().\n" +
                "MISSING VALUE - Company name is required for individual customers when countrycode is NL. Use SetName().\n" +
                "MISSING VALUE - Street address and house number is required for all customers when countrycode is NL. Use SetStreetAddress().\n" +
                "MISSING VALUE - Locality is required for all customers when countrycode is NL. Use SetLocality().\n" +
                "MISSING VALUE - Zip code is required for all customers when countrycode is NL. Use SetZipCode().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(CountryCode.NL).Build()
                                                       .AddCustomerDetails(Item.CompanyCustomer());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingCompanyIdentityForDeOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - Vat number is required for company customers when countrycode is DE. Use SetVatNumber().\n" +
                "MISSING VALUE - Street address is required for all customers when countrycode is DE. Use SetStreetAddress().\n" +
                "MISSING VALUE - Locality is required for all customers when countrycode is DE. Use SetLocality().\n" +
                "MISSING VALUE - Zip code is required for all customers when countrycode is DE. Use SetCustomerZipCode().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(CountryCode.DE).Build()
                                                       .AddCustomerDetails(Item.CompanyCustomer());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingInitialsForNlOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - Initials is required for individual customers when countrycode is NL. Use SetInitials().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddCustomerDetails(Item.IndividualCustomer()
                                                                               .SetBirthDate("19231212")
                                                                               .SetName("Tess", "Testson")
                                                                               .SetStreetAddress("Gatan", "23")
                                                                               .SetZipCode("9999")
                                                                               .SetLocality("Stan"))
                                                       .SetCountryCode(CountryCode.NL)
                                                       .SetValidator(new VoidValidator());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestMissingCountryCodeGetPaymentPlanParams()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";

            var exception =
                Assert.Throws<SveaWebPayValidationException>(
                    () => WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig()).PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingOrderDate()
        {
            const string expectedMessage =
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                                       .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                                       .SetOrderDate(new DateTime())
                                                       .SetValidator(new VoidValidator());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestSucceedOnGoodValuesSe()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                                       .AddCustomerDetails(Item.IndividualCustomer()
                                                                               .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .SetOrderDate(new DateTime(2012, 05, 01))
                                                       .SetValidator(new VoidValidator());

            Assert.That(_orderValidator.Validate(order), Is.EqualTo(""));
        }

        [Test]
        public void TestFailOnMissingOrderIdOnDeliverOrder()
        {
            const string expectedMessage = "MISSING VALUE - SetOrderId is required.";

            var exception =
                Assert.Throws<SveaWebPayValidationException>(() =>
                                                             WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                                             .SetNumberOfCreditDays(1)
                                                                             .SetInvoiceDistributionType(DistributionType.POST)
                                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                             .DeliverInvoiceOrder()
                                                                             .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingOrderTypeForInvoiceOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - SetInvoiceDistributionType is required for DeliverInvoiceOrder.";

            var exception =
                Assert.Throws<SveaWebPayValidationException>(() =>
                                                             WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                                             .SetNumberOfCreditDays(1)
                                                                             .SetOrderId(2345L)
                                                                             .SetCountryCode(CountryCode.SE)
                                                                             .DeliverInvoiceOrder()
                                                                             .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingRows()
        {
            const string expectedMessage =
                "MISSING VALUE - No order or fee has been included. Use AddOrder(...) or AddFee(...).";

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                      .SetNumberOfCreditDays(1)
                                      .SetOrderId(2345L)
                                      .SetInvoiceDistributionType(DistributionType.POST)
                                      .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                      .DeliverInvoiceOrder()
                                      .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailCompanyCustomerUsingPaymentPlan()
        {
            const string expectedMessage = "ERROR - CompanyCustomer is not allowed to use payment plan option.";

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .SetOrderDate(new DateTime(2012, 09, 09))
                                .UsePaymentPlanPayment(777L));


            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnIncorrectFormatPeppolIdFirstCharacters()
        {
            const string expectedMessage = "NOT VALID - First 4 characters of PeppolId must be numeric.";

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                 .AddCustomerDetails(Item.IndividualCustomer()
                                                                         .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber)
                                                                         .SetIpAddress("123.123.123"))
                                                 .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                 .SetOrderDate(TestingTool.DefaultTestDate)
                                                 .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                 .SetCurrency(TestingTool.DefaultTestCurrency)
                                                 .SetPeppolId("12a4:1sdf")
                                                 .UseInvoicePayment()
                                                 .PrepareRequest());


            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnIncorrectFormatPeppolIdFifthCharacter()
        {
            const string expectedMessage = "NOT VALID - The fifth character of PeppolId must be \":\"";

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                 .AddCustomerDetails(Item.IndividualCustomer()
                                                                         .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber)
                                                                         .SetIpAddress("123.123.123"))
                                                 .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                 .SetOrderDate(TestingTool.DefaultTestDate)
                                                 .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                 .SetCurrency(TestingTool.DefaultTestCurrency)
                                                 .SetPeppolId("12345678")
                                                 .UseInvoicePayment()
                                                 .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnIncorrectFormatPeppolIdLastCharacters()
        {
            const string expectedMessage = "NOT VALID - All characters after the fifth character in PeppolId must be alphanumeric.";

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                 .AddCustomerDetails(Item.IndividualCustomer()
                                                                         .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber)
                                                                         .SetIpAddress("123.123.123"))
                                                 .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                 .SetOrderDate(TestingTool.DefaultTestDate)
                                                 .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                 .SetCurrency(TestingTool.DefaultTestCurrency)
                                                 .SetPeppolId("1234:.")
                                                 .UseInvoicePayment()
                                                 .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnIncorrectFormatPeppolIdTooShort()
        {
            const string expectedMessage = "NOT VALID - PeppolId is too short, must be 6 characters or longer.";

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                 .AddCustomerDetails(Item.CompanyCustomer()
                                                                         .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber)
                                                                         .SetIpAddress("123.123.123"))
                                                 .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                 .SetOrderDate(TestingTool.DefaultTestDate)
                                                 .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                 .SetCurrency(TestingTool.DefaultTestCurrency)
                                                 .SetPeppolId("1234")
                                                 .UseInvoicePayment()
                                                 .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnIncorrectFormatPeppolIdTooLong()
        {
            const string expectedMessage = "NOT VALID - PeppolId is too long, must be 55 characters or fewer.";

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                 .AddCustomerDetails(Item.CompanyCustomer()
                                                                         .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber)
                                                                         .SetIpAddress("123.123.123"))
                                                 .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                 .SetOrderDate(TestingTool.DefaultTestDate)
                                                 .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                 .SetCurrency(TestingTool.DefaultTestCurrency)
                                                 .SetPeppolId("1234:asdf123456789asdf123456789asdf123456789asdf123456789asdf123456789")
                                                 .UseInvoicePayment()
                                                 .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnIncorrectFormatPeppolIdWrongCustomerType()
        {
            const string expectedMessage = "NOT VALID - CustomerType must be a company when using PeppolId.";

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                 .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                 .AddCustomerDetails(Item.IndividualCustomer()
                                                                         .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber)
                                                                         .SetIpAddress("123.123.123"))
                                                 .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                 .SetOrderDate(TestingTool.DefaultTestDate)
                                                 .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                 .SetCurrency(TestingTool.DefaultTestCurrency)
                                                 .SetPeppolId("1234:asdf")
                                                 .UseInvoicePayment()
                                                 .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestIncorrectCountryCodeOnDeliverOrderWithEInvoiceB2B()
        {
            const string expectedMessage = "NOT VALID - Invalid country code, must be CountryCode.NO if InvoiceDistributionType is DistributionType.EInvoiceB2B.";

            DeliverOrderBuilder order = new DeliverOrderBuilder(SveaConfig.GetDefaultConfig());

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                    .AddFee(TestingTool.CreateExVatBasedShippingFee())
                    .AddDiscount(Item.FixedDiscount()
                        .SetAmountIncVat(10))
                    .SetInvoiceDistributionType(DistributionType.EINVOICEB2B)
                    .SetOrderId(54086L)
                    .SetCountryCode(TestingTool.DefaultTestCountryCode)
                    .DeliverInvoiceOrder()
                    .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestIncorrectPaymentOnDeliverOrderWithEInvoiceB2B()
        {
            const string expectedMessage = "NOT VALID - Invalid payment method, DistributionType.EINVOICEB2B can only be used when payment method is invoice.";

            DeliverOrderBuilder order = new DeliverOrderBuilder(SveaConfig.GetDefaultConfig());

            var exception = Assert.Throws<SveaWebPayValidationException>(
                () => order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                    .AddFee(TestingTool.CreateExVatBasedShippingFee())
                    .AddDiscount(Item.FixedDiscount()
                        .SetAmountIncVat(10))
                    .SetInvoiceDistributionType(DistributionType.EINVOICEB2B)
                    .SetOrderId(54086L)
                    .SetCountryCode(CountryCode.NO)
                    .DeliverPaymentPlanOrder()
                    .PrepareRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }
    }
}