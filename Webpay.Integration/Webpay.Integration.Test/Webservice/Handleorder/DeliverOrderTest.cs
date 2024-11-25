using Webpay.Integration.Config;
using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Order.Handle;
using Webpay.Integration.Order.Row;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using WebpayWS;
using OrderType = WebpayWS.OrderType;

namespace Webpay.Integration.Test.Webservice.Handleorder;

[TestFixture]
public class DeliverOrderTest
{
    private DeliverOrderBuilder _order;

    [SetUp]
    public void SetUp()
    {
        _order = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig());
    }

    [Test]
    public void TestBuildRequest()
    {
        DeliverOrderBuilder request = _order.SetOrderId(54086L);

        Assert.That(request.GetOrderId(), Is.EqualTo(54086L));
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
                .SetInvoiceDistributionType(DistributionType.POST)
                .SetOrderId(54086L)
                .SetNumberOfCreditDays(1)
                .SetCreditInvoice(117L)
                .SetCountryCode(TestingTool.DefaultTestCountryCode)
                .DeliverInvoiceOrder()
                .PrepareRequest();
        }
            );

        var invoiceDetails = request.DeliverOrderInformation.DeliverInvoiceDetails;

        var firstOrderRow = invoiceDetails.OrderRows[0];

        // First row
        Assert.That(firstOrderRow.ArticleNumber, Is.EqualTo("1"));
        Assert.That(firstOrderRow.Description, Is.EqualTo("Prod: Specification"));
        Assert.That(firstOrderRow.PricePerUnit, Is.EqualTo(100.00M));

        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[0].NumberOfUnits, Is.EqualTo(2));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[0].Unit, Is.EqualTo("st"));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[0].VatPercent, Is.EqualTo(25));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[0].DiscountPercent,
            Is.EqualTo(0));

        //Second order row is shipment
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].ArticleNumber,
            Is.EqualTo("33"));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].Description,
            Is.EqualTo("shipping: Specification"));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].PricePerUnit, Is.EqualTo(50));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].NumberOfUnits, Is.EqualTo(1));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].Unit, Is.EqualTo("st"));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].VatPercent, Is.EqualTo(25));
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[1].DiscountPercent,
            Is.EqualTo(0));
        //discount
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.OrderRows[2].PricePerUnit,
            Is.EqualTo(-8.0));

        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.NumberOfCreditDays, Is.EqualTo(1));

        Assert.That(invoiceDetails.InvoiceDistributionType, Is.EqualTo(WebpayWS.InvoiceDistributionType.Post));

        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.IsCreditInvoice, Is.True);
        Assert.That(request.DeliverOrderInformation.DeliverInvoiceDetails.InvoiceIdToCredit, Is.EqualTo(117L));
        Assert.That(request.DeliverOrderInformation.SveaOrderId, Is.EqualTo(54086L));
        Assert.That(request.DeliverOrderInformation.OrderType, Is.EqualTo(OrderType.Invoice));
    }

    [Test]
    public void TestDeliverPaymentPlanOrder()
    {
        DeliverOrderEuRequest request = _order
            .SetOrderId(54086L)
            .SetCountryCode(TestingTool.DefaultTestCountryCode)
            .DeliverPaymentPlanOrder()
            .PrepareRequest();

        Assert.That(request.DeliverOrderInformation.SveaOrderId, Is.EqualTo(54086L));
        Assert.That(request.DeliverOrderInformation.OrderType, Is.EqualTo(OrderType.PaymentPlan));
    }

    [Test]
    public void TestDeliverCardOrder()
    {
        var fakeSveaOrderId = 987654;
        HostedActionRequest confirmActionRequest = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .SetOrderId(fakeSveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .DeliverCardOrder()
            ;

        Assert.That(confirmActionRequest.ConfigurationProvider, Is.TypeOf<SveaTestConfigurationProvider>());
        Assert.That(confirmActionRequest.ServicePath, Contains.Substring("/confirm"));

        // Set after prepareRequest() has been called on confirmActionRequest
        Assert.That(confirmActionRequest.Xml, Contains.Substring(fakeSveaOrderId.ToString()));
    }
}