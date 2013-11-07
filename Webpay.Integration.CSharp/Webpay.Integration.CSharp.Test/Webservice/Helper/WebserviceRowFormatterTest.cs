using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;
using Webpay.Integration.CSharp.Webservice.Helper;

namespace Webpay.Integration.CSharp.Test.Webservice.Helper
{
    [TestFixture]
    public class WebserviceRowFormatterTest
    {
        [Test]
        public void TestFormatOrderRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetArticleNumber("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            Assert.AreEqual("0", newRows[0].ArticleNumber);
            Assert.AreEqual("Tess: Tester", newRows[0].Description);
            Assert.AreEqual(4.0, newRows[0].PricePerUnit);
            Assert.AreEqual(25.0, newRows[0].VatPercent);
            Assert.AreEqual(0, newRows[0].DiscountPercent);
            Assert.AreEqual(1, newRows[0].NumberOfUnits);
            Assert.AreEqual("st", newRows[0].Unit);
        }

        [Test]
        public void TestFormatShippingFeeRows()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetVatPercent(0)
                                                                            .SetDiscountPercent(0))
                                                           .AddFee(Item.ShippingFee()
                                                                       .SetShippingId("0")
                                                                       .SetName("Tess")
                                                                       .SetDescription("Tester")
                                                                       .SetAmountExVat(4)
                                                                       .SetVatPercent(25)
                                                                       .SetUnit("st"))
                                                           .AddCustomerDetails(Item.IndividualCustomer()
                                                                                   .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("0", request.CreateOrderInformation.OrderRows[1].ArticleNumber);
            Assert.AreEqual("Tess: Tester", request.CreateOrderInformation.OrderRows[1].Description);
            Assert.AreEqual(4.0, request.CreateOrderInformation.OrderRows[1].PricePerUnit);
            Assert.AreEqual(25.0, request.CreateOrderInformation.OrderRows[1].VatPercent);
            Assert.AreEqual(0, request.CreateOrderInformation.OrderRows[1].DiscountPercent);
            Assert.AreEqual(1, request.CreateOrderInformation.OrderRows[1].NumberOfUnits);
            Assert.AreEqual("st", request.CreateOrderInformation.OrderRows[1].Unit);
        }

        [Test]
        public void TestFormatShippingFeeRowsZero()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(Item.OrderRow()
                                                                              .SetArticleNumber("1")
                                                                              .SetQuantity(2)
                                                                              .SetAmountExVat(10)
                                                                              .SetDescription("Specification")
                                                                              .SetName("Prod")
                                                                              .SetVatPercent(0)
                                                                              .SetDiscountPercent(0))
                                                             .AddFee(Item.ShippingFee()
                                                                         .SetShippingId("0")
                                                                         .SetName("Tess")
                                                                         .SetDescription("Tester")
                                                                         .SetAmountExVat(0)
                                                                         .SetVatPercent(0)
                                                                         .SetUnit("st"))
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestFormatInvoiceFeeRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddFee(Item.InvoiceFee()
                                                                   .SetDescription("Tester")
                                                                   .SetAmountExVat(4)
                                                                   .SetVatPercent(25)
                                                                   .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            Assert.AreEqual("", newRows[0].ArticleNumber);
            Assert.AreEqual("Tester", newRows[0].Description);
            Assert.AreEqual(4.0, newRows[0].PricePerUnit);
            Assert.AreEqual(25.0, newRows[0].VatPercent);
            Assert.AreEqual(0, newRows[0].DiscountPercent);
            Assert.AreEqual(1, newRows[0].NumberOfUnits);
            Assert.AreEqual("st", newRows[0].Unit);
        }

