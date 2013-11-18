using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
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
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            Assert.That(request.CreateOrderInformation.CustomerIdentity.NationalIdNumber, Is.EqualTo("194605092222"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.CountryCode, Is.EqualTo(CountryCode.SE.ToString().ToUpper()));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Individual));
        }

        [Test]
        public void TestInvoiceRequestObjectWithAuth()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

            Assert.That(request.Auth.Username, Is.EqualTo("sverigetest"));
            Assert.That(request.Auth.Password, Is.EqualTo("sverigetest"));
            Assert.That(request.Auth.ClientNumber, Is.EqualTo(79021));
        }

        [Test]
        public void TestSetAuth()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                           .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                           .SetCountryCode(CountryCode.NL)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.That(request.CreateOrderInformation.CustomerIdentity.Email, Is.EqualTo("test@svea.com"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.PhoneNumber, Is.EqualTo("0811111111"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.IpAddress, Is.EqualTo("123.123.123"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.FullName, Is.EqualTo("Tess Persson"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.Street, Is.EqualTo("Testgatan"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.CoAddress, Is.EqualTo("c/o Eriksson, Erik"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.ZipCode, Is.EqualTo("99999"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.HouseNumber, Is.EqualTo("1"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.Locality, Is.EqualTo("Stan"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.CountryCode, Is.EqualTo(CountryCode.NL.ToString().ToUpper()));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Individual));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.FirstName, Is.EqualTo("Tess"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.LastName, Is.EqualTo("Persson"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.Initials, Is.EqualTo("SB"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.IndividualIdentity.BirthDate, Is.EqualTo("19231212"));
        }

        [Test]
        public void TestCompanyIdRequest()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                           .AddCustomerDetails(Item.CompanyCustomer()
                                                                                   .SetNationalIdNumber("4354kj"))
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetClientOrderNumber(
                                                               TestingTool.DefaultTestClientOrderNumber)
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
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                           .AddCustomerDetails(TestingTool.CreateIndividualCustomerNl())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("2"))
                                                           .SetCountryCode(CountryCode.NL)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.That(request.CreateOrderInformation.CustomerIdentity.Email, Is.EqualTo("test@svea.com"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.PhoneNumber, Is.EqualTo("999999"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.IpAddress, Is.EqualTo("123.123.123"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.FullName, Is.EqualTo("Svea bakkerij 123"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.Street, Is.EqualTo("Gatan"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.CoAddress, Is.EqualTo("c/o Eriksson"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.ZipCode, Is.EqualTo("9999"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.HouseNumber, Is.EqualTo("23"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.Locality, Is.EqualTo("Stan"));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.CountryCode, Is.EqualTo(CountryCode.NL.ToString().ToUpper()));
            Assert.That(request.CreateOrderInformation.CustomerIdentity.CustomerType, Is.EqualTo(CustomerType.Individual));
        }

        [Test]
        public void TestInvoiceRequestObjectForCustomerIdentityCompanyFromSe()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                           .AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer())
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
        public void TestInvoiceRequestObjectForSEorderOnOneProductRow()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            Assert.That(request.CreateOrderInformation.OrderRows[2].DiscountPercent, Is.EqualTo(0));
        }

        [Test]
        public void TestInvoiceRequestObjectWithFixedDiscountOnDifferentProductVat()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
        public void TestInvoiceRequestObjectWithCreateOrderInformation()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
        public void TestInvoiceRequestUsingAmountIncVatWithAmountExVat()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
    }
}