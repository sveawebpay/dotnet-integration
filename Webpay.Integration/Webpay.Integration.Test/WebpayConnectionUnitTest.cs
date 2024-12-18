using Webpay.Integration.Config;
using Webpay.Integration.Hosted.Admin;
using Webpay.Integration.Util.Constant;
using Webpay.Integration.Util.Testing;
using WebpayWS;

namespace Webpay.Integration.Test;

[TestFixture]
public class WebpayConnectionUnitTest
{
    [Test] static public void createOrder_useInvoicePayment_returns_InvoicePayment_request()
    {
        new Webpay.Integration.Test.Webservice.Payment.InvoicePaymentTest()
            .TestInvoiceRequestObjectForCustomerIdentityIndividualFromSe();
    }

    [Test] static public void validation_createOrder_customerDetails_invoice_individual_SE()
    {
        new Webpay.Integration.Test.Webservice.Payment.InvoicePaymentTest()
            .TestInvoiceRequestObjectForCustomerIdentityIndividualFromSe();
    }

    [Test] static public void validation_createOrder_customerDetails_invoice_company_SE()
    {
        new Webpay.Integration.Test.Webservice.Payment.InvoicePaymentTest()
            .TestInvoiceRequestObjectForCustomerIdentityCompanyFromSe();
    }        

    [Test] public static void validation_createOrder_customerDetails_invoice_individual_NL()
    {
        new Webpay.Integration.Test.Webservice.Payment.InvoicePaymentTest()
            .TestInvoiceRequestObjectForCustomerIdentityIndividualFromNl();
    }

    [Test] public static void validation_createOrder_customerDetails_invoice_company_NL()
    {
        new Webpay.Integration.Test.Webservice.Payment.InvoicePaymentTest()
            .TestInvoiceRequestObjectForCustomerIdentityCompanyFromNl();
    }
    
    [Test] public static async Task validation_createOrder_sums_orderRow_shippingFee_invoiceFee_company_NL()
    {
        new Webpay.Integration.Test.Webservice.Payment.InvoicePaymentTest()
            .TestInvoiceRequestObjectForSEOrderOnOneProductRow();
    }

    [Test]
    public void test_deliverOrder_deliverInvoiceOrder_with_order_rows_goes_against_DeliverOrderEU()
    {
        var fakeSveaOrderId = 987654;
        var request = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .SetOrderId(fakeSveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST)
            .DeliverInvoiceOrder()
                .PrepareRequest();

        Assert.IsInstanceOf<DeliverOrderEuRequest>(request);
    }

    [Test]
    public void test_deliverOrder_deliverInvoiceOrder_without_order_rows_throws_validation_exception()
    {
        var fakeSveaOrderId = 987654;

        var ex = Assert.Throws<Webpay.Integration.Exception.SveaWebPayValidationException>(() =>             
            WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            //.AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .SetOrderId(fakeSveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .SetInvoiceDistributionType(DistributionType.POST)
            .DeliverInvoiceOrder()
                .PrepareRequest()
        );
        Assert.That(ex.Message, Is.EqualTo("MISSING VALUE - No order or fee has been included. Use AddOrderRow(...) or AddFee(...)."));
    }

    [Test]
    public void test_deliverOrder_deliverCardOrder_with_order_rows_returns_HostedAdminRequest()
    {
        var fakeSveaOrderId = 987654;
        var request = WebpayConnection.DeliverOrder(SveaConfig.GetDefaultConfig())
            .AddOrderRow(TestingTool.CreateExVatBasedOrderRow("1"))
            .SetOrderId(fakeSveaOrderId)
            .SetCountryCode(CountryCode.SE)
            .DeliverCardOrder();

        Assert.IsInstanceOf<HostedActionRequest>(request);
    }
}
