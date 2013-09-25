using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Test.Webservice.Handleorder
{
    [TestFixture]
    public class CloseOrderTest
    {
        [Test]
        public void TestCloseOrder()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(Item.OrderRow()
                                                                              .SetArticleNumber("1")
                                                                              .SetQuantity(2)
                                                                              .SetAmountExVat(new decimal(100.00))
                                                                              .SetDescription("Specification")
                                                                              .SetName("Prod")
                                                                              .SetUnit("st")
                                                                              .SetVatPercent(25)
                                                                              .SetDiscountPercent(0))
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber("194605092222"))
                                                             .SetCountryCode(CountryCode.SE)
                                                             .SetClientOrderNumber("33")
                                                             .SetOrderDate("2012-12-12")
                                                             .SetCurrency(Currency.SEK)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.IsTrue(response.Accepted);

            CloseOrderEuResponse closeResponse = WebpayConnection.CloseOrder()
                                                                 .SetOrderId(response.CreateOrderResult.SveaOrderId)
                                                                 .SetCountrycode(CountryCode.SE)
                                                                 .CloseInvoiceOrder()
                                                                 .DoRequest();

            Assert.AreEqual(0, closeResponse.ResultCode);
            Assert.IsTrue(closeResponse.Accepted);
        }
    }
}