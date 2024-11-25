using Webpay.Integration.Config;
using Webpay.Integration.Order.Create;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;

namespace Webpay.Integration.Test.Webservice.Payment;

[TestFixture]
public class FixedDiscountRowsTest
{
    private CreateOrderBuilder CreateMixedExvatAndIncvatOrderAndFeeRowsOrder()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddCustomerDetails(
                Item.IndividualCustomer()
                    .SetNationalIdNumber("194605092222")
            )
            .SetCountryCode(CountryCode.SE)
            .SetCustomerReference("33")
            .SetOrderDate(new DateTime(2012, 12, 12))
            .SetCurrency(Currency.SEK)
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountExVat(60.00M)
                    .SetVatPercent(20)
                    .SetQuantity(1)
                    .SetName("exvatRow")
            )
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountIncVat(33.00M)
                    .SetVatPercent(10)
                    .SetQuantity(1)
                    .SetName("incvatRow")
            )
            .AddFee(
                Item.InvoiceFee()
                    .SetAmountIncVat(8.80M)
                    .SetVatPercent(10)
                    .SetName("incvatInvoiceFee")
            )
            .AddFee(
                Item.ShippingFee()
                    .SetAmountExVat(16.00M)
                    .SetVatPercent(10)
                    .SetName("exvatShippingFee")
            );

        return order;
    }

    // TODO: cleanup
    // This will always be false since it will compare types.
    // Generics like <T1, T2> in this case would cause a type mismatch unless T1 and T2 are of the same type or compatible types that can be compared.
    //private static void AssertEquals<T1, T2>(T1 actual, T2 expected)
    //{
    //    Assert.That(actual, Is.EqualTo(expected));
    //}
    // Use a single generic type instead...
    //private static void AssertEquals<T>(T actual, T expected)
    //{
    //    Assert.That(actual, Is.EqualTo(expected));
    //}
    // Or:
    private static void AssertEquals<T1, T2>(T1 actual, T2 expected)
    {
        Assert.That(Convert.ToDouble(actual), Is.EqualTo(Convert.ToDouble(expected)));
    }

    private static CreateOrderBuilder CreateOnlyIncvatOrderAndFeeRowsOrder()
    {
        var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
            .AddCustomerDetails(
                Item.IndividualCustomer()
                    .SetNationalIdNumber("194605092222")
            )
            .SetCountryCode(CountryCode.SE)
            .SetCustomerReference("33")
            .SetOrderDate(new DateTime(2012, 12, 12))
            .SetCurrency(Currency.SEK)
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountIncVat(72.00M)
                    .SetVatPercent(20)
                    .SetQuantity(1)
                    .SetName("incvatRow")
            )
            .AddOrderRow(
                Item.OrderRow()
                    .SetAmountIncVat(33.00M)
                    .SetVatPercent(10)
                    .SetQuantity(1)
                    .SetName("incvatRow2")
            )
            .AddFee(
                Item.InvoiceFee()
                    .SetAmountIncVat(8.80M)
                    .SetVatPercent(10)
                    .SetName("incvatInvoiceFee")
            )
            .AddFee(
                Item.ShippingFee()
                    .SetAmountIncVat(17.60M)
                    .SetVatPercent(10)
                    .SetName("incvatShippingFee")
            );

        return order;
    }

    [Test]
    public void TestIncvatOrderRowAndShippingFeesOnlyHasPriceIncludingVatTrue()
    {
        var order = CreateOnlyIncvatOrderAndFeeRowsOrder();
        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, true);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, true);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, true);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, true);
    }

    [Test]
    public void TestIncvatOrderWithFixedDiscountAsExvatAndVatpercentHasPriceIncludingVatFalse()
    {
        var order = CreateOnlyIncvatOrderAndFeeRowsOrder();

        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountExVat(10.0M)
                .SetVatPercent(10)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 10e@10%")
        );

        const bool expectingPricesIncludingVatFalse = true;
        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, expectingPricesIncludingVatFalse);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, expectingPricesIncludingVatFalse);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, expectingPricesIncludingVatFalse);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, expectingPricesIncludingVatFalse);
        // Discount rows
        // Expected: fixedDiscount: 10exvat @10% = -11.0M0
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -11.0M);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, expectingPricesIncludingVatFalse);
        Assert.That(request.CreateOrderInformation.OrderRows.Length, Is.EqualTo(5));
    }

    [Test]
    public void TestIncvatOrderWithFixedDiscountAsExvatOnlyHasPriceIncludingVatTrue()
    {
        var order = CreateOnlyIncvatOrderAndFeeRowsOrder();

        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountExVat(10.0M)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 10e")
        );

        const bool expectPriceIncludingVatTrue = true;
        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, expectPriceIncludingVatTrue);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, expectPriceIncludingVatTrue);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, expectPriceIncludingVatTrue);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, expectPriceIncludingVatTrue);
        // Discount rows
        // Expected: fixedDiscount: 10 exvat => split across 10*60/(60+30) @20% => 6.6666...7*1.20=8 and  10*30/(60+30) @10% => 3.33333...*1.20 
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -8); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, expectPriceIncludingVatTrue);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -3.67); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, expectPriceIncludingVatTrue);
    }

    [Test]
    public void TestIncvatOrderWithFixedDiscountAsIncvatAndVatpercentHasPriceIncludingVatTrue()
    {
        var order = CreateOnlyIncvatOrderAndFeeRowsOrder();

        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountIncVat(11.0M)
                .SetVatPercent(10)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 11i@10%")
        );

        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, true);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, true);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, true);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, true);
        // Discount rows
        // Expected: fixedDiscount: 11incvat @10% = -11.0M0
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -11.00M);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, true);
        Assert.That(request.CreateOrderInformation.OrderRows.Length, Is.EqualTo(5));
    }

    [Test]
    public void TestIncvatOrderWithFixedDiscountAsIncvatOnlyHasPriceIncludingVatTrue()
    {
        var order = CreateOnlyIncvatOrderAndFeeRowsOrder();

        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountIncVat(10.0M)
                // .SetVatPercent(10)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 10i")
        );

        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, true);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, true);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, true);
        // invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, true);
        // Discount rows
        // Expected: fixedDiscount: 10 incvat => split across 10i *(72/72+33) @20% + 10i *(33/72+33) @10% => 6.8571i @20% + 3.1428i @10% =>5.71 + 2.86
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -6.86); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, true);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -3.14); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, true);
    }

    [Test]
    public void TestMixedOrderWithFixedDiscountAsExvatAndVatpercentHasPriceIncludingVatFalse()
    {
        var order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();

        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountExVat(10.0M)
                .SetVatPercent(10)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 10e@10%")
        );

        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
        // Discount rows
        // Expected: fixedDiscount: 11 incvat @10% => -10e @10% 
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -10);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
        Assert.That(request.CreateOrderInformation.OrderRows.Length, Is.EqualTo(5));
    }

    [Test]
    public void TestMixedOrderWithFixedDiscountAsExvatOnlyHasPriceIncludingVatFalse()
    {
        var order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();

        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountExVat(10.0M)
                // .SetVatPercent(10)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 10e")
        );

        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
        // Discount rows
        // Expected: fixedDiscount: 10 exvat => split across 10e *(60/60+30) @20% + 10e *(30/60+30) @10% => 6.67e @20% + 3.33e @10%
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -6.67); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -3.33); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, false);
    }

    [Test]
    public void TestMixedOrderWithFixedDiscountAsExvatOnlyHasPriceIncludingVatFalse2()
    {
        var order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();
        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountExVat(10.0M)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 10e")
        );

        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
        // Discount rows
        // Expected: fixedDiscount: 10 exvat => split across 10e *(60/60+30) @20% + 10e *(30/60+30) @10% => 6.67e @20% + 3.33e @10%
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -6.67); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -3.33); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, false);
    }

    [Test]
    public void TestMixedOrderWithFixedDiscountAsIncvatAndVatpercentHasPriceIncludingVatFalse()
    {
        var order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();

        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountIncVat(11.0M)
                .SetVatPercent(10)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 11i@10%")
        );

        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
        // Discount rows
        // Expected: fixedDiscount: 11 incvat @10% => -10e @10% 
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -10);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
        Assert.That(request.CreateOrderInformation.OrderRows.Length, Is.EqualTo(5));
    }

    [Test]
    public void TestMixedOrderWithFixedDiscountAsIncvatOnlyHasPriceIncludingVatFalse()
    {
        var order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();
        order.AddDiscount(
            Item.FixedDiscount()
                .SetAmountIncVat(10.0M)
                .SetDiscountId("fixedDiscount")
                .SetName("fixedDiscount: 10i")
        );

        var request = order.UseInvoicePayment().PrepareRequest();

        AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
        // Shipping fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
        // Invoice fee rows
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
        // Discount rows
        // Expected: fixedDiscount: 10 incvat => split across 10i *(72/72+33) @20% + 10i *(33/72+33) @10% => 6.8571i @20% + 3.1428i @10% =>5.71 + 2.86
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -5.71); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
        AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -2.86); //=WS
        AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
        AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, false);
    }
}