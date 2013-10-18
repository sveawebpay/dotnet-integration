using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Row;
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
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber("194609052222"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            //CustomerIdentity            
            Assert.AreEqual("194609052222", request.CreateOrderInformation.CustomerIdentity.NationalIdNumber);
            Assert.AreEqual(CountryCode.SE.ToString().ToUpper(),
                            request.CreateOrderInformation.CustomerIdentity.CountryCode);
            Assert.AreEqual(CustomerType.Individual, request.CreateOrderInformation.CustomerIdentity.CustomerType);
        }

        [Test]
        public void TestInvoiceDoRequestWithIpAddressSetSe()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(Item.OrderRow()
                                                                              .SetArticleNumber("1")
                                                                              .SetQuantity(2)
                                                                              .SetAmountExVat(100.00M)
                                                                              .SetDescription("Specification")
                                                                              .SetName("Prod")
                                                                              .SetUnit("st")
                                                                              .SetVatPercent(25)
                                                                              .SetDiscountPercent(0))
                                                             .AddOrderRow(Item.OrderRow()
                                                                              .SetArticleNumber("1")
                                                                              .SetQuantity(2)
                                                                              .SetAmountExVat(100.00M)
                                                                              .SetDescription("Specification")
                                                                              .SetName("Prod")
                                                                              .SetVatPercent(25)
                                                                              .SetDiscountPercent(0))
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber("194605092222")
                                                                                     .SetIpAddress("123.123.123"))
                                                             .SetCountryCode(CountryCode.SE)
                                                             .SetOrderDate("2012-12-12")
                                                             .SetClientOrderNumber("33")
                                                             .SetCurrency(Currency.SEK)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestInvoiceRequestObjectWithAuth()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0)
                                                                            .SetAmountExVat(100.00M))
                                                           .AddFee(Item.ShippingFee()
                                                                       .SetShippingId("33")
                                                                       .SetName("shipping")
                                                                       .SetDescription("Specification")
                                                                       .SetAmountExVat(50)
                                                                       .SetUnit("st")
                                                                       .SetVatPercent(25)
                                                                       .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetInitials("SB")
                                                                                   .SetName("Tess", "Testson")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetBirthDate("19231212")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetNationalIdNumber("194605092222")
                                                                                   .SetZipCode("2222")
                                                                                   .SetLocality("Stan"))
                                                           .SetClientOrderNumber("33")
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
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
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber("194605092222"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
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
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetInitials("SB")
                                                                                   .SetBirthDate("19231212")
                                                                                   .SetName("Tess", "Testson")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("9999")
                                                                                   .SetLocality("Stan"))
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0)
                                                                            .SetAmountExVat(100.00M))
                                                           .SetCountryCode(CountryCode.NL)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("test@svea.com", request.CreateOrderInformation.CustomerIdentity.Email);
            Assert.AreEqual("999999", request.CreateOrderInformation.CustomerIdentity.PhoneNumber);
            Assert.AreEqual("123.123.123", request.CreateOrderInformation.CustomerIdentity.IpAddress);
            Assert.AreEqual("Tess Testson", request.CreateOrderInformation.CustomerIdentity.FullName);
            Assert.AreEqual("Gatan", request.CreateOrderInformation.CustomerIdentity.Street);
            Assert.AreEqual("c/o Eriksson", request.CreateOrderInformation.CustomerIdentity.CoAddress);
            Assert.AreEqual("9999", request.CreateOrderInformation.CustomerIdentity.ZipCode);
            Assert.AreEqual("23", request.CreateOrderInformation.CustomerIdentity.HouseNumber);
            Assert.AreEqual("Stan", request.CreateOrderInformation.CustomerIdentity.Locality);
            Assert.AreEqual(CountryCode.NL.ToString().ToUpper(),
                            request.CreateOrderInformation.CustomerIdentity.CountryCode);
            Assert.AreEqual(CustomerType.Individual, request.CreateOrderInformation.CustomerIdentity.CustomerType);
            Assert.AreEqual("Tess", request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.FirstName);
            Assert.AreEqual("Testson", request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.LastName);
            Assert.AreEqual("SB", request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.Initials);
            Assert.AreEqual("19231212", request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.BirthDate);
        }

        [Test]
        public void TestInvoiceRequestObjectForCustomerIdentityCompanyFromNl()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetInitials("SB")
                                                                                   .SetBirthDate("19231212")
                                                                                   .SetName("Svea bakkerij", "123")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("9999")
                                                                                   .SetLocality("Stan"))
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .SetCountryCode(CountryCode.NL)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
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
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.CompanyCustomer()
                                                                                   .SetNationalIdNumber("vat234"))
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("vat234", request.CreateOrderInformation.CustomerIdentity.NationalIdNumber);
            Assert.AreEqual(CountryCode.SE.ToString().ToUpper(),
                            request.CreateOrderInformation.CustomerIdentity.CountryCode);
            Assert.AreEqual(CustomerType.Company, request.CreateOrderInformation.CustomerIdentity.CustomerType);
        }

        [Test]
        public void TestInvoiceRequestObjectForSEorderOnOneProductRow()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddFee(Item.ShippingFee()
                                                                       .SetShippingId("33")
                                                                       .SetName("shipping")
                                                                       .SetDescription("Specification")
                                                                       .SetAmountExVat(50)
                                                                       .SetUnit("st")
                                                                       .SetVatPercent(25)
                                                                       .SetDiscountPercent(0))
                                                           .AddFee(Item.InvoiceFee()
                                                                       .SetName("Svea fee")
                                                                       .SetDescription("Fee for invoice")
                                                                       .SetAmountExVat(50)
                                                                       .SetUnit("st")
                                                                       .SetVatPercent(25)
                                                                       .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber("194605092222")
                                                                                   .SetInitials("SB")
                                                                                   .SetBirthDate("19231212")
                                                                                   .SetName("Tess", "Testson")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("2222")
                                                                                   .SetLocality("Stan"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
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
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber("194605092222")
                                                                                   .SetInitials("SB")
                                                                                   .SetBirthDate("19231212")
                                                                                   .SetName("Tess", "Testson")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("2222")
                                                                                   .SetLocality("Stan"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("1", request.CreateOrderInformation.OrderRows[2].ArticleNumber);
            Assert.AreEqual("RelativeDiscount", request.CreateOrderInformation.OrderRows[2].Description);
            Assert.AreEqual(-85.74, request.CreateOrderInformation.OrderRows[2].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[2].NumberOfUnits);
            Assert.AreEqual(16.64, request.CreateOrderInformation.OrderRows[2].VatPercent);
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
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber("194605092222")
                                                                                   .SetInitials("SB")
                                                                                   .SetBirthDate("19231212")
                                                                                   .SetName("Tess", "Testson")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("2222")
                                                                                   .SetLocality("Stan"))
                                                           .AddCustomerDetails(Item.CompanyCustomer()
                                                                                   .SetNationalIdNumber("666666")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("9999")
                                                                                   .SetLocality("Stan"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("1", request.CreateOrderInformation.OrderRows[2].ArticleNumber);
            Assert.AreEqual("FixedDiscount", request.CreateOrderInformation.OrderRows[2].Description);
            Assert.AreEqual(-85.74, request.CreateOrderInformation.OrderRows[2].PricePerUnit);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[2].NumberOfUnits);
            Assert.AreEqual(16.64, request.CreateOrderInformation.OrderRows[2].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[2].DiscountPercent);
        }

        [Test]
        public void TestInvoiceRequestObjectWithCreateOrderInformation()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(0)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddFee(Item.ShippingFee()
                                                                       .SetShippingId("33")
                                                                       .SetName("shipping")
                                                                       .SetDescription("Specification")
                                                                       .SetAmountExVat(50)
                                                                       .SetUnit("st")
                                                                       .SetVatPercent(25)
                                                                       .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber("194605092222")
                                                                                   .SetInitials("SB")
                                                                                   .SetBirthDate("19231212")
                                                                                   .SetName("Tess", "Testson")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("2222")
                                                                                   .SetLocality("Stan"))
                                                           .AddCustomerDetails(Item.CompanyCustomer()
                                                                                   .SetNationalIdNumber("194605092222")
                                                                                   .SetAddressSelector("ad33")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("2222")
                                                                                   .SetLocality("Stan"))
                                                           .SetCustomerReference("nr26")
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("2012-12-12", request.CreateOrderInformation.OrderDate.ToShortDateString());
            Assert.AreEqual("33", request.CreateOrderInformation.ClientOrderNumber);
            Assert.AreEqual(OrderType.Invoice, request.CreateOrderInformation.OrderType);
            Assert.AreEqual("nr26", request.CreateOrderInformation.CustomerReference);
            Assert.AreEqual("ad33", request.CreateOrderInformation.AddressSelector);
        }

        [Test]
        public void TestInvoiceRequestUsingAmountIncVatWithVatPercent()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddCustomerDetails(Item.CompanyCustomer()
                                                                                   .SetNationalIdNumber("194605092222")
                                                                                   .SetAddressSelector("ad33")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("2222")
                                                                                   .SetLocality("Stan"))
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetAmountIncVat(125)
                                                                            .SetDiscountPercent(0))
                                                           .AddFee(Item.ShippingFee()
                                                                       .SetShippingId("33")
                                                                       .SetName("shipping")
                                                                       .SetDescription("Specification")
                                                                       .SetAmountIncVat(62.50M)
                                                                       .SetUnit("st")
                                                                       .SetVatPercent(25)
                                                                       .SetDiscountPercent(0))
                                                           .AddFee(Item.InvoiceFee()
                                                                       .SetName("Svea fee")
                                                                       .SetDescription("Fee for invoice")
                                                                       .SetAmountIncVat(62.50M)
                                                                       .SetUnit("st")
                                                                       .SetVatPercent(25)
                                                                       .SetDiscountPercent(0))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
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
                                                             .AddOrderRow(Item.OrderRow()
                                                                              .SetArticleNumber("1")
                                                                              .SetQuantity(2)
                                                                              .SetAmountExVat(100.00M)
                                                                              .SetDescription("Specification")
                                                                              .SetName("Prod")
                                                                              .SetVatPercent(0)
                                                                              .SetDiscountPercent(0))
                                                             .AddOrderRow(Item.OrderRow()
                                                                              .SetArticleNumber("1")
                                                                              .SetQuantity(2)
                                                                              .SetAmountExVat(100.00M)
                                                                              .SetDescription("Specification")
                                                                              .SetName("Prod")
                                                                              .SetUnit("st")
                                                                              .SetVatPercent(25)
                                                                              .SetDiscountPercent(0))
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber("194605092222"))
                                                             .SetCountryCode(CountryCode.SE)
                                                             .SetOrderDate("2012-12-12")
                                                             .SetClientOrderNumber("33")
                                                             .SetCurrency(Currency.SEK)
                                                             .SetCustomerReference("33")
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestInvoiceRequestUsingAmountIncVatWithAmountExVat()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountIncVat(125)
                                                                            .SetAmountExVat(100)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetDiscountPercent(0))
                                                           .AddFee(Item.ShippingFee()
                                                                       .SetShippingId("33")
                                                                       .SetName("shipping")
                                                                       .SetDescription("Specification")
                                                                       .SetAmountIncVat(62.50M)
                                                                       .SetAmountExVat(50)
                                                                       .SetUnit("st")
                                                                       .SetDiscountPercent(0))
                                                           .AddFee(Item.InvoiceFee()
                                                                       .SetName("Svea fee")
                                                                       .SetDescription("Fee for invoice")
                                                                       .SetAmountIncVat(62.50M)
                                                                       .SetAmountExVat(50)
                                                                       .SetUnit("st")
                                                                       .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber("194605092222"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetCurrency(Currency.SEK)
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