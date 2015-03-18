using System;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Test.Webservice.Payment
{
    [TestFixture]  
    public class FixedDiscountRowsTest
    {
        private CreateOrderBuilder CreateMixedExvatAndIncvatOrderAndFeeRowsOrder() {
            var order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                .AddCustomerDetails(Item.IndividualCustomer().SetNationalIdNumber("194605092222"))
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
        [Test]  
        public void TestMixedOrderWithFixedDiscountAsExvatOnlyHasPriceIncludingVatFalse() {
            var order = CreateMixedExvatAndIncvatOrderAndFeeRowsOrder();
            order.
                AddDiscount(
                    Item.FixedDiscount()
                    .SetAmountExVat(10.0M)
                    //.setVatPercent(10)
                    .SetDiscountId("fixedDiscount")
                    .SetName("fixedDiscount: 10e")
                );

            var request = order.UseInvoicePayment().PrepareRequest();
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
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PricePerUnit, -6.67);//=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[4].VatPercent, 20);
            AssertEquals(request.CreateOrderInformation.OrderRows[4].PriceIncludingVat, false);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PricePerUnit, -3.33); //=WS
            AssertEquals(request.CreateOrderInformation.OrderRows[5].VatPercent, 10);
            AssertEquals(request.CreateOrderInformation.OrderRows[5].PriceIncludingVat, false);    
        }



        private static void AssertEquals<T1, T2>(T1 actual, T2 expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }


    }
}