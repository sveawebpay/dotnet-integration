using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
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

            OrderRow[] newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

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
                                                                                   .SetNationalIdNumber("194605092222"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetOrderDate("2012-12-12")
                                                           .SetClientOrderNumber("33")
                                                           .SetCurrency(Currency.SEK)
                                                           .SetCustomerReference("33")
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
                                                                                     .SetNationalIdNumber("194605092222"))
                                                             .SetCountryCode(CountryCode.SE)
                                                             .SetOrderDate("2012-12-12")
                                                             .SetClientOrderNumber("33")
                                                             .SetCurrency(Currency.SEK)
                                                             .SetCustomerReference("33")
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

            OrderRow[] newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

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

            OrderRow[] newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            Assert.AreEqual("0", newRows[1].ArticleNumber);
            Assert.AreEqual("Tess: Tester", newRows[1].Description);
            Assert.AreEqual(-0.8, newRows[1].PricePerUnit);
            Assert.AreEqual(25.0, newRows[1].VatPercent);
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

            OrderRow[] newRows = new WebServiceRowFormatter<CreateOrderBuilder>(order).FormatRows();

            Assert.AreEqual("0", newRows[1].ArticleNumber);
            Assert.AreEqual("Tess: Tester", newRows[1].Description);
            Assert.AreEqual(-0.4, newRows[1].PricePerUnit);
            Assert.AreEqual(25, newRows[1].VatPercent);
            Assert.AreEqual(0, newRows[1].DiscountPercent);
            Assert.AreEqual(1, newRows[1].NumberOfUnits);
            Assert.AreEqual("st", newRows[1].Unit);
        }
    }
}