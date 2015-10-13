using System;
using NUnit.Framework;

namespace Webpay.Integration.CSharp.Test.Webservice
{
    // Fixture contains defining tests for main WebpayConnection entrypoint methods & integration package functionality common between all integration packages
    [TestFixture]
    public class WebpayUnitTest
    {
        // REQUEST AND RESPONSE CLASSES
        // createOrder - useInvoicePayment => request class
        [Test] static void createOrder_useInvoicePayment_returns_InvoicePayment_request()
        {
            new Webpay.Integration.CSharp.Test.Webservice.Payment.InvoicePaymentTest()
                .TestInvoiceRequestObjectForCustomerIdentityIndividualFromSe();
        }
        // createOrder - useInvoicePayment - useInvoicePayment - doRequest => response class
        // TODO use existing integration tests or add mocked request response data if available
        
        // createOrder - usePaymentPlanPayment 
        // createOrder - usePaymentPlanPayment - doRequest

        // createOrder - usePaymentMethod

        // createOrder - usePaymentMethod - getPaymentForm
        // createOrder - usePaymentMethod - getPaymentUrl
        // createOrder - usePaymentMethod - doRecur

        // createOrder - usePayPage
        // createOrder - usePayPageCardOnly
        // createOrder - usePayPageDirectBankOnly

        // deliverOrder - deliverInvoiceOrder

        // test_deliverOrder_deliverInvoicePayment_with_orderrows_return_HandleOrder

        // VALIDATION
        // validation - createOrder - addOrderRow required
        // validation - createOrder - addFee optional
        // validation - createOrder - addDiscount optional

        // validation - createOrder - customerDetails - invoice - individual - SE
        [Test] static void validation_createOrder_customerDetails_invoice_individual_SE()
        {
            new Webpay.Integration.CSharp.Test.Webservice.Payment.InvoicePaymentTest()
                .TestInvoiceRequestObjectForCustomerIdentityIndividualFromSe();
        }
        // validation - createOrder - customerDetails - invoice - company - SE
        [Test] static void validation_createOrder_customerDetails_invoice_company_SE()
        {
            new Webpay.Integration.CSharp.Test.Webservice.Payment.InvoicePaymentTest()
                .TestInvoiceRequestObjectForCustomerIdentityCompanyFromSe();
        }        
        // validation - createOrder - customerDetails - invoice - individual - NO
        // validation - createOrder - customerDetails - invoice - company - NO
        // validation - createOrder - customerDetails - invoice - individual - DK
        // validation - createOrder - customerDetails - invoice - company - DK
        // validation - createOrder - customerDetails - invoice - individual - FI
        // validation - createOrder - customerDetails - invoice - company - FI
        // validation - createOrder - customerDetails - invoice - individual - DE
        // validation - createOrder - customerDetails - invoice - company - DE
        // validation - createOrder - customerDetails - invoice - individual - NL
        [Test] private static void validation_createOrder_customerDetails_invoice_individual_NL()
        {
            new Webpay.Integration.CSharp.Test.Webservice.Payment.InvoicePaymentTest()
                .TestInvoiceRequestObjectForCustomerIdentityIndividualFromNl();
        }
        // validation - createOrder - customerDetails - invoice - company - NL
        [Test] private static void validation_createOrder_customerDetails_invoice_company_NL()
        {
            new Webpay.Integration.CSharp.Test.Webservice.Payment.InvoicePaymentTest()
                .TestInvoiceRequestObjectForCustomerIdentityCompanyFromNl();
        }
        
        // validation - createOrder - customerDetails - partpayment - individual - SE
        // validation - createOrder - customerDetails - partpayment - company - SE
        // validation - createOrder - customerDetails - partpayment - individual - NO
        // validation - createOrder - customerDetails - partpayment - company - NO
        // validation - createOrder - customerDetails - partpayment - individual - DK
        // validation - createOrder - customerDetails - partpayment - company - DK
        // validation - createOrder - customerDetails - partpayment - individual - FI
        // validation - createOrder - customerDetails - partpayment - company - FI
        // validation - createOrder - customerDetails - partpayment - individual - DE
        // validation - createOrder - customerDetails - partpayment - company - DE
        // validation - createOrder - customerDetails - partpayment - individual - NL
        // validation - createOrder - customerDetails - partpayment - company - NL

        // validation - createOrder - hosted - all payment methods are accepted -- see BV documentation, TODO hosted invoice/paymentplan?
        // validation - createOrder - hosted - setReturnUrl
        // validation - createOrder - hosted - setCallbackUrl
        // validation - createOrder - hosted - setCancelUrl
        // validation - createOrder - hosted - setPayPageLanguageCode
        // validation - createOrder - hosted - setSubscriptionType
        // validation - createOrder - hosted - setSubscriptionId
        // validation - createOrder - hosted - customerDetails - getPaymentUrl
        // TODO how to handle the paymentmethods Invoice/Partpayment? -- not tested?? should be added to documentation if disallowed or deprecated.

        // validation - createOrder - addOrderRow - precisely two of setAmountIncVat, setAmountExVat, setVatPercent required
        // validation - createOrder - addOrderRow - setAmountIncVat, setVatPercent => priceIncludingVat true
        // validation - createOrder - addOrderRow - setAmountExVat, setVatPercent => priceIncludingVat false
        // validation - createOrder - addOrderRow - setAmountIncVat, setAmountExVat => priceIncludingVat ?? TODO

        // validation - createOrder - setName w/setDescription - webservice => concatenated
        // validation - createOrder - setName w/setDescription - hosted => both sent

        // validation - createOrder - sums - addOrderRow - webservice - priceIncludingVat false
        // validation - createOrder - sums - addOrderRow - webservice - priceIncludingVat true
        // validation - createOrder - sums - addOrderRow - hosted - priceIncludingVat false
        // validation - createOrder - sums - addOrderRow - hosted - priceIncludingVat true
        // validation - createOrder - sums - above w/addFee - webservice - priceIncludingVat false
        [Test] private static void validation_createOrder_sums_orderRow_shippingFee_invoiceFee_company_NL()
        {
            new Webpay.Integration.CSharp.Test.Webservice.Payment.InvoicePaymentTest()
                .TestInvoiceRequestObjectForSEorderOnOneProductRow();
        }
        // validation - createOrder - sums - above w/addFee - webservice - priceIncludingVat true
        // validation - createOrder - sums - above w/addFee - hosted - priceIncludingVat false
        // validation - createOrder - sums - above w/addFee - hosted - priceIncludingVat true
        // validation - createOrder - sums - above w/fixedDiscount - webservice - priceIncludingVat false
        // validation - createOrder - sums - above w/fixedDiscount - webservice - priceIncludingVat true
        // validation - createOrder - sums - above w/fixedDiscount - hosted - priceIncludingVat false
        // validation - createOrder - sums - above w/fixedDiscount - hosted - priceIncludingVat true
        // validation - createOrder - sums - above w/relativeDiscount - webservice - priceIncludingVat false
        // validation - createOrder - sums - above w/relativeDiscount - webservice - priceIncludingVat true
        // validation - createOrder - sums - above w/relativeDiscount - hosted - priceIncludingVat false
        // validation - createOrder - sums - above w/relativeDiscount - hosted - priceIncludingVat true

    }
}
