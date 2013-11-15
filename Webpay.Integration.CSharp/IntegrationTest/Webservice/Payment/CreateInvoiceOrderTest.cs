using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.Payment
{
    [TestFixture]
    public class CreateInvoiceOrderTest
    {
        [Test]
        public void TestInvoiceDoRequestWithIpAddressSetSe()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
        public void TestInvoiceRequestUsingAmountIncVatWithZeroVatPercent()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
        public void TestInvoiceRequestFailing()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(
                                                                 Item.IndividualCustomer().SetNationalIdNumber(""))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.That(response.Accepted, Is.False);
        }

        [Test]
        public void TestCalculationWithTwelvePercentVat()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(Item.OrderRow()
                                                                              .SetArticleNumber("1")
                                                                              .SetQuantity(1)
                                                                              .SetAmountExVat(45M)
                                                                              .SetDescription("Specification")
                                                                              .SetName("Prod")
                                                                              .SetVatPercent(12M)
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
            Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(50.4));
        }

        [Test]
        public void TestCalculationWithSixPercentVat()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
        public void TestInvoiceForIndividualFromSe()
        {
            var response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                           .AddCustomerDetails(
                                               Item.IndividualCustomer().SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                           .UseInvoicePayment()
                                           .DoRequest();

            Assert.IsTrue(response.Accepted);
            Assert.IsTrue(response.CreateOrderResult.SveaWillBuyOrder);
            Assert.That(response.CreateOrderResult.Amount, Is.EqualTo(250.00));
            Assert.That(response.CreateOrderResult.OrderType, Is.EqualTo("Invoice"));

            //CustomerIdentity            
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
        public void TestInvoiceCompanySe()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

        [Test]
        public void TestInvoiceForIndividualFromNl()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateOrderRowNl())
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetBirthDate("19550307")
                                                                                     .SetInitials("SB")
                                                                                     .SetName("Sneider", "Boasman")
                                                                                     .SetStreetAddress("Gate", "42")
                                                                                     .SetLocality("BARENDRECHT")
                                                                                     .SetZipCode("1102 HG")
                                                                                     .SetCoAddress("138"))
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

            //CustomerIdentity            
            Assert.That(response.CreateOrderResult.CustomerIdentity.Email, Is.Null);
            Assert.That(response.CreateOrderResult.CustomerIdentity.IpAddress, Is.Null);
            Assert.That(response.CreateOrderResult.CustomerIdentity.CountryCode, Is.EqualTo("NL"));
            Assert.That(response.CreateOrderResult.CustomerIdentity.HouseNumber, Is.EqualTo("23"));
            Assert.That(response.CreateOrderResult.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Individual));
            Assert.That(response.CreateOrderResult.CustomerIdentity.PhoneNumber, Is.Null);
            Assert.That(response.CreateOrderResult.CustomerIdentity.FullName, Is.EqualTo("Sneider Boasman"));
            Assert.That(response.CreateOrderResult.CustomerIdentity.Street, Is.EqualTo("Gate 42"));
            Assert.That(response.CreateOrderResult.CustomerIdentity.CoAddress, Is.EqualTo("138"));
            Assert.That(response.CreateOrderResult.CustomerIdentity.ZipCode, Is.EqualTo("1102 HG"));
            Assert.That(response.CreateOrderResult.CustomerIdentity.Locality, Is.EqualTo("BARENDRECHT"));
        }

        [Test]
        public void TestFormatShippingFeeRowsZero()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
}