using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Test.Order
{
    [TestFixture]
    public class NewOrderBuilderTest
    {
        [Test]
        public void TestBuildOrderRowList()
        {
            var orderRows = new List<OrderRowBuilder>
                {
                    Item.OrderRow()
                        .SetArticleNumber("1")
                        .SetQuantity(2)
                        .SetAmountExVat(100.00M)
                        .SetDescription("Specification")
                        .SetName("Prod")
                        .SetUnit("st")
                        .SetVatPercent(25)
                        .SetDiscountPercent(0),
                    Item.OrderRow()
                        .SetArticleNumber("2")
                        .SetQuantity(2)
                        .SetAmountExVat(100.00M)
                        .SetDescription("Specification")
                        .SetName("Prod")
                        .SetUnit("st")
                        .SetVatPercent(25)
                        .SetDiscountPercent(0)
                };

            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRows(orderRows)
                                                           .AddCustomerDetails(Item.CompanyCustomer()
                                                                                   .SetNationalIdNumber("666666")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("9999")
                                                                                   .SetLocality("Stan"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetCustomerReference("33")
                                                           .SetOrderDate("2012-12-12")
                                                           .SetCurrency(Currency.SEK)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual(request.CreateOrderInformation.CustomerIdentity.NationalIdNumber, "666666");
            Assert.AreEqual(request.CreateOrderInformation.OrderRows[0].ArticleNumber, "1");
            Assert.AreEqual(request.CreateOrderInformation.OrderRows[1].ArticleNumber, "2");
        }

        [Test]
        public void TestBuildOrderWithCompanyCustomer()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(Item.OrderRow()
                                                                            .SetArticleNumber("1")
                                                                            .SetQuantity(2)
                                                                            .SetAmountExVat(100.00M)
                                                                            .SetDescription("Specification")
                                                                            .SetName("Prod")
                                                                            .SetUnit("st")
                                                                            .SetVatPercent(25)
                                                                            .SetDiscountPercent(0))
                                                           .AddCustomerDetails(Item.CompanyCustomer()
                                                                                   .SetNationalIdNumber("666666")
                                                                                   .SetEmail("test@svea.com")
                                                                                   .SetPhoneNumber("999999")
                                                                                   .SetIpAddress("123.123.123.123")
                                                                                   .SetStreetAddress("Gatan", "23")
                                                                                   .SetCoAddress("c/o Eriksson")
                                                                                   .SetZipCode("9999")
                                                                                   .SetLocality("Stan"))
                                                           .SetCountryCode(CountryCode.SE)
                                                           .SetCustomerReference("33")
                                                           .SetOrderDate("2012-12-12")
                                                           .SetCurrency(Currency.SEK)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("666666", request.CreateOrderInformation.CustomerIdentity.NationalIdNumber);
        }
    }
}