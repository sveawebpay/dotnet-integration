using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;
using InvoiceDistributionType = Webpay.Integration.CSharp.Util.Constant.InvoiceDistributionType;

namespace Webpay.Integration.CSharp.IntegrationTest.Webservice.Payment
{
    [TestFixture]
    public class DeliverOrderTest
    {
        [Test]
        public void TestDeliverPaymentPlanOrderDoRequest()
        {
            DeliverOrderEuResponse response =
                WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                .SetOrderId(54086L)
                                .SetInvoiceDistributionType(InvoiceDistributionType.POST)
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .DeliverInvoiceOrder()
                                .DoRequest();

            Assert.AreEqual("An order with the provided id does not exist.", response.ErrorMessage);
        }

        [Test]
        public void TestDeliverInvoiceOrderResult()
        {
            long orderId = CreateInvoiceAndReturnOrderId();

            DeliverOrderEuResponse response = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                              .SetOrderId(orderId)
                                                              .SetNumberOfCreditDays(1)
                                                              .SetInvoiceDistributionType(InvoiceDistributionType.POST)
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .DeliverInvoiceOrder()
                                                              .DoRequest();

            Assert.IsTrue(response.Accepted);
            Assert.AreEqual(WebpayWS.InvoiceDistributionType.Post, response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType);
            Assert.NotNull(response.DeliverOrderResult.InvoiceResultDetails.Ocr);
            Assert.IsTrue(0 < response.DeliverOrderResult.InvoiceResultDetails.Ocr.Length);
            Assert.AreEqual(0.0, response.DeliverOrderResult.InvoiceResultDetails.LowestAmountToPay);
        }

        [Test]
        public void TestDeliverPaymentPlanOrderResult()
        {
            GetPaymentPlanParamsEuResponse paymentPlanParamResponse = WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                                                                                      .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                                      .DoRequest();
            long code = paymentPlanParamResponse.CampaignCodes[0].CampaignCode;

            CreateOrderEuResponse createOrderResponse = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                                        .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
                                                                        .AddCustomerDetails(
                                                                            TestingTool.CreateIndividualCustomer())
                                                                        .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                        .SetCustomerReference(
                                                                            TestingTool.DefaultTestClientOrderNumber)
                                                                        .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                                        .SetOrderDate(TestingTool.DefaultTestDate)
                                                                        .SetCurrency(TestingTool.DefaultTestCurrency)
                                                                        .UsePaymentPlanPayment(code)
                                                                        .DoRequest();

            DeliverOrderEuResponse deliverOrderResponse = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                                          .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                                          .SetOrderId(
                                                                              createOrderResponse.CreateOrderResult
                                                                                                 .SveaOrderId)
                                                                          .SetNumberOfCreditDays(1)
                                                                          .SetInvoiceDistributionType(
                                                                              InvoiceDistributionType.POST)
                                                                          .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                          .DeliverPaymentPlanOrder()
                                                                          .DoRequest();

            Assert.IsTrue(deliverOrderResponse.Accepted);
        }

        private long CreateInvoiceAndReturnOrderId()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                             .AddCustomerDetails(Item.IndividualCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            return response.CreateOrderResult.SveaOrderId;
        }
    }
}