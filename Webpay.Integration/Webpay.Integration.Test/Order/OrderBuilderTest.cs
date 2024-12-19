using Webpay.Integration.Config;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.Test.Order;

[TestFixture]
public class OrderBuilderTest
{
    private CreateOrderBuilder _order;

    [SetUp]
    public void SetUp()
    {
        _order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig());
        _order.SetValidator(new VoidValidator());
    }

    [Test]
    public void TestThatValidatorIsCalledOnBuild()
    {
        _order.Build();
        var v = (VoidValidator) _order.GetValidator();
        Assert.That(v.NoOfCalls, Is.EqualTo(1));
    }

    [Test]
    public void TestBuildEmptyOrder()
    {
        var sveaRequest = _order
            .SetCountryCode(CountryCode.NL)
            .Build();

        Assert.That(sveaRequest.GetOrderRows().Count, Is.EqualTo(0));
        Assert.That(sveaRequest.GetFixedDiscountRows().Count, Is.EqualTo(0));
    }

    [Test]
    public void TestCustomerIdentity()
    {
        _order = CreateTestCustomerIdentity(_order);

        Assert.That(_order.GetIndividualCustomer().GetInitials(), Is.EqualTo("SB"));
        Assert.That(_order.GetIndividualCustomer().NationalIdNumber, Is.EqualTo("194605092222"));
        Assert.That(_order.GetIndividualCustomer().GetFirstName(), Is.EqualTo("Tess"));
        Assert.That(_order.GetIndividualCustomer().GetLastName(), Is.EqualTo("Persson"));
        Assert.That(_order.GetIndividualCustomer().GetBirthDate(), Is.EqualTo("19231212"));
        Assert.That(_order.GetIndividualCustomer().Email, Is.EqualTo("test@svea.com"));
        Assert.That(_order.GetIndividualCustomer().PhoneNumber, Is.EqualTo("0811111111"));
        Assert.That(_order.GetIndividualCustomer().IpAddress, Is.EqualTo("123.123.123"));
        Assert.That(_order.GetIndividualCustomer().Street, Is.EqualTo("Testgatan"));
        Assert.That(_order.GetIndividualCustomer().HouseNumber, Is.EqualTo("1"));
        Assert.That(_order.GetIndividualCustomer().CoAddress, Is.EqualTo("c/o Eriksson, Erik"));
        Assert.That(_order.GetIndividualCustomer().ZipCode, Is.EqualTo("99999"));
        Assert.That(_order.GetIndividualCustomer().Locality, Is.EqualTo("Stan"));
    }

    [Test]
    public void TestBuildCompanyDetails()
    {
        _order = CreateCompanyDetails(_order);

        Assert.That(_order.GetCompanyCustomer().GetCompanyName(), Is.EqualTo("TestCompagniet"));
        Assert.That(_order.GetCompanyCustomer().GetVatNumber(), Is.EqualTo("2345234"));
    }

    [Test]
    public void TestBuildOrderWithOneOrderRow()
    {
        CreateTestOrderRow();

        Assert.That(_order, Is.Not.Null);
        Assert.That(_order.GetOrderRows()[0].GetArticleNumber(), Is.EqualTo("1"));
        Assert.That(_order.GetOrderRows()[0].GetQuantity(), Is.EqualTo(2));
        Assert.That(_order.GetOrderRows()[0].GetAmountExVat(), Is.EqualTo(100));
        Assert.That(_order.GetOrderRows()[0].GetDescription(), Is.EqualTo("Specification"));
        Assert.That(_order.GetOrderRows()[0].GetUnit(), Is.EqualTo("st"));
        Assert.That(_order.GetOrderRows()[0].GetVatPercent(), Is.EqualTo(25));
        Assert.That(_order.GetOrderRows()[0].GetVatDiscount(), Is.EqualTo(0));
    }

    [Test]
    public void TestBuildOrderWithShippingFee()
    {
        CreateShippingFeeRow();

        Assert.That(_order.GetShippingFeeRows()[0].GetShippingId(), Is.EqualTo("33"));
        Assert.That(_order.GetShippingFeeRows()[0].GetDescription(), Is.EqualTo("Specification"));
        Assert.That(_order.GetShippingFeeRows()[0].GetAmountExVat(), Is.EqualTo(50));
        Assert.That(_order.GetShippingFeeRows()[0].GetVatPercent(), Is.EqualTo(25));
    }

    [Test]
    public void TestBuildWithInvoiceFee()
    {
        CreateTestInvoiceFee();

        Assert.That(_order.GetInvoiceFeeRows()[0].GetName(), Is.EqualTo("Svea fee"));
        Assert.That(_order.GetInvoiceFeeRows()[0].GetDescription(), Is.EqualTo("Fee for invoice"));
        Assert.That(_order.GetInvoiceFeeRows()[0].GetAmountExVat(), Is.EqualTo(50));
        Assert.That(_order.GetInvoiceFeeRows()[0].GetUnit(), Is.EqualTo("st"));
        Assert.That(_order.GetInvoiceFeeRows()[0].GetVatPercent(), Is.EqualTo(25));
        Assert.That(_order.GetInvoiceFeeRows()[0].GetDiscountPercent(), Is.EqualTo(0));
    }

    [Test]
    public void TestBuildOrderWithFixedDiscount()
    {
        CreateTestFixedDiscountRow();

        Assert.That(_order.GetFixedDiscountRows()[0].GetDiscountId(), Is.EqualTo("1"));
        Assert.That(_order.GetFixedDiscountRows()[0].GetAmountIncVat(), Is.EqualTo(100));
        Assert.That(_order.GetFixedDiscountRows()[0].GetDescription(), Is.EqualTo("FixedDiscount"));
    }

    [Test]
    public void TestBuildWithOrderWithRelativeDiscount()
    {
        CreateTestRelativeDiscountBuilder();

        Assert.That(_order.GetRelativeDiscountRows()[0].GetDiscountId(), Is.EqualTo("1"));
        Assert.That(_order.GetRelativeDiscountRows()[0].GetDiscountPercent(), Is.EqualTo(50));
        Assert.That(_order.GetRelativeDiscountRows()[0].GetDescription(), Is.EqualTo("RelativeDiscount"));
        Assert.That(_order.GetRelativeDiscountRows()[0].GetName(), Is.EqualTo("Relative"));
        Assert.That(_order.GetRelativeDiscountRows()[0].GetUnit(), Is.EqualTo("st"));
    }

    [Test]
    public void TestBuildOrderWithOrderDate()
    {
        _order.SetOrderDate(TestingTool.DefaultTestDate);

        Assert.That(_order.GetOrderDate(), Is.EqualTo(TestingTool.DefaultTestDate));
    }

    [Test]
    public void TestBuildOrderWithCountryCode()
    {
        _order.SetCountryCode(TestingTool.DefaultTestCountryCode);

        Assert.That(_order.GetCountryCode(), Is.EqualTo(CountryCode.SE));
    }

    [Test]
    public void TestBuildOrderWithCurrency()
    {
        _order.SetCurrency(TestingTool.DefaultTestCurrency);

        Assert.That(_order.GetCurrency(), Is.EqualTo("SEK"));
    }

    [Test]
    public void TestBuildOrderWithClientOrderNumber()
    {
        _order.SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber);

        Assert.That(_order.GetClientOrderNumber(), Is.EqualTo(TestingTool.DefaultTestClientOrderNumber));
    }

    private static CreateOrderBuilder CreateTestCustomerIdentity(CreateOrderBuilder orderBuilder)
    {
        return orderBuilder.AddCustomerDetails(TestingTool.CreateIndividualCustomer());
    }

    private CreateOrderBuilder CreateCompanyDetails(CreateOrderBuilder orderBuilder)
    {
        return orderBuilder.AddCustomerDetails(TestingTool.CreateMinimalCompanyCustomer());
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
        _order.AddDiscount(
            Item.FixedDiscount()
                .SetDiscountId("1")
                .SetAmountIncVat(100.00M)
                .SetDescription("FixedDiscount")
        );
    }

    private void CreateTestRelativeDiscountBuilder()
    {
        _order.AddDiscount(TestingTool.CreateRelativeDiscount());
    }
}