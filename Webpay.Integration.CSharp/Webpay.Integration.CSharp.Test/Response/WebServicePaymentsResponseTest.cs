using NUnit.Framework;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Util.Constant;
using Webpay.Integration.CSharp.Util.Testing;
using Webpay.Integration.CSharp.WebpayWS;
using InvoiceDistributionType = Webpay.Integration.CSharp.Util.Constant.InvoiceDistributionType;

namespace Webpay.Integration.CSharp.Test.Response
{
    [TestFixture]
    public class WebServicePaymentsResponseTest
    {
        [Test]
        public void TestDeliverInvoiceOrderResult()
        {
            CreateOrderEuResponse createOrderResponse = WebpayConnection.CreateOrder()
                                                                        .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                                        .AddCustomerDetails(
                                                                            TestingTool.CreateIndividualCustomer())
                                                                        .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                        .SetClientOrderNumber(
                                                                            TestingTool.DefaultTestClientOrderNumber)
                                                                        .SetOrderDate(TestingTool.DefaultTestDate)
                                                                        .SetCurrency(TestingTool.DefaultTestCurrency)
                                                                        .UseInvoicePayment()
                                                                        .DoRequest();

            Assert.AreEqual(0, createOrderResponse.ResultCode);
            Assert.IsTrue(createOrderResponse.Accepted);

            DeliverOrderEuResponse deliverOrderResponse = WebpayConnection.DeliverOrder()
                                                                          .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                                          .SetOrderId(
                                                                              createOrderResponse.CreateOrderResult
                                                                                                 .SveaOrderId)
                                                                          .SetNumberOfCreditDays(1)
                                                                          .SetInvoiceDistributionType(
                                                                              InvoiceDistributionType.POST)
                                                                          .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                          .DeliverInvoiceOrder()
                                                                          .DoRequest();

            Assert.AreEqual(0, deliverOrderResponse.ResultCode);
            Assert.IsTrue(deliverOrderResponse.Accepted);
        }

        [Test]
        public void TestCompanyIdRequest()
        {
            CreateOrderEuRequest request = WebpayConnection.CreateOrder()
                                                           .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                           .AddCustomerDetails(Item.CompanyCustomer()
                                                                                   .SetNationalIdNumber("4354kj"))
                                                           .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                           .SetClientOrderNumber(
                                                               TestingTool.DefaultTestClientOrderNumber)
                                                           .SetOrderDate(TestingTool.DefaultTestDate)
                                                           .SetCurrency(TestingTool.DefaultTestCurrency)
                                                           .UseInvoicePayment()
                                                           .PrepareRequest();

            Assert.AreEqual(79021, request.Auth.ClientNumber);
            Assert.AreEqual("4354kj", request.CreateOrderInformation.CustomerIdentity.NationalIdNumber);
        }

