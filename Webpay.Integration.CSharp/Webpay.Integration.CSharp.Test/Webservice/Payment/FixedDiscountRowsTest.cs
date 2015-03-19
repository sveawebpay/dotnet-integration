using System;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Test.Webservice.Payment
{
    [TestFixture]
    public class FixedDiscountRowsTest
    {
        private CreateOrderBuilder CreateMixedExvatAndIncvatOrderAndFeeRowsOrder()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddCustomerDetails(
                                                           Item.IndividualCustomer().SetNationalIdNumber("194605092222"))
                                                       .SetCountryCode(CountryCode.SE)
                                                       .SetCustomerReference("33")
                                                       .SetOrderDate(new DateTime(2012, 12, 12))
                                                       .SetCurrency(Currency.SEK)
                                                       .AddOrderRow(
                                                           Item
                                                               .OrderRow().SetAmountExVat(60.00M)
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
                )
                ;
            return order;
        }

        // same order with discount exvat should be sent with PriceIncludingVat = false but with split discount rows based on order amounts ex vat


        private static void AssertEquals<T1, T2>(T1 actual, T2 expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }


        // same order with discount exvat should be sent with PriceIncludingVat = false but with split discount rows based on order amounts ex vat

        private static CreateOrderBuilder CreateOnlyIncvatOrderAndFeeRowsOrder()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddCustomerDetails(
                                                           Item.IndividualCustomer().SetNationalIdNumber("194605092222"))
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
                )
                ;
            return order;
        }

        // order with order/fee rows all having incvat should be sent with PriceIncludingVat = true
        [Test]
        public void TestIncvatOrderRowAndShippingFeesOnlyHasPriceIncludingVatTrue()
        {
            CreateOrderBuilder order = CreateOnlyIncvatOrderAndFeeRowsOrder();
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, true);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, true);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, true);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, true);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }

        // same order with discount exvat should be sent with PriceIncludingVat = true but with split discount rows based on order amounts ex vat

        // same order with discount exvat+vat should be sent with PriceIncludingVat = true with one discount row amount based on given exvat + vat
        [Test]
        public void TestIncvatOrderWithFixedDiscountAsExvatAndVatpercentHasPriceIncludingVatTrue()
        {
            CreateOrderBuilder order = CreateOnlyIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountExVat(10.0M)
                        .SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 10e@10%")
                )
                ;
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, true);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, true);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, true);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, true);
            // all discount rows
            // expected: fixedDiscount: 10exvat @10% = -11.0M0
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -11.0M);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, true);
            Assert.That(request.CreateOrderInformation.OrderRows[5], Is.Null);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }

        [Test]
        public void TestIncvatOrderWithFixedDiscountAsExvatOnlyHasPriceIncludingVatTrue()
        {
            CreateOrderBuilder order = CreateOnlyIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountExVat(10.0M)
                        //.SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 10e")
                )
                ;
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, true);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, true);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, true);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, true);
            // all discount rows
            // expected: fixedDiscount: 10 exvat => split across 10e *(60/60+30) @20% + 10e *(30/60+30) @10% => 6.6666e @20% + 3.3333e @10% => 8.00i + 3.67i
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -8.00); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, true);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -3.67); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, true);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }

        // same order with discount incvat+vat should be sent with PriceIncludingVat = true with one discount row amount based on given incvat + vat
        [Test]
        public void TestIncvatOrderWithFixedDiscountAsIncvatAndVatpercentHasPriceIncludingVatTrue()
        {
            CreateOrderBuilder order = CreateOnlyIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountIncVat(11.0M)
                        .SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 11i@10%")
                )
                ;
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, true);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, true);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, true);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, true);
            // all discount rows
            // expected: fixedDiscount: 11incvat @10% = -11.0M0
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -11.00M);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, true);
            Assert.That(request.CreateOrderInformation.OrderRows[5], Is.Null);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }

        [Test]
        public void TestIncvatOrderWithFixedDiscountAsIncvatOnlyHasPriceIncludingVatTrue()
        {
            CreateOrderBuilder order = CreateOnlyIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountIncVat(10.0M)
                        //.SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 10i")
                )
                ;
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 72.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, true);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 33.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, true);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 17.60);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, true);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.80);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, true);
            // all discount rows
            // expected: fixedDiscount: 10 incvat => split across 10i *(72/72+33) @20% + 10i *(33/72+33) @10% => 6.8571i @20% + 3.1428i @10% =>5.71 + 2.86
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -6.86); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, true);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -3.14); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, true);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }

        [Test]
        public void TestMixedOrderWithFixedDiscountAsExvatAndVatpercentHasPriceIncludingVatFalse()
        {
            CreateOrderBuilder order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountExVat(10.0M)
                        .SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 10e@10%")
                )
                ;
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
            // all discount rows
            // expected: fixedDiscount: 11 incvat @10% => -10e @10% 
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -10);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
            Assert.That(request.CreateOrderInformation.OrderRows[5], Is.Null);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }

        [Test]
        public void TestMixedOrderWithFixedDiscountAsExvatOnlyHasPriceIncludingVatFalse()
        {
            CreateOrderBuilder order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountExVat(10.0M)
                        //.SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 10e")
                );

            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
            // all discount rows
            // expected: fixedDiscount: 10 exvat => split across 10e *(60/60+30) @20% + 10e *(30/60+30) @10% => 6.67e @20% + 3.33e @10%
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
            CreateOrderBuilder order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountExVat(10.0M)
                        //.SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 10e")
                )
                ;
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
            // all discount rows
            // expected: fixedDiscount: 10 exvat => split across 10e *(60/60+30) @20% + 10e *(30/60+30) @10% => 6.67e @20% + 3.33e @10%
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -6.67); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -3.33); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, false);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }

        [Test]
        public void TestMixedOrderWithFixedDiscountAsIncvatAndVatpercentHasPriceIncludingVatFalse()
        {
            CreateOrderBuilder order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountIncVat(11.0M)
                        .SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 11i@10%")
                )
                ;
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
            // all discount rows
            // expected: fixedDiscount: 11 incvat @10% => -10e @10% 
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -10);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
            Assert.That(request.CreateOrderInformation.OrderRows[5], Is.Null);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }

        [Test]
        public void TestMixedOrderWithFixedDiscountAsIncvatOnlyHasPriceIncludingVatFalse()
        {
            CreateOrderBuilder order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                        .SetAmountIncVat(10.0M)
                        //.SetVatPercent(10)
                        .SetDiscountId("fixedDiscount")
                        .SetName("fixedDiscount: 10i")
                )
                ;
            CreateOrderEuRequest request = order.UseInvoicePayment().PrepareRequest();
            // all order rows
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PricePerUnit, 60.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[0].PriceIncludingVat, false);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PricePerUnit, 30.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[1].PriceIncludingVat, false);
            // all shipping fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PricePerUnit, 16.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[2].PriceIncludingVat, false);
            // all invoice fee rows
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PricePerUnit, 8.00);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[3].PriceIncludingVat, false);
            // all discount rows
            // expected: fixedDiscount: 10 incvat => split across 10i *(72/72+33) @20% + 10i *(33/72+33) @10% => 6.8571i @20% + 3.1428i @10% =>5.71 + 2.86
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -5.71); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -2.86); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, false);

            // See file IntegrationTest/WebService/Payment/FixedDiscountRowsIntegrationTest for service response tests.
        }
    }
}