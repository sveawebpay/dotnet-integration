using System;
using System.ServiceModel;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Validator;
using Webpay.Integration.CSharp.Test.Util;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Handleorder;
using InvoiceDistributionType = Webpay.Integration.CSharp.Util.Constant.InvoiceDistributionType;

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
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddCustomerDetails(TestingTool.CreateCompanyCustomer());
            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestCustomerIdentityIsNull()
        {
            const string expectedMessage = "MISSING VALUE - CustomerIdentity must be set.\n" +
                                           "MISSING VALUE - CountryCode is required. Use SetCountryCode().\n" +
                                           "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                                           "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .AddCustomerDetails(null);

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingCountryCodeOnCreateOrder()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required. Use SetCountryCode().\n" +
                                           "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                                           "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .AddCustomerDetails(Item.IndividualCustomer()
                                                                               .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber));

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingCountryCodeOnDeliverOrder()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).";
            var exception = Assert.Throws<SveaWebPayValidationException>(() => WebpayConnection.DeliverOrder()
                                                                                               .AddOrderRow(
                                                                                                   TestingTool
                                                                                                       .CreateExVatBasedOrderRow())
                                                                                               .SetNumberOfCreditDays(1)
                                                                                               .SetOrderId(2345L)
                                                                                               .SetInvoiceDistributionType
                                                                                   (InvoiceDistributionType.POST)
                                                                                               .DeliverInvoiceOrder()
                                                                                               .DoRequest());

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingValuesForGetAddresses()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n" +
                                           "MISSING VALUE - orderType is required, use one of: SetOrderTypePaymentPlan() or SetOrderTypeInvoice().\n" +
                                           "MISSING VALUE - either nationalNumber or companyId is required. Use: SetCompany(...) or SetIndividual(...).\n";
            var exception =
                Assert.Throws<SveaWebPayValidationException>(() => WebpayConnection.GetAddresses().DoRequest());
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingCustomerIdentity()
        {
            const string expectedMessage =
                "MISSING VALUE - National number(ssn) is required for individual customers when countrycode is SE, NO, DK or FI. Use SetNationalIdNumber(...).\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddCustomerDetails(Item.IndividualCustomer())
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode);

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingOrderRows()
        {
            const string expectedMessage =
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetValidator(new VoidValidator())
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .AddCustomerDetails(Item.IndividualCustomer()
                                                                               .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber));

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingOrderRowValues()
        {
            const string expectedMessage =
                "MISSING VALUE - Quantity is required in Item object. Use Item.SetQuantity().\n" +
                "MISSING VALUE - Two of the values must be set: AmountExVat(not set), AmountIncVat(not set) or VatPercent(not set) for Orderrow. Use two of: SetAmountExVat(), SetAmountIncVat or SetVatPercent().\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .AddOrderRow(Item.OrderRow())
                                                       .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .SetValidator(new VoidValidator());

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingSsnForSeOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - National number(ssn) is required for individual customers when countrycode is SE, NO, DK or FI. Use SetNationalIdNumber(...).\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddCustomerDetails(Item.IndividualCustomer())
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode);

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
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
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(CountryCode.DE)
                                                       .AddCustomerDetails(Item.IndividualCustomer());

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingBirthDateForDeOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - Birth date is required for individual customers when countrycode is DE. Use SetBirthDate().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n";

            var exception = Assert.Throws<SveaWebPayValidationException>(() =>
                {
                    var builder = WebpayConnection.CreateOrder();

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

            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(CountryCode.DE)
                                                       .AddCustomerDetails(Item.IndividualCustomer())
                                                       .SetValidator(new VoidValidator());

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingVatNumberForCompanyOrderDe()
        {
            const string expectedMessage =
                "MISSING VALUE - Vat number is required for company customers when countrycode is DE. Use SetVatNumber().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
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
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(CountryCode.NL).Build()
                                                       .AddCustomerDetails(Item.IndividualCustomer());

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
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
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(CountryCode.NL).Build()
                                                       .AddCustomerDetails(Item.CompanyCustomer());

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
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
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetValidator(new VoidValidator())
                                                       .SetCountryCode(CountryCode.DE).Build()
                                                       .AddCustomerDetails(Item.CompanyCustomer());

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingInitialsForNlOrder()
        {
            const string expectedMessage =
                "MISSING VALUE - Initials is required for individual customers when countrycode is NL. Use SetInitials().\n" +
                "MISSING VALUE - OrderRows are required. Use AddOrderRow(Item.OrderRow) to get orderrow setters.\n" +
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddCustomerDetails(Item.IndividualCustomer()
                                                                               .SetBirthDate("19231212")
                                                                               .SetName("Tess", "Testson")
                                                                               .SetStreetAddress("Gatan", "23")
                                                                               .SetZipCode("9999")
                                                                               .SetLocality("Stan"))
                                                       .SetCountryCode(CountryCode.NL)
                                                       .SetValidator(new VoidValidator());

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestMissingCountryCodeGetPaymentPlanParams()
        {
            const string expectedMessage = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";
            var exception =
                Assert.Throws<SveaWebPayValidationException>(
                    () => WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig()).DoRequest());
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailOnMissingOrderDate()
        {
            const string expectedMessage =
                "MISSING VALUE - OrderDate is required. Use SetOrderDate().\n";

            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                                       .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                                       .SetOrderDate(new DateTime())
                                                       .SetValidator(new VoidValidator());

            Assert.AreEqual(expectedMessage, _orderValidator.Validate(order));
        }

        [Test]
        public void TestSucceedOnGoodValuesSe()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                                       .AddCustomerDetails(Item.IndividualCustomer()
                                                                               .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .SetOrderDate(new DateTime(2012, 05, 01))
                                                       .SetValidator(new VoidValidator());

            Assert.AreEqual("", _orderValidator.Validate(order));
        }

        [Test]
        public void TestFailOnMissingOrderIdOnDeliverOrder()
        {
            const string expectedMessage = "MISSING VALUE - SetOrderId is required.";
            var exception =
                Assert.Throws<SveaWebPayValidationException>(() =>
                                                             WebpayConnection.DeliverOrder()
                                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                                             .SetNumberOfCreditDays(1)
                                                                             .SetInvoiceDistributionType(InvoiceDistributionType.POST)
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
                                                             WebpayConnection.DeliverOrder()
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
                () => WebpayConnection.DeliverOrder()
                                      .SetNumberOfCreditDays(1)
                                      .SetOrderId(2345L)
                                      .SetInvoiceDistributionType(InvoiceDistributionType.POST)
                                      .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                      .DeliverInvoiceOrder()
                                      .DoRequest());
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestFailCompanyCustomerUsingPaymentPlan()
        {
            const string expectedMessage = "ERROR - CompanyCustomer is not allowed to use payment plan option.";
            try
            {
                WebpayConnection.CreateOrder()
                                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .SetOrderDate(new DateTime(2012, 09, 09))
                                .UsePaymentPlanPayment(777L);

                Assert.Fail("Expected exception not thrown.");
            }
            catch (SveaWebPayException ex)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
            }
        }

        [Test]
        public void TestFailOnMissingCountryCodeOfCloseOrder()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();


            var serviceSoapClient = new ServiceSoapClient(new BasicHttpBinding
                {
                    Name = "ServiceSoap",
                    Security = new BasicHttpSecurity
                        {
                            Mode = BasicHttpSecurityMode.Transport
                        }
                },
                                                          new EndpointAddress(
                                                              SveaConfig.GetDefaultConfig()
                                                                        .GetEndPoint(PaymentType.INVOICE)));


            CreateOrderEuResponse response = serviceSoapClient.CreateOrderEu(request);

            Assert.AreEqual(true, response.Accepted);
            CloseOrder closeRequest = WebpayConnection.CloseOrder()
                                                      .SetOrderId(response.CreateOrderResult.SveaOrderId)
                                                      .CloseInvoiceOrder();

            const string expectedMessage = "MISSING VALUE - CountryCode is required, use SetCountryCode(...).\n";
            Assert.AreEqual(expectedMessage, closeRequest.ValidateRequest());
        }
    }
}