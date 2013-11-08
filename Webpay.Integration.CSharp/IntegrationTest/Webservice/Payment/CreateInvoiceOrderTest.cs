using NUnit.Framework;
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
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestInvoiceRequestUsingAmountIncVatWithZeroVatPercent()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
        }


        [Test]
        public void TestInvoiceRequestFailing()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(
                                                                 Item.IndividualCustomer().SetNationalIdNumber(""))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.IsFalse(response.Accepted);
        }

        [Test]
        public void TestCalculationWithTwelvePercentVat()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
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


            Assert.IsTrue(response.Accepted);
            Assert.AreEqual(50.4, response.CreateOrderResult.Amount);
        }

        [Test]
        public void TestCalculationWithSixPercentVat()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
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


            Assert.IsTrue(response.Accepted);
            Assert.AreEqual(248.04, response.CreateOrderResult.Amount);
        }

        [Test]
        public void TestInvoiceForIndividualFromSe()
        {
            var response = WebpayConnection.CreateOrder()
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
            Assert.AreEqual(250.00, response.CreateOrderResult.Amount);
            Assert.AreEqual("Invoice", response.CreateOrderResult.OrderType);

            //CustomerIdentity            
            Assert.AreEqual("194605092222", response.CreateOrderResult.CustomerIdentity.NationalIdNumber);
            Assert.AreEqual("SE", response.CreateOrderResult.CustomerIdentity.CountryCode);
            Assert.AreEqual(CustomerType.Individual, response.CreateOrderResult.CustomerIdentity.CustomerType);
            Assert.AreEqual("Persson, Tess T", response.CreateOrderResult.CustomerIdentity.FullName);
            Assert.AreEqual("Testgatan 1", response.CreateOrderResult.CustomerIdentity.Street);
            Assert.AreEqual("c/o Eriksson, Erik", response.CreateOrderResult.CustomerIdentity.CoAddress);
            Assert.AreEqual("99999", response.CreateOrderResult.CustomerIdentity.ZipCode);
            Assert.AreEqual("Stan", response.CreateOrderResult.CustomerIdentity.Locality);
        }

        [Test]
        public void TestInvoiceCompanySe()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
            Assert.IsTrue(response.CreateOrderResult.SveaWillBuyOrder);
            Assert.AreEqual("SE", response.CreateOrderResult.CustomerIdentity.CountryCode);
        }

        [Test]
        public void TestInvoiceForIndividualFromNl()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
            Assert.IsTrue(response.CreateOrderResult.SveaWillBuyOrder);
            Assert.AreEqual(212.00, response.CreateOrderResult.Amount);
            Assert.AreEqual("Invoice", response.CreateOrderResult.OrderType);

            //CustomerIdentity            
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.Email);
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.IpAddress);
            Assert.AreEqual("NL", response.CreateOrderResult.CustomerIdentity.CountryCode);
            Assert.AreEqual("23", response.CreateOrderResult.CustomerIdentity.HouseNumber);
            Assert.AreEqual(CustomerType.Individual, response.CreateOrderResult.CustomerIdentity.CustomerType);
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.PhoneNumber);
            Assert.AreEqual("Sneider Boasman", response.CreateOrderResult.CustomerIdentity.FullName);
            Assert.AreEqual("Gate 42", response.CreateOrderResult.CustomerIdentity.Street);
            Assert.AreEqual("138", response.CreateOrderResult.CustomerIdentity.CoAddress);
            Assert.AreEqual("1102 HG", response.CreateOrderResult.CustomerIdentity.ZipCode);
            Assert.AreEqual("BARENDRECHT", response.CreateOrderResult.CustomerIdentity.Locality);
        }
    }
}