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

            Assert.That(response.ErrorMessage, Is.EqualTo("An order with the provided id does not exist."));
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

            Assert.That(response.Accepted, Is.True);
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.Post));
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr, Is.Not.Null);
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr.Length, Is.GreaterThan(0));
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.LowestAmountToPay, Is.EqualTo(0.0));
        }

        [Test]
        public void TestDeliverInvoiceOrderCreatedInclVatDeliveredInclVat()
        {
            CreateOrderEuResponse response1 = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
                                                              .AddCustomerDetails(Item.IndividualCustomer()
                                                                                      .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                              .SetOrderDate(TestingTool.DefaultTestDate)
                                                              .SetCurrency(TestingTool.DefaultTestCurrency)
                                                              .UseInvoicePayment()
                                                              .DoRequest();
            long orderId = response1.CreateOrderResult.SveaOrderId;

            DeliverOrderEuResponse response = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
                                                              .SetOrderId(orderId)
                                                              .SetNumberOfCreditDays(1)
                                                              .SetInvoiceDistributionType(InvoiceDistributionType.POST)
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .DeliverInvoiceOrder()
                                                              .DoRequest();

            Assert.That(response.Accepted, Is.True);
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.Post));
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr, Is.Not.Null);
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr.Length, Is.GreaterThan(0));
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.LowestAmountToPay, Is.EqualTo(0.0));
        }


        [Test]
        public void TestDeliverInvoiceOrderCreatedExclVatDeliveredInclVatRetries()
        {
            CreateOrderEuResponse response1 = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                              .AddCustomerDetails(Item.IndividualCustomer()
                                                                                      .SetNationalIdNumber(TestingTool.DefaultTestIndividualNationalIdNumber))
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                              .SetOrderDate(TestingTool.DefaultTestDate)
                                                              .SetCurrency(TestingTool.DefaultTestCurrency)
                                                              .UseInvoicePayment()
                                                              .DoRequest();
            long orderId = response1.CreateOrderResult.SveaOrderId;

            DeliverOrderEuResponse response = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
                                                              .SetOrderId(orderId)
                                                              .SetNumberOfCreditDays(1)
                                                              .SetInvoiceDistributionType(InvoiceDistributionType.POST)
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .DeliverInvoiceOrder()
                                                              .DoRequest();

            Assert.That(response.Accepted, Is.True);
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.Post));
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr, Is.Not.Null);
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.Ocr.Length, Is.GreaterThan(0));
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.LowestAmountToPay, Is.EqualTo(0.0));
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

            Assert.That(deliverOrderResponse.Accepted, Is.True);
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