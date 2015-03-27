using System;
using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order;
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
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddOrderRow(Item.OrderRow()
                                                                        .SetArticleNumber("0")
                                                                        .SetName("Tess")
                                                                        .SetDescription("Tester")
                                                                        .SetAmountExVat(4)
                                                                        .SetVatPercent(25)
                                                                        .SetQuantity(1)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            Assert.That(newRows[0].ArticleNumber, Is.EqualTo("0"));
            Assert.That(newRows[0].Description, Is.EqualTo("Tess: Tester"));
            Assert.That(newRows[0].PricePerUnit, Is.EqualTo(4.0));
            Assert.That(newRows[0].VatPercent, Is.EqualTo(25.0));
            Assert.That(newRows[0].DiscountPercent, Is.EqualTo(0));
            Assert.That(newRows[0].NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRows[0].Unit, Is.EqualTo("st"));
        }

        [Test]
        public void TestFormatShippingFeeRows()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

            Assert.That(request.CreateOrderInformation.OrderRows[1].ArticleNumber, Is.EqualTo("0"));
            Assert.That(request.CreateOrderInformation.OrderRows[1].Description, Is.EqualTo("Tess: Tester"));
            Assert.That(request.CreateOrderInformation.OrderRows[1].PricePerUnit, Is.EqualTo(4.0));
            Assert.That(request.CreateOrderInformation.OrderRows[1].VatPercent, Is.EqualTo(25.0));
            Assert.That(request.CreateOrderInformation.OrderRows[1].DiscountPercent, Is.EqualTo(0));
            Assert.That(request.CreateOrderInformation.OrderRows[1].NumberOfUnits, Is.EqualTo(1));
            Assert.That(request.CreateOrderInformation.OrderRows[1].Unit, Is.EqualTo("st"));
        }

        [Test]
        public void TestFormatInvoiceFeeRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                       .AddFee(Item.InvoiceFee()
                                                                   .SetDescription("Tester")
                                                                   .SetAmountExVat(4)
                                                                   .SetVatPercent(25)
                                                                   .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            Assert.That(newRows[0].ArticleNumber, Is.Empty);
            Assert.That(newRows[0].Description, Is.EqualTo("Tester"));
            Assert.That(newRows[0].PricePerUnit, Is.EqualTo(4.0));
            Assert.That(newRows[0].VatPercent, Is.EqualTo(25.0));
            Assert.That(newRows[0].DiscountPercent, Is.EqualTo(0));
            Assert.That(newRows[0].NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRows[0].Unit, Is.EqualTo("st"));
        }

        [Test]
        public void TestFormatFixedDiscountRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

            Assert.That(newRows[1].ArticleNumber, Is.EqualTo("0"));
            Assert.That(newRows[1].Description, Is.EqualTo("Tess: Tester (25%)"));
            Assert.That(newRows[1].PricePerUnit, Is.EqualTo(-0.8M));
            Assert.That(newRows[1].VatPercent, Is.EqualTo(25.0M));
            Assert.That(newRows[1].DiscountPercent, Is.EqualTo(0));
            Assert.That(newRows[1].NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRows[1].Unit, Is.EqualTo("st"));
        }

        [Test]
        public void TestFormatRelativeDiscountRows()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

            Assert.That(newRows[1].ArticleNumber, Is.EqualTo("0"));
            Assert.That(newRows[1].Description, Is.EqualTo("Tess: Tester (25%)"));
            Assert.That(newRows[1].PricePerUnit, Is.EqualTo(-0.4));
            Assert.That(newRows[1].VatPercent, Is.EqualTo(25));
            Assert.That(newRows[1].DiscountPercent, Is.EqualTo(0));
            Assert.That(newRows[1].NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRows[1].Unit, Is.EqualTo("st"));
        }

        // only amountIncVat => calculate mean vat split into diffrent tax rates present
        // if we have two orders items with different vat rate, we need to create
        // two discount order rows, one for each vat rate
        [Test]
        public void TestFormatFixedDiscountRowsAmountIncVatWithDifferentVatRatesPresent()
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
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetAmountIncVat(100)")
                                                                        .SetDescription("testFormatFixedDiscountRowsWithDifferentVatRatesPresent")
                                                                        .SetAmountIncVat(100)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            // 100*250/356 = 70.22 incl. 25% vat => 14.04 vat as amount 
            OrderRow newRow = newRows[2];
            Assert.That(newRow.ArticleNumber, Is.EqualTo("42"));
            Assert.That(newRow.Description, Is.EqualTo(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (25%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-56.18M));
            Assert.That(newRow.VatPercent, Is.EqualTo(25));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0));
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));

            // 100*106/356 = 29.78 incl. 6% vat => 1.69 vat as amount 
            newRow = newRows[3];
            Assert.That(newRow.ArticleNumber, Is.EqualTo("42"));
            Assert.That(newRow.Description, Is.EqualTo(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (6%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-28.09M));
            Assert.That(newRow.VatPercent, Is.EqualTo(6));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0));
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));
        }

        // only amountIncVat => calculate mean vat split into diffrent tax rates present
        // if we have two orders items with different vat rate, we need to create
        // two discount order rows, one for each vat rate
        [Test]
        public void TestFormatFixedDiscountRowsMixedItemVatSpecAmountIncVatWithDifferentVatRatesPresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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

            Assert.That(newRows[2].ArticleNumber, Is.EqualTo("42"));

            // 100*250/356 = 70.22 incl. 25% vat => 14.04 vat as amount 
            OrderRow newRow = newRows[2];
            Assert.That(newRow.ArticleNumber, Is.EqualTo("42"));
            Assert.That(newRow.Description, Is.EqualTo(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (25%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-70.22M));
            Assert.That(newRow.VatPercent, Is.EqualTo(25));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0));
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));

            // 100*106/356 = 29.78 incl. 6% vat => 1.69 vat as amount 
            newRow = newRows[3];
            Assert.That(newRow.ArticleNumber, Is.EqualTo("42"));
            Assert.That(newRow.Description, Is.EqualTo(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (6%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-29.78M));
            Assert.That(newRow.VatPercent, Is.EqualTo(6));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0));
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));
        }


        // amountIncVat and vatPercent => add as one row with specified vat rate only
        [Test]
        public void TestFormatFixedDiscountRowsAmountIncVatAndVatPercentWithSingleVatRatePresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            Assert.That(newRow.ArticleNumber, Is.EqualTo("0"));
            Assert.That(newRow.Description, Is.EqualTo(".SetAmountExVat(4.0), .SetVatPercent(25): TestFormatFixedDiscountRowsamountIncVatAndVatPercentWithSingleVatRatePresent (25%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-0.8M));
            Assert.That(newRow.VatPercent, Is.EqualTo(25));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0));
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));
        }

        // amountIncVat and vatPercent => add as one row with specified vat rate only
        [Test]
        public void TestFormatFixedDiscountRowsAmountIncVatAndVatPercentWithDifferentVatRatesPresent()
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
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetAmountIncVat(111), .vatPercent(25)")
                                                                        .SetDescription("TestFormatFixedDiscountRowsamountIncVatAndVatPercentWithDifferentVatRatesPresent")
                                                                        .SetAmountIncVat(111)
                                                                        .SetVatPercent(25)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            // 100 @25% vat = -80 excl. vat
            OrderRow newRow = newRows[2];
            Assert.That(newRow.ArticleNumber, Is.EqualTo("42"));
            Assert.That(newRow.Description, Is.EqualTo(".SetAmountIncVat(111), .vatPercent(25): TestFormatFixedDiscountRowsamountIncVatAndVatPercentWithDifferentVatRatesPresent (25%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-88.80M));
            Assert.That(newRow.VatPercent, Is.EqualTo(25));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0));
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));
        }

        // amountExVat and vatPercent => add as one row with specified vat rate only
        [Test]
        public void TestFormatFixedDiscountRowsAmountExVatAndVatPercentWithSingleVatRatePresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            Assert.That(newRow.ArticleNumber, Is.EqualTo("0"));
            Assert.That(newRow.Description, Is.EqualTo("Tess: Tester (25%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-1.0M));
            Assert.That(newRow.VatPercent, Is.EqualTo(25));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0));
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));
        }

        // amountExVat and vatPercent => add as one row with specified vat rate only
        [Test]
        public void TestFormatFixedDiscountRowsAmountExVatAndVatPercentWithDifferentVatRatesPresent()
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
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetAmountIncVat(100)")
                                                                        .SetDescription("testFormatFixedDiscountRowsWithDifferentVatRatesPresent")
                                                                        .SetAmountExVat(111)
                                                                        .SetVatPercent(25)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            // 100 @25% vat = -80 excl. vat
            OrderRow newRow = newRows[2];
            Assert.That(newRow.ArticleNumber, Is.EqualTo("42"));
            Assert.That(newRow.Description, Is.EqualTo(".SetAmountIncVat(100): testFormatFixedDiscountRowsWithDifferentVatRatesPresent (25%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-111.00M));
            Assert.That(newRow.VatPercent, Is.EqualTo(25));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0));
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));
        }

        [Test]
        public void TestFormatRelativeDiscountRowsWithSingleVatRatePresent()
        {
            CreateOrderBuilder order = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
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
            Assert.That(newRow.ArticleNumber, Is.EqualTo("0"));
            Assert.That(newRow.Description, Is.EqualTo(".SetDiscountPercent(20): TestFormatRelativeDiscountRowsWithSingleVatRatePresent (12%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-20.00M));
            Assert.That(newRow.VatPercent, Is.EqualTo(12));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0)); // not the same thing as in our WebPayItem...
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1));
            Assert.That(newRow.Unit, Is.EqualTo("st"));
        }

        // if we have two orders items with different vat rate, we need to create
        // two discount order rows, one for each vat rate
        [Test]
        public void TestFormatRelativeDiscountRowsWithDifferentVatRatesPresent()
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
                                                       .AddDiscount(Item.RelativeDiscount()
                                                                        .SetDiscountId("42")
                                                                        .SetName(".SetDiscountPercent(10)")
                                                                        .SetDescription("TestFormatRelativeDiscountRowsWithDifferentVatRatesPresent")
                                                                        .SetDiscountPercent(10)
                                                                        .SetUnit("st"));

            List<OrderRow> newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();


            OrderRow newRow = newRows[2];
            Assert.That(newRow.ArticleNumber, Is.EqualTo("42"));
            Assert.That(newRow.Description, Is.EqualTo(".SetDiscountPercent(10): TestFormatRelativeDiscountRowsWithDifferentVatRatesPresent (25%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-20.00M));
            Assert.That(newRow.VatPercent, Is.EqualTo(25));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0)); // not the same thing as in our WebPayItem...
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1)); // 1 "discount unit"
            Assert.That(newRow.Unit, Is.EqualTo("st"));

            newRow = newRows[3];
            Assert.That(newRow.ArticleNumber, Is.EqualTo("42"));
            Assert.That(newRow.Description, Is.EqualTo(".SetDiscountPercent(10): TestFormatRelativeDiscountRowsWithDifferentVatRatesPresent (6%)"));
            Assert.That(newRow.PricePerUnit, Is.EqualTo(-10.00));
            Assert.That(newRow.VatPercent, Is.EqualTo(6));
            Assert.That(newRow.DiscountPercent, Is.EqualTo(0)); // not the same thing as in our WebPayItem...
            Assert.That(newRow.NumberOfUnits, Is.EqualTo(1)); // 1 "discount unit"
            Assert.That(newRow.Unit, Is.EqualTo("st"));
        }


    }
}