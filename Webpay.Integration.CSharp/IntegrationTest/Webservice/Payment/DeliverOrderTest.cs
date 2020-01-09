using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;

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
                                .SetInvoiceDistributionType(DistributionType.POST)
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .DeliverInvoiceOrder()
                                .DoRequest();

            Assert.That(response.ErrorMessage, Is.EqualTo("Order does not exist."));
        }

        [Test]
        public void TestDeliverInvoiceOrderResult()
        {
            long orderId = CreateExVatInvoiceAndReturnOrderId();

            DeliverOrderEuResponse response = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                              .SetOrderId(orderId)
                                                              .SetNumberOfCreditDays(1)
                                                              .SetInvoiceDistributionType(DistributionType.POST)
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
        public void TestDeliverInvoiceOrderWithEInvoiceB2BResult()
        {
            long orderId = CreateNorwegianExVatInvoiceAndReturnOrderId();

            DeliverOrderEuResponse response = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                              .SetOrderId(orderId)
                                                              .SetNumberOfCreditDays(1)
                                                              .SetInvoiceDistributionType(DistributionType.EINVOICEB2B)
                                                              .SetCountryCode(CountryCode.NO)
                                                              .DeliverInvoiceOrder()
                                                              .DoRequest();

            Assert.That(response.Accepted, Is.True);
            Assert.That(response.DeliverOrderResult.InvoiceResultDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.EInvoiceB2B));
        }

        [Test]
        public void TestDeliverInvoiceOrderCreatedInclVatDeliveredInclVat()
        {
            var orderId = CreateIncVatOrderAndReturnOrderId();

            DeliverOrderEuResponse response = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
                                                              .SetOrderId(orderId)
                                                              .SetNumberOfCreditDays(1)
                                                              .SetInvoiceDistributionType(DistributionType.POST)
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
        public void TestDeliverInvoiceOrderCreatedExclVatDeliveredInclVatRetriesWithExVat()
        {
            long orderId = CreateExVatInvoiceAndReturnOrderId();

            DeliverOrderEuResponse response = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
                                                              .SetOrderId(orderId)
                                                              .SetNumberOfCreditDays(1)
                                                              .SetInvoiceDistributionType(DistributionType.POST)
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
                                                                              DistributionType.POST)
                                                                          .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                          .DeliverPaymentPlanOrder()
                                                                          .DoRequest();

            Assert.That(deliverOrderResponse.Accepted, Is.True);
        }

        private long CreateExVatInvoiceAndReturnOrderId()
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

        private static long CreateIncVatOrderAndReturnOrderId()
        {
            CreateOrderEuResponse response1 = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                              .AddOrderRow(TestingTool.CreateIncVatBasedOrderRow("1"))
                                                              .AddCustomerDetails(Item.IndividualCustomer()
                                                                                      .SetNationalIdNumber(
                                                                                          TestingTool
                                                                                              .DefaultTestIndividualNationalIdNumber))
                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                              .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                              .SetOrderDate(TestingTool.DefaultTestDate)
                                                              .SetCurrency(TestingTool.DefaultTestCurrency)
                                                              .UseInvoicePayment()
                                                              .DoRequest();
            return response1.CreateOrderResult.SveaOrderId;
        }

        private long CreateNorwegianExVatInvoiceAndReturnOrderId()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder(SveaConfig.GetDefaultConfig())
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetNationalIdNumber("923313850"))
                                                             .SetCountryCode(CountryCode.NO)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            return response.CreateOrderResult.SveaOrderId;
        }
    }
}