        [Test]
        public void TestFormatFixedDiscountRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetAmountIncVat(1)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            Assert.AreEqual("0", newRows[1].ArticleNumber);
            Assert.AreEqual("Tess: Tester", newRows[1].Description);
            Assert.AreEqual(-0.8M, newRows[1].PricePerUnit);
            Assert.AreEqual(25.0M, newRows[1].VatPercent);
            Assert.AreEqual(0, newRows[1].DiscountPercent);
            Assert.AreEqual(1, newRows[1].NumberOfUnits);
            Assert.AreEqual("st", newRows[1].Unit);
        }

        [Test]
        public void TestFormatRelativeDiscountRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetDiscountPercent(10)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            Assert.AreEqual("0", newRows[1].ArticleNumber);
            Assert.AreEqual("Tess: Tester", newRows[1].Description);
            Assert.AreEqual(-0.4, newRows[1].PricePerUnit);
            Assert.AreEqual(25, newRows[1].VatPercent);
            Assert.AreEqual(0, newRows[1].DiscountPercent);
            Assert.AreEqual(1, newRows[1].NumberOfUnits);
            Assert.AreEqual("st", newRows[1].Unit);
        }

        // only amountIncVat => calculate mean vat split into diffrent tax rates present
        // if we have two orders items with different vat rate, we need to create
        // two discount order rows, one for each vat rate
        [Test]
        public void TestFormatFixedDiscountRowsAmountIncVatWithDifferentVatRatesPresent()
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
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetAmountIncVat(100)")
                                                                        .SetDescription("testFormatFixedDiscountRowsWithDifferentVatRatesPresent")
                                                                        .SetAmountIncVat(100)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            // 100*250/356 = 70.22 incl. 25% vat => 14.04 vat as amount 
            OrderRow newRow = newRows[2];
            Assert.AreEqual("42", newRow.ArticleNumber);
            Assert.AreEqual(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (25%)", newRow.Description);
            Assert.AreEqual(-56.18M, newRow.PricePerUnit);
            Assert.AreEqual(25, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent);
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);

            // 100*106/356 = 29.78 incl. 6% vat => 1.69 vat as amount 
            newRow = newRows[3];
            Assert.AreEqual("42", newRow.ArticleNumber);
            Assert.AreEqual(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (6%)", newRow.Description);
            Assert.AreEqual(-28.09M, newRow.PricePerUnit);
            Assert.AreEqual(6, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent);
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);
        }

        // only amountIncVat => calculate mean vat split into diffrent tax rates present
        // if we have two orders items with different vat rate, we need to create
        // two discount order rows, one for each vat rate
        [Test]
        public void TestFormatFixedDiscountRowsMixedItemVatSpecAmountIncVatWithDifferentVatRatesPresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(100.00M)
                                                                        .SetAmountIncVat(125.00M)
                                                                        .SetQuantity(2))
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountIncVat(106.00M)
                                                                        .SetVatPercent(6)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetAmountIncVat(100)")
                                                                        .SetDescription("testFormatFixedDiscountRowsWithDifferentVatRatesPresent")
                                                                        .SetAmountIncVat(100)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            // 100*250/356 = 70.22 incl. 25% vat => 14.04 vat as amount 
            OrderRow newRow = newRows[2];
            Assert.AreEqual("42", newRow.ArticleNumber);
            Assert.AreEqual(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (25%)", newRow.Description);
            Assert.AreEqual(-56.18M, newRow.PricePerUnit);
            Assert.AreEqual(25, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent);
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);

            // 100*106/356 = 29.78 incl. 6% vat => 1.69 vat as amount 
            newRow = newRows[3];
            Assert.AreEqual("42", newRow.ArticleNumber);
            Assert.AreEqual(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (6%)", newRow.Description);
            Assert.AreEqual(-28.09M, newRow.PricePerUnit);
            Assert.AreEqual(6, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent);
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);
        }


        // amountIncVat and vatPercent => add as one row with specified vat rate only
        [Test]
        public void TestFormatFixedDiscountRowsAmountIncVatAndVatPercentWithSingleVatRatePresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4.0M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName(".SetAmountExVat(4.0), .SetVatPercent(25)")
                                                                        .SetDescription("TestFormatFixedDiscountRowsamountIncVatAndVatPercentWithSingleVatRatePresent")
                                                                        .SetAmountIncVat(1.0M)
                                                                        .SetVatPercent(25)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            OrderRow newRow = newRows[1];
            Assert.AreEqual("0", newRow.ArticleNumber);
            Assert.AreEqual(".SetAmountExVat(4.0), .SetVatPercent(25): TestFormatFixedDiscountRowsamountIncVatAndVatPercentWithSingleVatRatePresent", newRow.Description);
            Assert.AreEqual(-0.8M, newRow.PricePerUnit);
            Assert.AreEqual(25, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent);
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);
        }

        // amountIncVat and vatPercent => add as one row with specified vat rate only
        [Test]
        public void TestFormatFixedDiscountRowsAmountIncVatAndVatPercentWithDifferentVatRatesPresent()
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
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetAmountIncVat(111), .vatPercent(25)")
                                                                        .SetDescription("TestFormatFixedDiscountRowsamountIncVatAndVatPercentWithDifferentVatRatesPresent")
                                                                        .SetAmountIncVat(111)
                                                                        .SetVatPercent(25)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            // 100 @25% vat = -80 excl. vat
            OrderRow newRow = newRows[2];
            Assert.AreEqual("42", newRow.ArticleNumber);
            Assert.AreEqual(".SetAmountIncVat(111), .vatPercent(25): TestFormatFixedDiscountRowsamountIncVatAndVatPercentWithDifferentVatRatesPresent", newRow.Description);
            Assert.AreEqual(-88.80M, newRow.PricePerUnit);
            Assert.AreEqual(25, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent);
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);
        }

        // amountExVat and vatPercent => add as one row with specified vat rate only
        [Test]
        public void TestFormatFixedDiscountRowsAmountExVatAndVatPercentWithSingleVatRatePresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(4.0M)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.FixedDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetAmountExVat(1.0M)
                                                                        .SetVatPercent(25)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            OrderRow newRow = newRows[1];
            Assert.AreEqual("0", newRow.ArticleNumber);
            Assert.AreEqual("Tess: Tester", newRow.Description);
            Assert.AreEqual(-1.0M, newRow.PricePerUnit);
            Assert.AreEqual(25, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent);
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);
        }

        // amountExVat and vatPercent => add as one row with specified vat rate only
        [Test]
        public void TestFormatFixedDiscountRowsAmountExVatAndVatPercentWithDifferentVatRatesPresent()
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
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetAmountIncVat(100)")
                                                                        .SetDescription("testFormatFixedDiscountRowsWithDifferentVatRatesPresent")
                                                                        .SetAmountExVat(111)
                                                                        .SetVatPercent(25)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            // 100 @25% vat = -80 excl. vat
            OrderRow newRow = newRows[2];
            Assert.AreEqual("42", newRow.ArticleNumber);
            Assert.AreEqual(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent", newRow.Description);
            Assert.AreEqual(-111.00M, newRow.PricePerUnit);
            Assert.AreEqual(25, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent);
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);
        }

        [Test]
        public void TestFormatRelativeDiscountRowsWithSingleVatRatePresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder()
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetAmountExVat(100.00M)
                                                                        .SetVatPercent(12)
                                                                        .SetQuantity(1))
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountId("0")
                                                                        .SetName(".SetDiscountPercent(20)")
                                                                        .SetDescription("TestFormatRelativeDiscountRowsWithSingleVatRatePresent")
                                                                        .SetDiscountPercent(20)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            OrderRow newRow = newRows[1];
            Assert.AreEqual("0", newRow.ArticleNumber);
            Assert.AreEqual(".SetDiscountPercent(20): TestFormatRelativeDiscountRowsWithSingleVatRatePresent", newRow.Description);
            Assert.AreEqual(-20.00M, newRow.PricePerUnit);
            Assert.AreEqual(12, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent); // not the same thing as in our WebPayItem...
            Assert.AreEqual(1, newRow.NumberOfUnits);
            Assert.AreEqual("st", newRow.Unit);
        }

        // if we have two orders items with different vat rate, we need to create
        // two discount order rows, one for each vat rate
        [Test]
        public void TestFormatRelativeDiscountRowsWithDifferentVatRatesPresent()
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
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetDiscountPercent(10)")
                                                                        .SetDescription("TestFormatRelativeDiscountRowsWithDifferentVatRatesPresent")
                                                                        .SetDiscountPercent(10)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            OrderRow newRow = newRows[2];
            Assert.AreEqual("42", newRow.ArticleNumber);
            Assert.AreEqual(".SetDiscountPercent(10): TestFormatRelativeDiscountRowsWithDifferentVatRatesPresent (25%)", newRow.Description);
            Assert.AreEqual(-20.00M, newRow.PricePerUnit);
            Assert.AreEqual(25, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent); // not the same thing as in our WebPayItem...
            Assert.AreEqual(1, newRow.NumberOfUnits); // 1 "discount unit"
            Assert.AreEqual("st", newRow.Unit);

            newRow = newRows[3];
            Assert.AreEqual("42", newRow.ArticleNumber);
            Assert.AreEqual(".SetDiscountPercent(10): TestFormatRelativeDiscountRowsWithDifferentVatRatesPresent (6%)", newRow.Description);
            Assert.AreEqual(-10.00, newRow.PricePerUnit);
            Assert.AreEqual(6, newRow.VatPercent);
            Assert.AreEqual(0, newRow.DiscountPercent); // not the same thing as in our WebPayItem...
            Assert.AreEqual(1, newRow.NumberOfUnits); // 1 "discount unit"
            Assert.AreEqual("st", newRow.Unit);
        }
    }
}