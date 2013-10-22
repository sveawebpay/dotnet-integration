﻿using NUnit.Framework;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Test.Util;
using Webpay.Integration.CSharp.WebpayWS;
using InvoiceDistributionType = Webpay.Integration.CSharp.Util.Constant.InvoiceDistributionType;

namespace Webpay.Integration.CSharp.Test.Webservice.Handleorder
{
    [TestFixture]
    public class DeliverOrderTest
    {
        private DeliverOrderBuilder _order;

        [SetUp]
        public void SetUp()
        {
            _order = WebpayConnection.DeliverOrder();
        }

        [Test]
        public void TestBuildRequest()
        {
            DeliverOrderBuilder request = _order.SetOrderId(54086L);
            Assert.AreEqual(54086L, request.GetOrderId());
        }

        [Test]
        public void TestDeliverInvoice()
        {
            DeliverOrderEuRequest request = null;
            Assert.DoesNotThrow(() =>
                {
                    request = _order.AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                    .AddFee(TestingTool.CreateExVatBasedShippingFee())
                                    .AddDiscount(Item.FixedDiscount()
                                                     .SetAmountIncVat(10))
                                    .SetInvoiceDistributionType(InvoiceDistributionType.POST)
                                    .SetOrderId(54086L)
                                    .SetNumberOfCreditDays(1)
                                    .SetCreditInvoice(117L)
                                    .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                    .DeliverInvoiceOrder()
                                    .PrepareRequest();
                }
                );
            //First order row is a product

            var invoiceDetails = request.DeliverOrderInformation.DeliverInvoiceDetails;

            var firstOrderRow = invoiceDetails.OrderRows[0];

            // First row
            Assert.That(firstOrderRow.ArticleNumber, Is.EqualTo("1"));
            Assert.That(firstOrderRow.Description, Is.EqualTo("Prod: Specification"));
            Assert.That(firstOrderRow.PricePerUnit, Is.EqualTo(100.00M));

            Assert.AreEqual(2, request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[0].NumberOfUnits);
            Assert.AreEqual("st", request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[0].Unit);
            Assert.AreEqual(25, request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[0].VatPercent);
            Assert.AreEqual(0, request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[0].DiscountPercent);

            //Second order row is shipment
            Assert.AreEqual("33", request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].ArticleNumber);
            Assert.AreEqual("shipping: Specification",
                            request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].Description);
            Assert.AreEqual(50, request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].PricePerUnit);
            Assert.AreEqual(1, request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].NumberOfUnits);
            Assert.AreEqual("st", request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].Unit);
            Assert.AreEqual(25, request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].VatPercent);
            Assert.AreEqual(0, request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].DiscountPercent);
            //discount
            Assert.AreEqual(-8.0, request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[2].PricePerUnit);

            Assert.AreEqual(1, request.DeliverOrderInformation.DeliverInvoiceDetails.NumberOfCreditDays);

            Assert.That(invoiceDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.Post));


            Assert.AreEqual(true, request.DeliverOrderInformation.DeliverInvoiceDetails.IsCreditInvoice);
            Assert.AreEqual(117L, request.DeliverOrderInformation.DeliverInvoiceDetails.InvoiceIdToCredit);
            Assert.AreEqual(54086L, request.DeliverOrderInformation.SveaOrderId);
            Assert.AreEqual(OrderType.Invoice, request.DeliverOrderInformation.OrderType);
        }

        [Test]
        public void TestDeliverPaymentPlanOrder()
        {
            DeliverOrderEuRequest request = _order
                .SetOrderId(54086L)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DeliverPaymentPlanOrder()
                .PrepareRequest();

            Assert.AreEqual(54086L, request.DeliverOrderInformation.SveaOrderId);
            Assert.AreEqual(OrderType.PaymentPlan, request.DeliverOrderInformation.OrderType);
        }

        [Test]
        public void TestDeliverPaymentPlanOrderDoRequest()
        {
            DeliverOrderEuResponse response =
                WebpayConnection.DeliverOrder()
                                .AddOrderRow(TestingTool.CreateExVatBasedOrderRow())
                                .SetOrderId(54086L)
                                .SetInvoiceDistributionType(InvoiceDistributionType.POST)
                                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                .DeliverInvoiceOrder()
                                .DoRequest();

            Assert.AreEqual("An order with the provided id does not exist.", response.ErrorMessage);
        }
    }
}