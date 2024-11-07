using Webpay.Integration.Config;
using Webpay.Integration.Hosted;
using Webpay.Integration.Hosted.Helper;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;

namespace Webpay.Integration.Test.Hosted.Payment;

[TestFixture]
public class HostedPaymentTest
{
    [Test]
    public void TestCalculateRequestValuesNullExtraRows()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                   .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                   .SetCurrency(TestingTool.DefaultTestCurrency)
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(4)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(1))
                                                   .AddFee(Item.ShippingFee())
                                                   .AddDiscount(Item.FixedDiscount())
                                                   .AddDiscount(Item.RelativeDiscount());

        var payment = new FakeHostedPayment(order);
        payment
            .SetReturnUrl("myurl")
            .CalculateRequestValues();

        Assert.That(payment.GetAmount(), Is.EqualTo(500));
    }

    [Test]
    public void TestVatPercentAndAmountIncVatCalculation()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                   .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                   .SetCurrency(TestingTool.DefaultTestCurrency)
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountIncVat(5)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(1));

        order.SetShippingFeeRows(null);
        order.SetFixedDiscountRows(null);
        order.SetRelativeDiscountRows(null);
        var payment = new FakeHostedPayment(order);
        payment.SetReturnUrl("myUrl").CalculateRequestValues();

        Assert.That(payment.GetAmount(), Is.EqualTo(500));
    }

    [Test]
    public void TestAmountIncVatAndvatPercentShippingFee()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                    .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                    .SetCurrency(TestingTool.DefaultTestCurrency)
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetAmountIncVat(5)
                                                     .SetVatPercent(25)
                                                     .SetQuantity(1))
                                    .AddFee(Item.ShippingFee()
                                                .SetAmountIncVat(5)
                                                .SetVatPercent(25));

        order.SetFixedDiscountRows(null);
        order.SetRelativeDiscountRows(null);

        var payment = new FakeHostedPayment(order);
        payment.SetReturnUrl("myUrl").CalculateRequestValues();

        Assert.That(payment.GetAmount(), Is.EqualTo(1000L));
    }

    [Test]
    public void TestAmountIncVatAndAmountExVatCalculation()
    {
        CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                   .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                   .SetCurrency(TestingTool.DefaultTestCurrency)
                                                   .AddOrderRow(TestingTool.CreateMiniOrderRow());

        order.SetShippingFeeRows(null);
        order.SetFixedDiscountRows(null);
        order.SetRelativeDiscountRows(null);
        var payment = new FakeHostedPayment(order);
        payment.SetReturnUrl("myurl").CalculateRequestValues();

        Assert.That(payment.GetAmount(), Is.EqualTo(500L));
    }

    [Test]
    public void TestCreatePaymentForm()
    {
        CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                   .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                   .SetCurrency(TestingTool.DefaultTestCurrency)
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(4)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(1))
                                                   .AddCustomerDetails(TestingTool.CreateCompanyCustomer());

        var payment = new FakeHostedPayment(order);
        payment.SetReturnUrl("myurl");
        var form = payment.GetPaymentForm();

        var formHtmlFields = form.GetFormHtmlFields();
        Assert.That(formHtmlFields["form_end_tag"], Is.EqualTo("</form>"));
    }

    [Test]
    public void TestExcludeInvoicesAndAllInstallmentsAllCountries()
    {
        var payment = new FakeHostedPayment(CreateOrderBuilder());
        var exclude = new ExcludePayments();
        var excludedPaymentMethod = payment.GetExcludedPaymentMethod();
        excludedPaymentMethod.AddRange(exclude.ExcludeInvoicesAndPaymentPlan());

        var expectedValues = new List<string>
        {
            InvoiceType.INVOICESE.Value,
            InvoiceType.INVOICEEUSE.Value,
            PaymentPlanType.PAYMENTPLANSE.Value,
            PaymentPlanType.PAYMENTPLANEUSE.Value,
            InvoiceType.INVOICEDE.Value,
            PaymentPlanType.PAYMENTPLANDE.Value,
            InvoiceType.INVOICEDK.Value,
            PaymentPlanType.PAYMENTPLANDK.Value,
            InvoiceType.INVOICEFI.Value,
            PaymentPlanType.PAYMENTPLANFI.Value,
            InvoiceType.INVOICENL.Value,
            PaymentPlanType.PAYMENTPLANNL.Value,
            InvoiceType.INVOICENO.Value,
            PaymentPlanType.PAYMENTPLANNO.Value
        };

        Assert.That(excludedPaymentMethod, Is.EqualTo(expectedValues));
    }

    /*
     * 30*69.99*1.25 = 2624.625 => 2624.62 w/Bankers rounding (half-to-even)
     * problem, sums to 2624.7, in xml request, i.e. calculates 30* round( (69.99*1.25), 2) :( 
     */
    [Test]
    public void TestAmountFromMultipleItemsDefinedWithExVatAndVatPercent()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetArticleNumber("0")
                                                                    .SetName("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                    .SetDescription("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                    .SetAmountExVat(69.99M)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(30)
                                                                    .SetUnit("st"));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(1));
        Assert.That(formattedTotalAmount, Is.EqualTo(262462)); // 262462,5 rounded half-to-even
        Assert.That(formattedTotalVat, Is.EqualTo(52492)); // 52492,5 rounded half-to-even
    }

    [Test]
    public void TestAmountFromMultipleItemsDefinedWithIncVatAndVatPercent()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetArticleNumber("0")
                                                                    .SetName("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                    .SetDescription("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                    .SetAmountIncVat(87.4875M) // If low precision here, i.e. 87.49, we'll get a cumulative rounding error
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(30)
                                                                    .SetUnit("st"));

        // Follows HostedPayment calculateRequestValues() outline:
        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(1));
        Assert.That(formattedTotalAmount, Is.EqualTo(262462)); // 262462,5 rounded half-to-even
        Assert.That(formattedTotalVat, Is.EqualTo(52492)); // 52492,5 rounded half-to-even
    }

    [Test]
    public void TestAmountFromMultipleItemsDefinedWithExVatAndIncVat()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetArticleNumber("0")
                                                                    .SetName("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                    .SetDescription("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                    .SetAmountExVat(69.99M)
                                                                    .SetAmountIncVat(87.4875M) // If low precision here, i.e. 87.49, we'll get a cumulative rounding error
                                                                    .SetQuantity(30)
                                                                    .SetUnit("st"));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(1));
        Assert.That(formattedTotalAmount, Is.EqualTo(262462)); // 262462,5 rounded half-to-even
        Assert.That(formattedTotalVat, Is.EqualTo(52492)); // 52492,5 rounded half-to-even
    }


    // Calculated fixed discount vat rate, single vat rate in order
    [Test]
    public void TestAmountFromMultipleItemsWithFixedDiscountIncVatOnly()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetAmountExVat(69.99M)
                                                     .SetVatPercent(25)
                                                     .SetQuantity(30))
                                    .AddDiscount(Item.FixedDiscount()
                                                     .SetAmountIncVat(10.00M));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(2));
        Assert.That(formattedTotalAmount, Is.EqualTo(261462)); // 262462,5 - 1000 discount rounded half-to-even
        Assert.That(formattedTotalVat, Is.EqualTo(52292)); // 52492,5  -  200 discount (= 10/2624,62*524,92) rounded half-to-even
    }

    // Explicit fixed discount vat rate, , single vat rate in order
    [Test]
    public void TestAmountFromMultipleItemsWithFixedDiscountIncVatAndVatPercent()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(69.99M)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(30))
                                                   .AddDiscount(Item.FixedDiscount()
                                                                    .SetAmountIncVat(12.50M)
                                                                    .SetVatPercent(25));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(2));
        Assert.That(formattedTotalAmount, Is.EqualTo(261212)); // 262462,5 - 1250 discount rounded half-to-even
        Assert.That(formattedTotalVat, Is.EqualTo(52242)); // 52492,5 - 250 discount rounded half-to-even
    }

    // Calculated fixed discount vat rate, multiple vat rate in order
    [Test]
    public void TestAmountWithFixedDiscountIncVatOnlyWithDifferentVatRatesPresent()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetAmountExVat(100.00M)
                                                     .SetVatPercent(25)
                                                     .SetQuantity(2))
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetAmountExVat(100.00M)
                                                     .SetVatPercent(6)
                                                     .SetQuantity(1))
                                    .AddDiscount(Item.FixedDiscount()
                                                     .SetAmountIncVat(100.00M));

        // follows HostedPayment calculateRequestValues() outline:
        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(3));
        // 100*250/356 = 70.22 incl. 25% vat => 14.04 vat as amount 
        // 100*106/356 = 29.78 incl. 6% vat => 1.69 vat as amount 
        // matches 15,73 discount (= 100/356 *56) discount
        Assert.That(formattedTotalAmount, Is.EqualTo(25600)); // 35600 - 10000 discount
        Assert.That(formattedTotalVat, Is.EqualTo(4027)); //  5600 -  1573 discount (= 10000/35600 *5600) discount
    }

    // Explicit fixed discount vat rate, multiple vat rate in order
    [Test]
    public void TestAmountWithFixedDiscountIncVatAndVatPercentWithDifferentVatRatesPresent()
    {
        CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(100.00M)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(2))
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(100.00M)
                                                                    .SetVatPercent(6)
                                                                    .SetQuantity(1))
                                                   .AddDiscount(Item.FixedDiscount()
                                                                    .SetAmountIncVat(125.00M)
                                                                    .SetVatPercent(25));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(3));
        Assert.That(formattedTotalAmount, Is.EqualTo(23100)); // 35600 - 12500 discount
        Assert.That(formattedTotalVat, Is.EqualTo(3100)); //  5600 -  2500 discount
    }

    [Test]
    public void TestAmountWithFixedDiscountExVatAndVatPercentWithDifferentVatRatesPresent()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetAmountExVat(100.00M)
                                                     .SetVatPercent(25)
                                                     .SetQuantity(2))
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetAmountExVat(100.00M)
                                                     .SetVatPercent(6)
                                                     .SetQuantity(1))
                                    .AddDiscount(Item.FixedDiscount()
                                                     .SetAmountExVat(100.00M)
                                                     .SetVatPercent(0));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(3));
        Assert.That(formattedTotalAmount, Is.EqualTo(25600)); // 35600 - 10000 discount
        Assert.That(formattedTotalVat, Is.EqualTo(5600)); //  5600 - 0 discount
    }

    [Test]
    public void TestAmountWithFixedDiscountExVatAndIncVatWithDifferentVatRatesPresent()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(100.00M)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(2))
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(100.00M)
                                                                    .SetVatPercent(6)
                                                                    .SetQuantity(1))
                                                   .AddDiscount(Item.FixedDiscount()
                                                                    .SetAmountExVat(80.00M)
                                                                    .SetAmountIncVat(100.00M));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(3));
        Assert.That(formattedTotalAmount, Is.EqualTo(25600)); // 35600 - 10000 discount
        Assert.That(formattedTotalVat, Is.EqualTo(3600)); //  5600 - 2000 discount
    }

    // Calculated relative discount vat rate, single vat rate in order
    [Test]
    public void TestAmountFromMultipleItemsWithRelativeDiscountWithDifferentVatRatesPresent()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                    .AddOrderRow(Item.OrderRow()
                                                     .SetAmountExVat(69.99M)
                                                     .SetVatPercent(25)
                                                     .SetQuantity(30))
                                    .AddDiscount(Item.RelativeDiscount()
                                                     .SetDiscountPercent(25));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(2));
        Assert.That(formattedTotalAmount, Is.EqualTo(196847)); // (262462,5  - 65615,625 discount (25%) rounded half-to-even
        Assert.That(formattedTotalVat, Is.EqualTo(39369)); //  52492,5  - 13123,125 discount (25%) rounded half-to-even
    }

    // Calculated relative discount vat rate, single vat rate in order
    [Test]
    public void TestAmountFromMultipleItemsWithRelativeDiscountWithDifferentVatRatesPresent2()
    {
        CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(69.99M)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(1))
                                                   .AddDiscount(Item.RelativeDiscount()
                                                                    .SetDiscountPercent(25));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(2));
        Assert.That(formattedTotalAmount, Is.EqualTo(6562)); // 8748,75 - 2187,18 discount rounded half-to-even
        Assert.That(formattedTotalVat, Is.EqualTo(1312)); // 1749,75 - 437,5 discount (1750*.25) rounded half-to-even
    }

    // Calculated relative discount vat rate, multiple vat rate in order
    [Test]
    public void TestAmountWithRelativeDiscountWithDifferentVatRatesPresent()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(100.00M)
                                                                    .SetVatPercent(25)
                                                                    .SetQuantity(2))
                                                   .AddOrderRow(Item.OrderRow()
                                                                    .SetAmountExVat(100.00M)
                                                                    .SetVatPercent(6)
                                                                    .SetQuantity(1))
                                                   .AddDiscount(Item.RelativeDiscount()
                                                                    .SetDiscountPercent(25));

        var formatter = new HostedRowFormatter<CreateOrderBuilder>();
        var formatRowsList = formatter.FormatRows(order);
        var formattedTotalAmount = formatter.GetTotalAmount();
        var formattedTotalVat = formatter.GetTotalVat();

        Assert.That(formatRowsList.Count, Is.EqualTo(3));
        // 5000*.25 = 1250
        // 600*.25 = 150  
        // matches 1400 discount
        Assert.That(formattedTotalAmount, Is.EqualTo(26700)); // 35600 - 8900 discount
        Assert.That(formattedTotalVat, Is.EqualTo(4200)); //  5600 - 1400 discount (= 10000/35600 *5600) discount
    }

    public CreateOrderBuilder CreateOrderBuilder()
    {
        return WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                               .SetCurrency(TestingTool.DefaultTestCurrency)
                               .AddOrderRow(Item.OrderRow()
                                                .SetQuantity(1));
    }
}