        [Test]
        public void TestCompanyIdResponse()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetNationalIdNumber(TestingTool.DefaultTestCompanyNationalIdNumber))
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(CustomerType.Company, response.CreateOrderResult.CustomerIdentity.CustomerType);
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.IndividualIdentity);
            Assert.IsNull(response.CreateOrderResult.CustomerIdentity.CompanyIdentity);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestDeCompanyIdentity()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(TestingTool.CreateOrderRowDe())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetNationalIdNumber("12345")
                                                                                     .SetVatNumber("DE123456789")
                                                                                     .SetStreetAddress(
                                                                                         "Adalbertsteinweg", "1")
                                                                                     .SetZipCode("52070")
                                                                                     .SetLocality("AACHEN"))
                                                             .SetCountryCode(CountryCode.DE)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(Currency.EUR)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.AreEqual(CustomerType.Company, response.CreateOrderResult.CustomerIdentity.CustomerType);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestNlCompanyIdentity()
        {
            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(TestingTool.CreateOrderRowNl())
                                                             .AddCustomerDetails(Item.CompanyCustomer()
                                                                                     .SetCompanyName("Svea bakkerij 123")
                                                                                     .SetVatNumber("NL123456789A12")
                                                                                     .SetStreetAddress("broodstraat",
                                                                                                       "1")
                                                                                     .SetZipCode("1111 CD")
                                                                                     .SetLocality("BARENDRECHT"))
                                                             .SetCountryCode(CountryCode.NL)
                                                             .SetClientOrderNumber(
                                                                 TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(Currency.EUR)
                                                             .UseInvoicePayment()
                                                             .DoRequest();

            Assert.AreEqual(0, response.ResultCode);
            Assert.AreEqual(CustomerType.Company, response.CreateOrderResult.CustomerIdentity.CustomerType);
            Assert.IsTrue(response.Accepted);
        }

        [Test]
        public void TestDeliverPaymentPlanOrderResult()
        {
            GetPaymentPlanParamsEuResponse paymentPlanParamResponse = WebpayConnection.GetPaymentPlanParams()
                                                                                      .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                                      .DoRequest();
            long code = paymentPlanParamResponse.CampaignCodes[0].CampaignCode;

            CreateOrderEuResponse createOrderResponse = WebpayConnection.CreateOrder()
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

            DeliverOrderEuResponse deliverOrderResponse = WebpayConnection.DeliverOrder()
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

        [Test]
        public void TestResultGetPaymentPlanParams()
        {
            GetPaymentPlanParamsEuResponse response =
                WebpayConnection.GetPaymentPlanParams(SveaConfig.GetDefaultConfig())
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .DoRequest();

            Assert.IsTrue(response.Accepted);
            Assert.AreEqual(213060, response.CampaignCodes[0].CampaignCode);
            Assert.AreEqual("Köp nu betala om 3 månader (räntefritt)", response.CampaignCodes[0].Description);
            Assert.AreEqual(PaymentPlanTypeCode.InterestAndAmortizationFree, response.CampaignCodes[0].PaymentPlanType);
            Assert.AreEqual(3, response.CampaignCodes[0].ContractLengthInMonths);
            Assert.AreEqual(100, response.CampaignCodes[0].InitialFee);
            Assert.AreEqual(29, response.CampaignCodes[0].NotificationFee);
            Assert.AreEqual(0, response.CampaignCodes[0].InterestRatePercent);
            Assert.AreEqual(3, response.CampaignCodes[0].NumberOfInterestFreeMonths);
            Assert.AreEqual(3, response.CampaignCodes[0].NumberOfPaymentFreeMonths);
            Assert.AreEqual(1000, response.CampaignCodes[0].FromAmount);
            Assert.AreEqual(50000, response.CampaignCodes[0].ToAmount);
        }

        [Test]
        public void TestResultGetAddresses()
        {
            GetAddressesEuResponse request = WebpayConnection.GetAddresses()
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetOrderTypeInvoice()
                                                             .SetIndividual(TestingTool.DefaultTestIndividualNationalIdNumber)
                                                             .SetZipCode("99999")
                                                             .DoRequest();

            Assert.AreEqual(0, request.ResultCode);
            Assert.IsTrue(request.Accepted);
            Assert.AreEqual("Tess T", request.GetAddressesResult.Addresses[0].FirstName);
            Assert.AreEqual("Persson", request.GetAddressesResult.Addresses[0].LastName);
            Assert.AreEqual("Testgatan 1", request.GetAddressesResult.Addresses[0].Address.Street);
            Assert.AreEqual("99999", request.GetAddressesResult.Addresses[0].Address.ZipCode);
            Assert.AreEqual("Stan", request.GetAddressesResult.Addresses[0].Address.Locality);
        }

        [Test]
        public void TestPaymentPlanRequestReturnsAcceptedResult()
        {
            GetPaymentPlanParamsEuResponse paymentPlanParam = WebpayConnection.GetPaymentPlanParams()
                                                                              .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                                              .DoRequest();
            long code = paymentPlanParam.CampaignCodes[0].CampaignCode;

            CreateOrderEuResponse response = WebpayConnection.CreateOrder()
                                                             .AddOrderRow(TestingTool.CreatePaymentPlanOrderRow())
                                                             .AddCustomerDetails(TestingTool.CreateIndividualCustomer())
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .SetCustomerReference(TestingTool.DefaultTestCustomerReferenceNumber)
                                                             .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                                             .SetOrderDate(TestingTool.DefaultTestDate)
                                                             .SetCurrency(TestingTool.DefaultTestCurrency)
                                                             .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                                             .UsePaymentPlanPayment(code)
                                                             .DoRequest();

            Assert.IsTrue(response.Accepted);
        }
    }
}