using NUnit.Framework;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Validator;
using Webpay.Integration.CSharp.Test.Hosted.Payment;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Order.Validator
{
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
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
                                           "MISSING VALUE - ClientOrderNumber is required. Use SetClientOrderNumber(...).\n" +
                                           "MISSING VALUE - Currency is required. Use SetCurrency(...).\n" +
                                           "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet"))
                                                       .SetValidator(new VoidValidator())
                                                       .Build();

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnEmptyClientOrderNumber()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
                                           "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetQuantity(1)
                                                                        .SetAmountExVat(100)
                                                                        .SetVatPercent(25))
                                                       .SetCurrency(Currency.SEK)
                                                       .SetClientOrderNumber("")
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet")
                                                                               .SetNationalIdNumber("1222"))
                                                       .SetValidator(new VoidValidator())
                                                       .Build();
            _orderValidator = new HostedOrderValidator();
            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingReturnUrl()
        {
            const string expectedMessage = "MISSING VALUE - Return url is required, SetReturnUrl(...).\n";

            var exception = Assert.Throws<SveaWebPayValidationException>(() =>
                {
                    CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                               .SetCountryCode(CountryCode.SE)
                                                               .SetClientOrderNumber("nr22")
                                                               .SetCurrency(Currency.SEK)
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
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetCountryCode(CountryCode.SE)
                                                       .SetCurrency(Currency.SEK)
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber("1")
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(5.0M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet"));
            _orderValidator = new HostedOrderValidator();

            Assert.AreEqual("", _orderValidator.Validate(order));
        }

        [Test]
        public void TestValidateNlCustomerIdentity()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetClientOrderNumber("1")
                                                       .SetCountryCode(CountryCode.NL)
                                                       .SetCurrency(Currency.EUR)
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(5.0M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet"));
            _orderValidator = new HostedOrderValidator();

            Assert.AreEqual("", _orderValidator.Validate(order));
        }

        [Test]
        public void TestValidateDeCustomerIdentity()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetClientOrderNumber("1")
                                                       .SetCountryCode(CountryCode.DE)
                                                       .SetCurrency(Currency.EUR)
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(5.0M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet"));
            _orderValidator = new HostedOrderValidator();

            Assert.AreEqual("", _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailVatPercentIsMissing()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
                                           "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n" +
                                           "MISSING VALUE - At least one of the values must be set in combination with AmountExVat: AmountIncVat or VatPercent for Orderrow. Use one of: SetAmountIncVat() or SetVatPercent().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetQuantity(1)
                                                                        .SetAmountExVat(100))
                                                       .SetCurrency(Currency.SEK)
                                                       .SetClientOrderNumber("")
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet")
                                                                               .SetNationalIdNumber("1222"))
                                                       .SetValidator(new VoidValidator())
                                                       .Build();
            _orderValidator = new HostedOrderValidator();

            Assert.AreEqual(_orderValidator.Validate(order), expectedMessage);
        }

        [Test]
        public void TestFailAmountExVatIsMissing()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
                                           "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n" +
                                           "MISSING VALUE - At least one of the values must be set in combination with VatPercent: AmountIncVat or AmountExVat for Orderrow. Use one of: SetAmountExVat() or SetAmountIncVat().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetQuantity(1)
                                                                        .SetVatPercent(25))
                                                       .SetCurrency(Currency.SEK)
                                                       .SetClientOrderNumber("")
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet")
                                                                               .SetNationalIdNumber("1222"))
                                                       .SetValidator(new VoidValidator())
                                                       .Build();
            _orderValidator = new HostedOrderValidator();
            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailAmountExVatAndVatPercentIsMissing()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
                                           "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n" +
                                           "MISSING VALUE - At least one of the values must be set in combination with AmountIncVat: AmountExVat or VatPercent for Orderrow. Use one of: SetAmountExVat() or SetVatPercent().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetQuantity(1)
                                                                        .SetAmountIncVat(125))
                                                       .SetCurrency(Currency.SEK)
                                                       .SetClientOrderNumber("")
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet")
                                                                               .SetNationalIdNumber("1222"))
                                                       .SetValidator(new VoidValidator())
                                                       .Build();
            _orderValidator = new HostedOrderValidator();
            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestValidateFailOrderIsNull()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode(...).\n" +
                                           "MISSING VALUE - ClientOrderNumber is required (has an empty value). Use SetClientOrderNumber(...).\n" +
                                           "MISSING VALUES - AmountExVat, Quantity and VatPercent are required for Orderrow. Use SetAmountExVat(), SetQuantity() and SetVatPercent().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(null)
                                                       .SetCurrency(Currency.SEK)
                                                       .SetClientOrderNumber("")
                                                       .AddCustomerDetails(Item.CompanyCustomer()
                                                                               .SetVatNumber("2345234")
                                                                               .SetCompanyName("TestCompagniet")
                                                                               .SetNationalIdNumber("1222"))
                                                       .SetValidator(new VoidValidator())
                                                       .Build();
            _orderValidator = new HostedOrderValidator();
            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
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

            var exception = Assert.Throws<SveaWebPayValidationException>(() => WebpayConnection.CreateOrder()
                                                                                               .AddOrderRow(Item
                                                                                                                .OrderRow
                                                                                                                ()
                                                                                                                .SetArticleNumber
                                                                                                                ("1")
                                                                                                                .SetQuantity
                                                                                                                (2)
                                                                                                                .SetAmountExVat
                                                                                                                (100.00M)
                                                                                                                .SetDescription
                                                                                                                ("Specification")
                                                                                                                .SetName
                                                                                                                ("Prod")
                                                                                                                .SetUnit
                                                                                                                ("st")
                                                                                                                .SetVatPercent
                                                                                                                (25)
                                                                                                                .SetDiscountPercent
                                                                                                                (0))
                                                                                               .AddDiscount(Item
                                                                                                                .RelativeDiscount
                                                                                                                ()
                                                                                                                .SetDiscountId
                                                                                                                ("1")
                                                                                                                .SetDiscountPercent
                                                                                                                (50)
                                                                                                                .SetUnit
                                                                                                                ("st")
                                                                                                                .SetName
                                                                                                                ("Relative")
                                                                                                                .SetDescription
                                                                                                                ("RelativeDiscount"))
                                                                                               .AddCustomerDetails(
                                                                                                   Item
                                                                                                       .IndividualCustomer
                                                                                                       ())
                                                                                               .SetCountryCode(
                                                                                                   CountryCode.NL)
                                                                                               .SetClientOrderNumber(
                                                                                                   "33")
                                                                                               .SetOrderDate(
                                                                                                   "2012-12-12")
                                                                                               .SetCurrency(Currency.SEK)
                                                                                               .UsePaymentMethod(
                                                                                                   PaymentMethod.INVOICE)
                                                                                               .SetReturnUrl(
                                                                                                   "http://myurl.se")
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
            var exception = Assert.Throws<SveaWebPayValidationException>(() => WebpayConnection.CreateOrder()
                                                                                               .AddOrderRow(Item
                                                                                                                .OrderRow
                                                                                                                ()
                                                                                                                .SetArticleNumber
                                                                                                                ("1")
                                                                                                                .SetQuantity
                                                                                                                (2)
                                                                                                                .SetAmountExVat
                                                                                                                (100.00M)
                                                                                                                .SetDescription
                                                                                                                ("Specification")
                                                                                                                .SetName
                                                                                                                ("Prod")
                                                                                                                .SetUnit
                                                                                                                ("st")
                                                                                                                .SetVatPercent
                                                                                                                (25)
                                                                                                                .SetDiscountPercent
                                                                                                                (0))
                                                                                               .AddDiscount(Item
                                                                                                                .RelativeDiscount
                                                                                                                ()
                                                                                                                .SetDiscountId
                                                                                                                ("1")
                                                                                                                .SetDiscountPercent
                                                                                                                (50)
                                                                                                                .SetUnit
                                                                                                                ("st")
                                                                                                                .SetName
                                                                                                                ("Relative")
                                                                                                                .SetDescription
                                                                                                                ("RelativeDiscount"))
                                                                                               .AddCustomerDetails(
                                                                                                   Item
                                                                                                       .IndividualCustomer
                                                                                                       ())
                                                                                               .SetCountryCode(
                                                                                                   CountryCode.DE)
                                                                                               .SetClientOrderNumber(
                                                                                                   "33")
                                                                                               .SetOrderDate(
                                                                                                   "2012-12-12")
                                                                                               .SetCurrency(Currency.SEK)
                                                                                               .UsePaymentMethod(
                                                                                                   PaymentMethod.INVOICE)
                                                                                               .SetReturnUrl(
                                                                                                   "http://myurl.se")
                                                                                               .GetPaymentForm());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }
    }
}