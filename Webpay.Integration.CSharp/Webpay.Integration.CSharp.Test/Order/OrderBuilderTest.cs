using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Test.Util;
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
            Assert.AreEqual(_order.GetIndividualCustomer().NationalIdNumber, "194605092222");
            Assert.AreEqual(_order.GetIndividualCustomer().GetFirstName(), "Tess");
            Assert.AreEqual(_order.GetIndividualCustomer().GetLastName(), "Persson");
            Assert.AreEqual(_order.GetIndividualCustomer().GetBirthDate(), "19231212");
            Assert.AreEqual(_order.GetIndividualCustomer().Email, "test@svea.com");
            Assert.AreEqual(_order.GetIndividualCustomer().PhoneNumber, "0811111111");
            Assert.AreEqual(_order.GetIndividualCustomer().IpAddress, "123.123.123");
            Assert.AreEqual(_order.GetIndividualCustomer().Street, "Testgatan");
            Assert.AreEqual(_order.GetIndividualCustomer().HouseNumber, "1");
            Assert.AreEqual(_order.GetIndividualCustomer().CoAddress, "c/o Eriksson, Erik");
            Assert.AreEqual(_order.GetIndividualCustomer().ZipCode, "99999");
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
            Assert.AreEqual(_order.GetInvoiceFeeRows()[0].GetDiscountPercent(), 0);
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
            _order.SetOrderDate(TestingTool.DefaultTestDate);

            Assert.AreEqual(TestingTool.DefaultTestDate, _order.GetOrderDate());
        }

        [Test]
        public void TestBuildOrderWithCountryCode()
        {
            _order.SetCountryCode(TestingTool.DefaultTestCountryCode);

            Assert.AreEqual(CountryCode.SE, _order.GetCountryCode());
        }

        [Test]
        public void TestBuildOrderWithCurrency()
        {
            _order.SetCurrency(TestingTool.DefaultTestCurrency);

            Assert.AreEqual("SEK", _order.GetCurrency());
        }

        [Test]
        public void TestBuildOrderWithClientOrderNumber()
        {
            _order.SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber);

            Assert.AreEqual(TestingTool.DefaultTestClientOrderNumber, _order.GetClientOrderNumber());
        }

        private static CreateOrderBuilder CreateTestCustomerIdentity(CreateOrderBuilder orderBuilder)
        {
            return orderBuilder.AddCustomerDetails(TestingTool.CreateIndividualCustomer());
        }

        private CreateOrderBuilder CreateCompanyDetails(CreateOrderBuilder orderBuilder)
        {
            return orderBuilder.AddCustomerDetails(TestingTool.CreateMiniCompanyCustomer());
        }

        private void CreateTestOrderRow()
        {
            _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow());
        }

        private void CreateShippingFeeRow()
        {
            _order.AddFee(TestingTool.CreateExVatBasedShippingFee());
        }

        private void CreateTestInvoiceFee()
        {
            _order.AddFee(TestingTool.CreateExVatBasedInvoiceFee());
        }

        private void CreateTestFixedDiscountRow()
        {
            _order.AddDiscount(Item.FixedDiscount()
                                   .SetDiscountId("1")
                                   .SetAmountIncVat(100.00M)
                                   .SetDescription("FixedDiscount"));
        }

        private void CreateTestRelativeDiscountBuilder()
        {
            _order.AddDiscount(TestingTool.CreateRelativeDiscount());
        }
    }
}