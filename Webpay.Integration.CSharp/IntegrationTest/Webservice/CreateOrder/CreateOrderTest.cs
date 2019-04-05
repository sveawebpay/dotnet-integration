using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.IntegrationTest.Config;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.CreateOrder
{
    [TestFixture]
    public class CreateOrderTest
    {
        [Test]
        public void TestConfiguration()
        {
            var conf = new ConfigurationProviderTestData();
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(conf)
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
                                                             .DoRequest();

            Assert.That(response.ResultCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
        }

        [Test]
        public void TestFormatShippingFeeRowsZero()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
                                                             .DoRequest();

            Assert.That(response.ResultCode, Is.EqualTo(0));
            Assert.That(response.Accepted, Is.True);
        }

        [Test]
        public void TestCompanyIdResponse()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.IndividualIdentity);
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.CompanyIdentity);
            Assert.That(response.Accepted, Is.True);
        }

        [Test, Ignore("Credit score is broken for Germany in stage")]
        public void TestDeCompanyIdentity()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateOrderRowDe())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetNationalIdNumber("12345")
                                                                                     .SetVatNumber("DE123456789")
                                                                                     .SetStreetAddress(
                                                                                         "Adalbertsteinweg", "1")
                                                                                     .SetZipCode("52070")
                                                                                     .SetLocality("AACHEN"))
                                                             .SetCountryCode(CountryCode.DE)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(Currency.EUR)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.That(response.ResultCode, Is.EqualTo(0));
            Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
            Assert.That(response.Accepted, Is.True);
        }

        [Test, Ignore("Credit score is broken for Netherlands in stage")]
        public void TestNlCompanyIdentity()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateOrderRowNl())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetCompanyName("Svea bakkerij 123")
                                                                                     .SetVatNumber("NL123456789A12")
                                                                                     .SetStreetAddress("broodstraat",
                                                                                                       "1")
                                                                                     .SetZipCode("1111 CD")
                                                                                     .SetLocality("BARENDRECHT"))
                                                             .SetCountryCode(CountryCode.NL)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(Currency.EUR)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.That(response.ResultCode, Is.EqualTo(0));
            Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
            Assert.That(response.Accepted, Is.True);
        }

        [Test]
        public void Test_CreateOrder_SE_WithOnlyNationalIdNumber_ShouldNotSetIndividualIdentity()
        {
            CreateOrderBuilder createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .SetOrderDate(TestingTool.DefaultTestDate)
                .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                .SetCurrency(TestingTool.DefaultTestCurrency)
                ;
            CreateOrderEuRequest request = createOrderBuilder.UseInvoicePayment().PrepareRequest();
            Assert.IsNull(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity);

            CreateOrderEuResponse order = createOrderBuilder.UseInvoicePayment().DoRequest();
            Assert.IsTrue(order.Accepted);
        }

        [Test]
        public void Test_CreateOrder_NO_WithOnlyNationalIdNumber_ShouldNotSetIndividualIdentity()
        {
            CreateOrderBuilder createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber("17054512066"))    // NO test individual "Ola Norrmann"
                .SetCountryCode(CountryCode.NO) // NO
                .SetOrderDate(TestingTool.DefaultTestDate)
                .SetClientOrderNumber("33308")  // NO Invoice
                .SetCurrency(TestingTool.DefaultTestCurrency)
                ;
            CreateOrderEuRequest request = createOrderBuilder.UseInvoicePayment().PrepareRequest();
            Assert.IsNull(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity);

            CreateOrderEuResponse order = createOrderBuilder.UseInvoicePayment().DoRequest();
            Assert.IsTrue(order.Accepted);
        }

        [Test]
        public void Test_CreateOrder_NO_WithAllCustomerDetailsSet_ShouldNotSetIndividualIdentity()
        {
            CreateOrderBuilder createOrderBuilder = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                .AddCustomerDetails(Item.IndividualCustomer()
                    .SetNationalIdNumber("17054512066")     // NO test individual "Ola Norrmann"
                    // below taken from docs, not accurate
                    .SetBirthDate("19460509")               //Required for individual customers in NL and DE
                    .SetName("Tess", "Testson")             //Required for individual customers in NL and DE
                    .SetInitials("SB")                      //Required for individual customers in NL
                    .SetStreetAddress("Gatan", "23")            //Required in NL and DE
                    .SetCoAddress("c/o Eriksson")           //Optional
                    .SetZipCode("9999")                         //Required in NL and DE
                    .SetLocality("Stan")                    //Required in NL and DE
                    .SetPhoneNumber("999999")               //Optional
                    .SetEmail("test@svea.com")              //Optional but desirable
                    .SetIpAddress("123.123.123")            //Optional but desirable
                )
                .SetCountryCode(CountryCode.NO) // NO
                .SetOrderDate(TestingTool.DefaultTestDate)
                .SetClientOrderNumber("33308")  // NO Invoice
                .SetCurrency(TestingTool.DefaultTestCurrency)
                ;
            CreateOrderEuRequest request = createOrderBuilder.UseInvoicePayment().PrepareRequest();
            Assert.IsNull(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity);

            CreateOrderEuResponse order = createOrderBuilder.UseInvoicePayment().DoRequest();
            Assert.IsTrue(order.Accepted);
        }

        //CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
        //                                                 .AddOrderRow(TestingTool.CreateOrderRowDe())
        //                                                 .AddCustomerDetails(Item.CompanyCustomer()
        //                                                                         .SetNationalIdNumber("12345")
        //                                                                         .SetVatNumber("DE123456789")
        //                                                                         .SetStreetAddress(
        //                                                                             "Adalbertsteinweg", "1")
        //                                                                         .SetZipCode("52070")
        //                                                                         .SetLocality("AACHEN"))
        //                                                 .SetCountryCode(CountryCode.DE)
        //                                                 .SetClientOrderNumber(
        //                                                     TestingTool.DefaultTestClientOrderNumber)
        //                                                 .SetOrderDate(TestingTool.DefaultTestDate)
        //                                                 .SetCurrency(Currency.EUR)
        //                                                 .UseInvoicePayment()
        //                                                 .DoRequest();

        //Assert.That(response.ResultCode, Is.EqualTo(0));
        //Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Company));
        //Assert.That(response.Accepted, Is.True);
    }
}
