using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Hosted;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.Test.Hosted.Payment
{
    [TestFixture]
    public class HostedPaymentTest
    {
        [Test]
        public void TestCalculateRequestValuesNullExtraRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            Assert.AreEqual(500, payment.GetAmount());
        }

        [Test]
        public void TestVatPercentAndAmountIncVatCalculation()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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
            payment
                .SetReturnUrl("myUrl")
                .CalculateRequestValues();

            Assert.AreEqual(500, payment.GetAmount());
        }

        [Test]
        public void TestAmountIncVatAndvatPercentShippingFee()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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
            payment
                .SetReturnUrl("myUrl")
                .CalculateRequestValues();

            Assert.AreEqual(1000L, payment.GetAmount());
        }

        [Test]
        public void TestAmountIncVatAndAmountExVatCalculation()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                       .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                       .SetCurrency(TestingTool.DefaultTestCurrency)
                                                       .AddOrderRow(TestingTool.CreateMiniOrderRow());

            order.SetShippingFeeRows(null);
            order.SetFixedDiscountRows(null);
            order.SetRelativeDiscountRows(null);
            var payment = new FakeHostedPayment(order);
            payment
                .SetReturnUrl("myurl")
                .CalculateRequestValues();


            Assert.That(payment.GetAmount(), Is.EqualTo(500L));
        }

        [Test]
        public void TestCreatePaymentForm()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            PaymentForm form = payment.GetPaymentForm();

            var formHtmlFields = form.GetFormHtmlFields();
            Assert.AreEqual("</form>", formHtmlFields["form_end_tag"]);
        }

        [Test]
        public void TestExcludeInvoicesAndAllInstallmentsAllCountries()
        {
            var payment = new FakeHostedPayment(null);
            var exclude = new ExcludePayments();
            List<string> excludedPaymentMethod = payment.GetExcludedPaymentMethod();
            excludedPaymentMethod.AddRange(exclude.ExcludeInvoicesAndPaymentPlan());

            var expected = new List<string>
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

            CollectionAssert.AreEqual(expected, excludedPaymentMethod);
        }

        /*
         * 30*69.99*1.25 = 2624.625 => 2624.62 w/Bankers rounding (half-to-even)
         * problem, sums to 2624.7, in xml request, i.e. calculates 30* round( (69.99*1.25), 2) :( 
         */
        [Test]
        public void TestAmountFromMultipleItemsDefinedWithExVatAndVatPercent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetArticleNumber("0")
                                                                        .SetName("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                        .SetDescription("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                        .SetAmountExVat(69.99M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(30)
                                                                        .SetUnit("st"));

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(1, formatRowsList.Count);
            Assert.AreEqual(262462, formattedTotalAmount); // 262462,5 rounded half-to-even
            Assert.AreEqual(52492, formattedTotalVat); // 52492,5 rounded half-to-even
        }

        [Test]
        public void TestAmountFromMultipleItemsDefinedWithIncVatAndVatPercent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetArticleNumber("0")
                                                                        .SetName("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                        .SetDescription("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                        .SetAmountIncVat(87.4875M) // if low precision here, i.e. 87.49, we'll get a cumulative rounding error
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(30)
                                                                        .SetUnit("st"));

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(1, formatRowsList.Count);
            Assert.AreEqual(262462, formattedTotalAmount); // 262462,5 rounded half-to-even
            Assert.AreEqual(52492, formattedTotalVat); // 52492,5 rounded half-to-even
        }

        [Test]
        public void TestAmountFromMultipleItemsDefinedWithExVatAndIncVat()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetArticleNumber("0")
                                                                        .SetName("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                        .SetDescription("testCalculateRequestValuesCorrectTotalAmountFromMultipleItems")
                                                                        .SetAmountExVat(69.99M)
                                                                        .SetAmountIncVat(87.4875M) // if low precision here, i.e. 87.49, we'll get a cumulative rounding error
                                                                        .SetQuantity(30)
                                                                        .SetUnit("st"));

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(1, formatRowsList.Count);
            Assert.AreEqual(262462, formattedTotalAmount); // 262462,5 rounded half-to-even
            Assert.AreEqual(52492, formattedTotalVat); // 52492,5 rounded half-to-even
        }


        // calculated fixed discount vat rate, single vat rate in order
        [Test]
        public void TestAmountFromMultipleItemsWithFixedDiscountIncVatOnly()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(69.99M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(30))
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetAmountIncVat(10.00M));

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(2, formatRowsList.Count);
            Assert.AreEqual(261462, formattedTotalAmount); // 262462,5 - 1000 discount rounded half-to-even
            Assert.AreEqual(52292, formattedTotalVat); // 52492,5  -  200 discount (= 10/2624,62*524,92) rounded half-to-even
        }

        // explicit fixed discount vat rate, , single vat rate in order
        [Test]
        public void TestAmountFromMultipleItemsWithFixedDiscountIncVatAndVatPercent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(69.99M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(30))
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetAmountIncVat(12.50M)
                                                                        .SetVatPercent(25));

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(2, formatRowsList.Count);
            Assert.AreEqual(261212, formattedTotalAmount); // 262462,5 - 1250 discount rounded half-to-even
            Assert.AreEqual(52242, formattedTotalVat); // 52492,5 - 250 discount rounded half-to-even
        }

        // calculated fixed discount vat rate, multiple vat rate in order
        [Test]
        public void TestAmountWithFixedDiscountIncVatOnlyWithDifferentVatRatesPresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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
            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);

            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(3, formatRowsList.Count);
            // 100*250/356 = 70.22 incl. 25% vat => 14.04 vat as amount 
            // 100*106/356 = 29.78 incl. 6% vat => 1.69 vat as amount 
            // matches 15,73 discount (= 100/356 *56) discount
            Assert.AreEqual(25600, formattedTotalAmount); // 35600 - 10000 discount
            Assert.AreEqual(4027, formattedTotalVat); //  5600 -  1573 discount (= 10000/35600 *5600) discount
        }

        // explicit fixed discount vat rate, multiple vat rate in order
        [Test]
        public void TestAmountWithFixedDiscountIncVatAndVatPercentWithDifferentVatRatesPresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(3, formatRowsList.Count);
            Assert.AreEqual(23100, formattedTotalAmount); // 35600 - 12500 discount
            Assert.AreEqual(3100, formattedTotalVat); //  5600 -  2500 discount
        }

        [Test]
        public void TestAmountWithFixedDiscountExVatAndVatPercentWithDifferentVatRatesPresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(3, formatRowsList.Count);
            Assert.AreEqual(25600, formattedTotalAmount); // 35600 - 10000 discount
            Assert.AreEqual(5600, formattedTotalVat); //  5600 - 0 discount
        }

        [Test]
        public void TestAmountWithFixedDiscountExVatAndIncVatWithDifferentVatRatesPresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(3, formatRowsList.Count);
            Assert.AreEqual(25600, formattedTotalAmount); // 35600 - 10000 discount
            Assert.AreEqual(3600, formattedTotalVat); //  5600 - 2000 discount
        }

        // calculated relative discount vat rate, single vat rate in order
        [Test]
        public void TestAmountFromMultipleItemsWithRelativeDiscountWithDifferentVatRatesPresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(69.99M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(30))
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountPercent(25));

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(2, formatRowsList.Count);
            Assert.AreEqual(196847, formattedTotalAmount); // (262462,5  - 65615,625 discount (25%) rounded half-to-even
            Assert.AreEqual(39369, formattedTotalVat); //  52492,5  - 13123,125 discount (25%) rounded half-to-even
        }

        // calculated relative discount vat rate, single vat rate in order
        [Test]
        public void TestAmountFromMultipleItemsWithRelativeDiscountWithDifferentVatRatesPresent2()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(69.99M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountPercent(25));

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(2, formatRowsList.Count);
            Assert.AreEqual(6562, formattedTotalAmount); // 8748,75 - 2187,18 discount rounded half-to-even
            Assert.AreEqual(1312, formattedTotalVat); // 1749,75 - 437,5 discount (1750*.25) rounded half-to-even
        }

        // calculated relative discount vat rate, multiple vat rate in order
        [Test]
        public void TestAmountWithRelativeDiscountWithDifferentVatRatesPresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
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

            // follows HostedPayment calculateRequestValues() outline:
            var formatter = new HostedRowFormatter<CreateOrderBuilder>();

            List<HostedOrderRowBuilder> formatRowsList = formatter.FormatRows(order);
            long formattedTotalAmount = formatter.GetTotalAmount();
            long formattedTotalVat = formatter.GetTotalVat();

            Assert.AreEqual(3, formatRowsList.Count);
            // 5000*.25 = 1250
            // 600*.25 = 150  
            // matches 1400 discount
            Assert.AreEqual(26700, formattedTotalAmount); // 35600 - 8900 discount
            Assert.AreEqual(4200, formattedTotalVat); //  5600 - 1400 discount (= 10000/35600 *5600) discount
        }
    }
}