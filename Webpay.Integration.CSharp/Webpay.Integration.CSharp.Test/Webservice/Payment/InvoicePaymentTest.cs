using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Test.Util;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;
using OrderType = Webpay.Integration.CSharp.WebpayWS.OrderType;

namespace Webpay.Integration.CSharp.Test.Webservice.Payment
{
    [TestFixture]
    public class InvoicePaymentTest
    {
        [Test]
        public void TestInvoiceRequestObjectForCustomerIdentityIndividualFromSe()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
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

            //CustomerIdentity            
            Assert.AreEqual("194605092222", request.CreateOrderInformation.CustomerIdentity.NationalIdNumber);
            Assert.AreEqual(CountryCode.SE.ToString().ToUpper(),
                            request.CreateOrderInformation.CustomerIdentity.CountryCode);
            Assert.AreEqual(CustomerType.Individual, request.CreateOrderInformation.CustomerIdentity.CustomerType);
        }

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

            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestInvoiceRequestObjectWithAuth()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                           .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                                           .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("sverigetest", request.Auth.Username);
            Assert.AreEqual("sverigetest", request.Auth.Password);
            Assert.AreEqual(79021, request.Auth.ClientNumber);
        }

        [Test]
        public void TestSetAuth()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(79021, request.Auth.ClientNumber);
            Assert.AreEqual("sverigetest", request.Auth.Username);
            Assert.AreEqual("sverigetest", request.Auth.Password);
        }

        [Test]
        public void TestInvoiceRequestObjectForCustomerIdentityIndividualFromNl()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                           .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                           .SetCountryCode(CountryCode.NL)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("test@svea.com", request.CreateOrderInformation.CustomerIdentity.Email);
            Assert.AreEqual("0811111111", request.CreateOrderInformation.CustomerIdentity.PhoneNumber);
            Assert.AreEqual("123.123.123", request.CreateOrderInformation.CustomerIdentity.IpAddress);
            Assert.AreEqual("Tess Persson", request.CreateOrderInformation.CustomerIdentity.FullName);
            Assert.AreEqual("Testgatan", request.CreateOrderInformation.CustomerIdentity.Street);
            Assert.AreEqual("c/o Eriksson, Erik", request.CreateOrderInformation.CustomerIdentity.CoAddress);
            Assert.AreEqual("99999", request.CreateOrderInformation.CustomerIdentity.ZipCode);
            Assert.AreEqual("1", request.CreateOrderInformation.CustomerIdentity.HouseNumber);
            Assert.AreEqual("Stan", request.CreateOrderInformation.CustomerIdentity.Locality);
            Assert.AreEqual(CountryCode.NL.ToString().ToUpper(),
                            request.CreateOrderInformation.CustomerIdentity.CountryCode);
            Assert.AreEqual(CustomerType.Individual, request.CreateOrderInformation.CustomerIdentity.CustomerType);
            Assert.AreEqual("Tess", request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.FirstName);
            Assert.AreEqual("Persson", request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.LastName);
            Assert.AreEqual("SB", request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.Initials);
            Assert.AreEqual("19231212", request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.BirthDate);
        }

        [Test]
        public void TestInvoiceRequestObjectForCustomerIdentityCompanyFromNl()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                           .AddCustomerDetails(TestingTool.CreateIndividualCustomerNl())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                           .SetCountryCode(CountryCode.NL)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("test@svea.com", request.CreateOrderInformation.CustomerIdentity.Email);
            Assert.AreEqual("999999", request.CreateOrderInformation.CustomerIdentity.PhoneNumber);
            Assert.AreEqual("123.123.123", request.CreateOrderInformation.CustomerIdentity.IpAddress);
            Assert.AreEqual("Svea bakkerij 123", request.CreateOrderInformation.CustomerIdentity.FullName);
            Assert.AreEqual("Gatan", request.CreateOrderInformation.CustomerIdentity.Street);
            Assert.AreEqual("c/o Eriksson", request.CreateOrderInformation.CustomerIdentity.CoAddress);
            Assert.AreEqual("9999", request.CreateOrderInformation.CustomerIdentity.ZipCode);
            Assert.AreEqual("23", request.CreateOrderInformation.CustomerIdentity.HouseNumber);
            Assert.AreEqual("Stan", request.CreateOrderInformation.CustomerIdentity.Locality);
            Assert.AreEqual(CountryCode.NL.ToString().ToUpper(),
                            request.CreateOrderInformation.CustomerIdentity.CountryCode);
            Assert.AreEqual(CustomerType.Individual, request.CreateOrderInformation.CustomerIdentity.CustomerType);
        }

        [Test]
        public void TestInvoiceRequestObjectForCustomerIdentityCompanyFromSe()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                           .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("2345234", request.CreateOrderInformation.CustomerIdentity.NationalIdNumber);
            Assert.AreEqual(CountryCode.SE.ToString().ToUpper(),
                            request.CreateOrderInformation.CustomerIdentity.CountryCode);
            Assert.AreEqual(CustomerType.Company, request.CreateOrderInformation.CustomerIdentity.CustomerType);
        }

        [Test]
        public void TestInvoiceRequestObjectForSEorderOnOneProductRow()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
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

            Assert.AreEqual("1", request.CreateOrderInformation.OrderRows[0].ArticleNumber);
            Assert.AreEqual("Prod: Specification", request.CreateOrderInformation.OrderRows[0].Description);
            Assert.AreEqual(100.00, request.CreateOrderInformation.OrderRows[0].PricePerUnit);
            Assert.AreEqual(2, request.CreateOrderInformation.OrderRows[0].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[0].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[0].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[0].DiscountPercent);

            Assert.AreEqual("33", request.CreateOrderInformation.OrderRows[1].ArticleNumber);
            Assert.AreEqual("shipping: Specification", request.CreateOrderInformation.OrderRows[1].Description);
            Assert.AreEqual(50, request.CreateOrderInformation.OrderRows[1].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[1].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[1].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[1].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[1].DiscountPercent);

            Assert.AreEqual("", request.CreateOrderInformation.OrderRows[2].ArticleNumber);
            Assert.AreEqual("Svea fee: Fee for invoice", request.CreateOrderInformation.OrderRows[2].Description);
            Assert.AreEqual(50, request.CreateOrderInformation.OrderRows[2].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[2].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[2].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[2].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[2].DiscountPercent);
        }

        [Test]
        public void TestInvoiceRequestObjectWithRelativeDiscountOnDifferentProductVat()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
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

            Assert.AreEqual("1", request.CreateOrderInformation.OrderRows[2].ArticleNumber);
            Assert.AreEqual("RelativeDiscount (25%)", request.CreateOrderInformation.OrderRows[2].Description);
            Assert.AreEqual("RelativeDiscount (6%)", request.CreateOrderInformation.OrderRows[3].Description);
            Assert.AreEqual(-85.74, 
                request.CreateOrderInformation.OrderRows[2].PricePerUnit +
                request.CreateOrderInformation.OrderRows[3].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[2].NumberOfUnits);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[2].VatPercent);
            Assert.AreEqual(6, request.CreateOrderInformation.OrderRows[3].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[2].DiscountPercent);
        }

        [Test]
        public void TestInvoiceRequestObjectWithFixedDiscountOnDifferentProductVat()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
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

            Assert.AreEqual("1", request.CreateOrderInformation.OrderRows[2].ArticleNumber);
            Assert.AreEqual("FixedDiscount (25%)", request.CreateOrderInformation.OrderRows[2].Description);
            Assert.AreEqual("FixedDiscount (6%)", request.CreateOrderInformation.OrderRows[3].Description);
            Assert.AreEqual(-85.74, 
                request.CreateOrderInformation.OrderRows[2].PricePerUnit +
                request.CreateOrderInformation.OrderRows[3].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[2].NumberOfUnits);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[2].VatPercent);
            Assert.AreEqual(6, request.CreateOrderInformation.OrderRows[3].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[2].DiscountPercent);
        }

        [Test]
        public void TestInvoiceRequestObjectWithCreateOrderInformation()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(TestingTool.DefaultTestDate, request.CreateOrderInformation.OrderDate);
            Assert.AreEqual("33", request.CreateOrderInformation.ClientOrderNumber);
            Assert.AreEqual(OrderType.Invoice, request.CreateOrderInformation.OrderType);
            Assert.AreEqual("ref33", request.CreateOrderInformation.CustomerReference);
            Assert.AreEqual("ad33", request.CreateOrderInformation.AddressSelector);
        }

        [Test]
        public void TestInvoiceRequestUsingAmountIncVatWithVatPercent()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
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

            Assert.AreEqual("1", request.CreateOrderInformation.OrderRows[0].ArticleNumber);
            Assert.AreEqual("Prod: Specification", request.CreateOrderInformation.OrderRows[0].Description);
            Assert.AreEqual(100.00, request.CreateOrderInformation.OrderRows[0].PricePerUnit);
            Assert.AreEqual(2, request.CreateOrderInformation.OrderRows[0].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[0].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[0].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[0].DiscountPercent);

            Assert.AreEqual("33", request.CreateOrderInformation.OrderRows[1].ArticleNumber);
            Assert.AreEqual("shipping: Specification", request.CreateOrderInformation.OrderRows[1].Description);
            Assert.AreEqual(50, request.CreateOrderInformation.OrderRows[1].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[1].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[1].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[1].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[1].DiscountPercent);

            Assert.AreEqual("", request.CreateOrderInformation.OrderRows[2].ArticleNumber);
            Assert.AreEqual("Svea fee: Fee for invoice", request.CreateOrderInformation.OrderRows[2].Description);
            Assert.AreEqual(50, request.CreateOrderInformation.OrderRows[2].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[2].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[2].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[2].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[2].DiscountPercent);
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

            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestInvoiceRequestUsingAmountIncVatWithAmountExVat()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
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

            Assert.AreEqual("1", request.CreateOrderInformation.OrderRows[0].ArticleNumber);
            Assert.AreEqual("Prod: Specification", request.CreateOrderInformation.OrderRows[0].Description);
            Assert.AreEqual(100.00, request.CreateOrderInformation.OrderRows[0].PricePerUnit);
            Assert.AreEqual(2, request.CreateOrderInformation.OrderRows[0].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[0].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[0].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[0].DiscountPercent);

            Assert.AreEqual("33", request.CreateOrderInformation.OrderRows[1].ArticleNumber);
            Assert.AreEqual("shipping: Specification", request.CreateOrderInformation.OrderRows[1].Description);
            Assert.AreEqual(50, request.CreateOrderInformation.OrderRows[1].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[1].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[1].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[1].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[1].DiscountPercent);

            Assert.AreEqual("", request.CreateOrderInformation.OrderRows[2].ArticleNumber);
            Assert.AreEqual("Svea fee: Fee for invoice", request.CreateOrderInformation.OrderRows[2].Description);
            Assert.AreEqual(50, request.CreateOrderInformation.OrderRows[2].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[2].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[2].Unit);
            Assert.AreEqual(25, request.CreateOrderInformation.OrderRows[2].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[2].DiscountPercent);
        }
    }
}