using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Order
{
    [TestFixture]
    public class OrderBuilderTest
    {
        private CreateOrderBuilder _order;

        [SetUp]
        public void SetUp()
        {
            _order = WebpayConnection.CreateOrder();
            _order.SetValidator(new VoidValidator());
        }

        [Test]
        public void TestThatValidatorIsCalledOnBuild()
        {
            _order.Build();
            var v = (VoidValidator) _order.GetValidator();
            Assert.AreEqual(1, v.NoOfCalls);
        }

        [Test]
        public void TestBuildEmptyOrder()
        {
            CreateOrderBuilder sveaRequest = _order
                .SetCountryCode(CountryCode.NL)
                .Build();

            Assert.AreEqual(sveaRequest.GetOrderRows().Count, 0);
            Assert.AreEqual(sveaRequest.GetFixedDiscountRows().Count, 0);
        }

        [Test]
        public void TestCustomerIdentity()
        {
            _order = CreateTestCustomerIdentity(_order);

            Assert.AreEqual(_order.GetIndividualCustomer().GetInitials(), "SB");
            Assert.AreEqual(_order.GetIndividualCustomer().NationalIdNumber, "194609052222");
            Assert.AreEqual(_order.GetIndividualCustomer().GetFirstName(), "Tess");
            Assert.AreEqual(_order.GetIndividualCustomer().GetLastName(), "Testson");
            Assert.AreEqual(_order.GetIndividualCustomer().GetBirthDate(), "19231212");
            Assert.AreEqual(_order.GetIndividualCustomer().Email, "test@svea.com");
            Assert.AreEqual(_order.GetIndividualCustomer().PhoneNumber, "999999");
            Assert.AreEqual(_order.GetIndividualCustomer().IpAddress, "123.123.123");
            Assert.AreEqual(_order.GetIndividualCustomer().Street, "Gatan");
            Assert.AreEqual(_order.GetIndividualCustomer().HouseNumber, "23");
            Assert.AreEqual(_order.GetIndividualCustomer().CoAddress, "c/o Eriksson");
            Assert.AreEqual(_order.GetIndividualCustomer().ZipCode, "9999");
            Assert.AreEqual(_order.GetIndividualCustomer().Locality, "Stan");
        }

        [Test]
        public void TestBuildCompanyDetails()
        {
            _order = CreateCompanyDetails(_order);

            Assert.AreEqual("TestCompagniet", _order.GetCompanyCustomer().GetCompanyName());
            Assert.AreEqual("2345234", _order.GetCompanyCustomer().GetVatNumber());
        }

        [Test]
        public void TestBuildOrderWithOneOrderRow()
        {
            CreateTestOrderRow();

            Assert.IsTrue(_order != null);
            Assert.AreEqual(_order.GetOrderRows()[0].GetArticleNumber(), "1");
            Assert.AreEqual(_order.GetOrderRows()[0].GetQuantity(), 2);
            Assert.AreEqual(_order.GetOrderRows()[0].GetAmountExVat(), 100);
            Assert.AreEqual(_order.GetOrderRows()[0].GetDescription(), "Specification");
            Assert.AreEqual(_order.GetOrderRows()[0].GetUnit(), "st");
            Assert.AreEqual(_order.GetOrderRows()[0].GetVatPercent(), 25);
            Assert.AreEqual(_order.GetOrderRows()[0].GetVatDiscount(), 0);
        }

        [Test]
        public void TestBuildOrderWithShippingFee()
        {
            CreateShippingFeeRow();

            Assert.AreEqual(_order.GetShippingFeeRows()[0].GetShippingId(), "33");
            Assert.AreEqual("Specification", _order.GetShippingFeeRows()[0].GetDescription());
            Assert.AreEqual(_order.GetShippingFeeRows()[0].GetAmountExVat(), 50);
            Assert.AreEqual(_order.GetShippingFeeRows()[0].GetVatPercent(), 25);
        }

        [Test]
        public void TestBuildWithInvoiceFee()
        {
            CreateTestInvoiceFee();

            Assert.AreEqual(_order.GetInvoiceFeeRows()[0].GetName(), "Svea fee");
            Assert.AreEqual(_order.GetInvoiceFeeRows()[0].GetDescription(), "Fee for invoice");
            Assert.AreEqual(_order.GetInvoiceFeeRows()[0].GetAmountExVat(), 50);
            Assert.AreEqual(_order.GetInvoiceFeeRows()[0].GetUnit(), "st");
            Assert.AreEqual(_order.GetInvoiceFeeRows()[0].GetVatPercent(), 25);
            Assert.AreEqual(_order.GetInvoiceFeeRows()[0].GetDiscountPercent(), 0, 0);
        }

        [Test]
        public void TestBuildOrderWithFixedDiscount()
        {
            CreateTestFixedDiscountRow();

            Assert.AreEqual("1", _order.GetFixedDiscountRows()[0].GetDiscountId());
            Assert.AreEqual(_order.GetFixedDiscountRows()[0].GetAmount(), 100);
            Assert.AreEqual("FixedDiscount", _order.GetFixedDiscountRows()[0].GetDescription());
        }

        [Test]
        public void TestBuildWithOrderWithRelativeDiscount()
        {
            CreateTestRelativeDiscountBuilder();

            Assert.AreEqual("1", _order.GetRelativeDiscountRows()[0].GetDiscountId());
            Assert.AreEqual(50, _order.GetRelativeDiscountRows()[0].GetDiscountPercent());
            Assert.AreEqual("RelativeDiscount", _order.GetRelativeDiscountRows()[0].GetDescription());
            Assert.AreEqual(_order.GetRelativeDiscountRows()[0].GetName(), "Relative");
            Assert.AreEqual(_order.GetRelativeDiscountRows()[0].GetUnit(), "st");
        }

        [Test]
        public void TestBuildOrderWithOrderDate()
        {
            _order.SetOrderDate("2012-12-12");

            Assert.AreEqual("2012-12-12", _order.GetOrderDate());
        }

        [Test]
        public void TestBuildOrderWithCountryCode()
        {
            _order.SetCountryCode(CountryCode.SE);

            Assert.AreEqual(CountryCode.SE, _order.GetCountryCode());
        }

        [Test]
        public void TestBuildOrderWithCurrency()
        {
            _order.SetCurrency(Currency.SEK);

            Assert.AreEqual("SEK", _order.GetCurrency());
        }

        [Test]
        public void TestBuildOrderWithClientOrderNumber()
        {
            _order.SetClientOrderNumber("33");

            Assert.AreEqual("33", _order.GetClientOrderNumber());
        }

        private static CreateOrderBuilder CreateTestCustomerIdentity(CreateOrderBuilder orderBuilder)
        {
            return orderBuilder.AddCustomerDetails(Item.IndividualCustomer()
                                                       .SetNationalIdNumber("194609052222")
                                                       .SetInitials("SB")
                                                       .SetBirthDate("19231212")
                                                       .SetName("Tess", "Testson")
                                                       .SetEmail("test@svea.com")
                                                       .SetPhoneNumber("999999")
                                                       .SetIpAddress("123.123.123")
                                                       .SetStreetAddress("Gatan", "23")
                                                       .SetCoAddress("c/o Eriksson")
                                                       .SetZipCode("9999")
                                                       .SetLocality("Stan"));
        }

        private CreateOrderBuilder CreateCompanyDetails(CreateOrderBuilder orderBuilder)
        {
            return orderBuilder.AddCustomerDetails(Item.CompanyCustomer()
                                                       .SetCompanyName("TestCompagniet")
                                                       .SetVatNumber("2345234"));
        }

        private void CreateTestOrderRow()
        {
            _order.AddOrderRow(Item.OrderRow()
                                   .SetArticleNumber("1")
                                   .SetQuantity(2)
                                   .SetAmountExVat(100.00M)
                                   .SetDescription("Specification")
                                   .SetUnit("st")
                                   .SetVatPercent(25)
                                   .SetVatDiscount(0));
        }

        private void CreateShippingFeeRow()
        {
            _order.AddFee(Item.ShippingFee()
                              .SetAmountExVat(50)
                              .SetShippingId("33")
                              .SetDescription("Specification")
                              .SetVatPercent(25));
        }

        private void CreateTestInvoiceFee()
        {
            _order.AddFee(Item.InvoiceFee()
                              .SetName("Svea fee")
                              .SetDescription("Fee for invoice")
                              .SetAmountExVat(50)
                              .SetUnit("st")
                              .SetVatPercent(25)
                              .SetDiscountPercent(0));
        }

        private void CreateTestFixedDiscountRow()
        {
            _order.AddDiscount(Item.FixedDiscount()
                                   .SetDiscountId("1")
                                   .SetAmountIncVat(100.00M)
                                   .SetAmountIncVat(100.00M)
                                   .SetDescription("FixedDiscount"));
        }

        private void CreateTestRelativeDiscountBuilder()
        {
            _order.AddDiscount(Item.RelativeDiscount()
                                   .SetDiscountId("1")
                                   .SetDiscountPercent(50)
                                   .SetDescription("RelativeDiscount")
                                   .SetName("Relative")
                                   .SetUnit("st"));
        }
    }
}