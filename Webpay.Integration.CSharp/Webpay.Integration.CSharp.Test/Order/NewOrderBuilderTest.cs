using System.Collections.Generic;
using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Testing;
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
                    TestingTool.CreateExVatBasedOrderRow("1"),
                    TestingTool.CreateExVatBasedOrderRow("2")
                };

            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRows(orderRows)
                                                           .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual(request.CreateOrderInformation.CustomerIdentity.NationalIdNumber, "164608142222");
            Assert.AreEqual(request.CreateOrderInformation.OrderRows[0].ArticleNumber, "1");
            Assert.AreEqual(request.CreateOrderInformation.OrderRows[1].ArticleNumber, "2");
        }

        [Test]
        public void TestBuildOrderWithCompanyCustomer()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                           .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual("164608142222", request.CreateOrderInformation.CustomerIdentity.NationalIdNumber);
        }
    }
}